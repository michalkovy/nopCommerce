using System;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Shipping.PPL.Models;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Web.Framework.Controllers;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Core.Domain.Shipping;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Shipping.PPL.Controllers
{
    [AdminAuthorize]
    public class ShippingPPLController : Controller
    {
        private readonly PPLSettings _PPLSettings;
        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkflowMessageService _workflowMessageService;

        public ShippingPPLController(PPLSettings PPLSettings, ISettingService settingService,
            ICountryService countryService, IOrderService orderService,
            IShipmentService shipmentService, ILogger logger, ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService, IEventPublisher eventPublisher, IWorkflowMessageService workflowMessageService)
        {
            this._PPLSettings = PPLSettings;
            this._settingService = settingService;
            this._countryService = countryService;
            this._shipmentService = shipmentService;
            this._orderService = orderService;
            this._logger = logger;
            this._localizationService = localizationService;
            this._orderProcessingService = orderProcessingService;
            this._eventPublisher = eventPublisher;
            this._workflowMessageService = workflowMessageService;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new PPLShippingModel();

            return View("~/Plugins/Shipping.PPL/Views/ShippingPPL/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        [FormValueRequired("save")]
        public ActionResult Configure(PPLShippingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            _settingService.SaveSetting(_PPLSettings);

            return Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("updateOrders")]
        public ActionResult UpdateOrders(PPLShippingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            try
            {
                UpdateStatuses();
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
            }

            return View("~/Plugins/Shipping.PPL/Views/ShippingPPL/Configure.cshtml", model);
        }

        private void UpdateStatuses(bool notifyCustomer = false)
        {
            var shipmentTracker = new PPLShipmentTracker(_logger, _localizationService, _PPLSettings);

            var orders = _orderService.SearchOrders(createdFromUtc: DateTime.UtcNow - TimeSpan.FromDays(90), createdToUtc: DateTime.UtcNow + TimeSpan.FromDays(1), ssIds: (new int[]{ (int)ShippingStatus.Shipped }).ToList() );
            foreach (var order in orders)
            {
                foreach (var shipment in order.Shipments)
                {
                    if (shipment.DeliveryDateUtc != null)
                        continue;

                    if (!shipmentTracker.IsMatch(shipment.TrackingNumber))
                        continue;

                    DateTime? deliveryDate;
                    DateTime? paymentDate;
                    float? weight;
                    shipmentTracker.GetDeliveryAndPaymentDates(shipment.TrackingNumber, out deliveryDate, out paymentDate, out weight);
                    if (weight != null && shipment.TotalWeight != (decimal?)weight)
                    {
                        shipment.TotalWeight = (decimal?)weight;
                        _shipmentService.UpdateShipment(shipment);
                    }
                    if (order.PaymentMethodSystemName == "Payments.CashOnDelivery" && paymentDate != null)
                    {
                        order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
                        order.PaidDateUtc = paymentDate.Value.ToUniversalTime();
                        _orderService.UpdateOrder(order);
                        //add a note
                        order.OrderNotes.Add(new OrderNote()
                        {
                            Note = "Order has been marked as paid",
                            DisplayToCustomer = false,
                            CreatedOnUtc = order.PaidDateUtc.Value
                        });
                        _orderService.UpdateOrder(order);
                        _orderProcessingService.CheckOrderStatus(order);
                        //raise event         
                        if (order.PaymentStatus == PaymentStatus.Paid)
                        {
                            _eventPublisher.Publish(new OrderPaidEvent(order));
                        }
                    }
                    if (deliveryDate != null)
                    {
                        shipment.DeliveryDateUtc = deliveryDate.Value.ToUniversalTime();
                        _shipmentService.UpdateShipment(shipment);
                        order.ShippingStatus = Core.Domain.Shipping.ShippingStatus.Delivered;
                        //add a note
                        order.OrderNotes.Add(new OrderNote()
                        {
                            Note = string.Format("Shipment# {0} has been delivered", shipment.Id),
                            DisplayToCustomer = false,
                            CreatedOnUtc = shipment.DeliveryDateUtc.Value
                        });
                        _orderService.UpdateOrder(order);
                        
                        if (notifyCustomer)
                        {
                            //send email notification
                            int queuedEmailId = _workflowMessageService.SendShipmentDeliveredCustomerNotification(shipment, order.CustomerLanguageId);
                            if (queuedEmailId > 0)
                            {
                                order.OrderNotes.Add(new OrderNote()
                                {
                                    Note = string.Format("\"Delivered\" email (to customer) has been queued. Queued email identifier: {0}.", queuedEmailId),
                                    DisplayToCustomer = false,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                                _orderService.UpdateOrder(order);
                            }
                        }
                        _eventPublisher.PublishShipmentDelivered(shipment);
                    }
                    _orderProcessingService.CheckOrderStatus(order);
                }
            }

            orders = _orderService.SearchOrders(createdFromUtc: DateTime.UtcNow - TimeSpan.FromDays(90), createdToUtc: DateTime.UtcNow + TimeSpan.FromDays(1), ssIds: (new int[] { (int)ShippingStatus.Shipped }).ToList());
            foreach (var order in orders)
            {
                if ((order.PickUpInStore || order.ShippingMethod == "Osobní odběr v Praze") && order.PaymentMethodSystemName == "Payments.PayInStore" && order.PaidDateUtc.HasValue)
                {
                    AddDeliveredShipment(order, order.PaidDateUtc.Value);
                    _orderProcessingService.CheckOrderStatus(order);
                }
            }
        }

        public void AddDeliveredShipment(Order order, DateTime shipmentDateUtc)
        {
            var orderItems = order.OrderItems;

            Shipment shipment = null;
            decimal? totalWeight = null;
            foreach (var orderItem in orderItems)
            {
                //is shippable
                if (!orderItem.Product.IsShipEnabled)
                    continue;

                //ensure that this product can be shipped (have at least one item to ship)
                var qtyToAdd = orderItem.GetTotalNumberOfItemsCanBeAddedToShipment();
                if (qtyToAdd <= 0)
                    continue;

                //ok. we have at least one item. let's create a shipment (if it does not exist)

                var orderItemTotalWeight = orderItem.ItemWeight.HasValue ? orderItem.ItemWeight * qtyToAdd : null;
                if (orderItemTotalWeight.HasValue)
                {
                    if (!totalWeight.HasValue)
                        totalWeight = 0;
                    totalWeight += orderItemTotalWeight.Value;
                }
                if (shipment == null)
                {
                    shipment = new Shipment()
                    {
                        OrderId = order.Id,
                        TrackingNumber = "",
                        TotalWeight = null,
                        ShippedDateUtc = shipmentDateUtc,
                        DeliveryDateUtc = shipmentDateUtc,
                        CreatedOnUtc = shipmentDateUtc,
                    };
                }
                //create a shipment item
                var shipmentItem = new ShipmentItem()
                {
                    OrderItemId = orderItem.Id,
                    Quantity = qtyToAdd,
                };
                shipment.ShipmentItems.Add(shipmentItem);
            }
            //if we have at least one item in the shipment, then save it
            if (shipment != null && shipment.ShipmentItems.Count > 0)
            {
                shipment.TotalWeight = totalWeight;
                _shipmentService.InsertShipment(shipment);
            }
            order.ShippingStatus = Core.Domain.Shipping.ShippingStatus.Delivered;
            //add a note
            order.OrderNotes.Add(new OrderNote()
            {
                Note = string.Format("Shipment# {0} has been delivered", shipment.Id),
                DisplayToCustomer = false,
                CreatedOnUtc = shipment.DeliveryDateUtc.Value
            });
            _orderService.UpdateOrder(order);
        }

    }
}
