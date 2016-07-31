using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using Nop.Services.Stores;
using Nop.Services.Localization;
using Nop.Services.Catalog;
using Nop.Core.Data;

namespace Nop.Plugin.Widgets.Flexibee
{
    public static class OrderImporter
    {
        public static string UpdateOrder(Order order, string flexibeeExternalIdPrefix, IRepository<Product> productRepository, IOrderService orderService)
        {
            string result = "";
            var winstrom = FlexibeeCommunicator.ReceiveFromFlexibee<Flexibee.ObjednavkaPrijata.Export.winstrom>(String.Format("objednavka-prijata/(id='ext:{0}:O{1}').xml?detail=full&relations=polozky", flexibeeExternalIdPrefix, order.Id));
            if (winstrom != null && winstrom.objednavkaprijata.Length > 0)
            {
                var objednavka = winstrom.objednavkaprijata[0];
                order.OrderTotal = Decimal.Parse(objednavka.sumCelkem.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                order.OrderTax = Decimal.Parse(objednavka.sumDphCelkem.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                order.TaxRates = String.Format("15:{0}; 21:{1}",
                    objednavka.sumDphSniz.Value, objednavka.sumDphZakl.Value);
                var doprava = objednavka.polozkyObchDokladu.objednavkaprijatapolozka.Where(p => p.kod.Value == "DOPRAVA").FirstOrDefault();
                if (doprava != null)
                {
                    order.OrderShippingExclTax = Decimal.Parse(doprava.sumZkl.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    order.OrderShippingInclTax = Decimal.Parse(doprava.sumCelkem.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                }
                var doberecne = objednavka.polozkyObchDokladu.objednavkaprijatapolozka.Where(p => p.kod.Value == "DOBERECNE").FirstOrDefault();
                if (doberecne != null)
                {
                    order.PaymentMethodAdditionalFeeExclTax = Decimal.Parse(doberecne.sumZkl.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    order.PaymentMethodAdditionalFeeInclTax = Decimal.Parse(doberecne.sumCelkem.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                }
                var polozkySeZaokrouhlenim = objednavka.polozkyObchDokladu.objednavkaprijatapolozka.Where(p => p.kod.Value != "DOPRAVA" && p.kod.Value != "DOBERECNE" && p.kod.Value != "SLEVABODY");
                order.OrderSubtotalExclTax = polozkySeZaokrouhlenim.Sum(p => Decimal.Parse(p.sumZkl.Value, NumberStyles.Any, CultureInfo.InvariantCulture));
                order.OrderSubtotalInclTax = polozkySeZaokrouhlenim.Sum(p => Decimal.Parse(p.sumCelkem.Value, NumberStyles.Any, CultureInfo.InvariantCulture));
                var polozky = polozkySeZaokrouhlenim.Where(p => p.nazev.Value != "Zaokrouhleno");
                
                //We have a defect - order is sometimes missing some items incorrectly, we don't know why
                //var itemsToDelete = order.OrderItems.Where(i => !polozky.Any(p => p.id.Any(id => id.Value == String.Format("ext:{0}:PON{1}", flexibeeExternalIdPrefix, i.Id)))).ToList();
                //foreach (var item in itemsToDelete)
                //{
                //    orderService.DeleteOrderItem(item);
                //}

                int ziskaneId;
                var polozkyNaPridani = polozky.Where(p => !p.id.Any(id => id.Value.StartsWith("ext:") && Int32.TryParse(id.Value.Replace(String.Format("ext:{0}:PON", flexibeeExternalIdPrefix), ""), out ziskaneId) && order.OrderItems.Any(i => i.Id == ziskaneId)));
                foreach (var polozka in polozkyNaPridani)
                {
                    var manufacturerCode = polozka.cenik.Value.Replace("code:", "");
                    int productId = productRepository.Table.Where(p => p.ManufacturerPartNumber == manufacturerCode).Select(p => p.Id).FirstOrDefault();
                    if (productId == 0)
                        continue;
                    var orderItem = new OrderItem()
                    {
                        ProductId = productId,
                        UnitPriceExclTax = Decimal.Parse(polozka.cenaMj.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                        Quantity = (int)Decimal.Parse(polozka.mnozMj.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                        PriceExclTax = Decimal.Parse(polozka.sumZkl.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                        PriceInclTax = Decimal.Parse(polozka.sumCelkem.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                        OrderId = order.Id,
                    };
                    orderItem.UnitPriceInclTax = orderItem.PriceInclTax / orderItem.Quantity;
                    order.OrderItems.Add(orderItem);
                }
                foreach (var polozka in polozky)
                {
                    var orderItemId = polozka.id.Where(id => id.Value.StartsWith(String.Format("ext:{0}:PON", flexibeeExternalIdPrefix))).Select(eid => Int32.TryParse(eid.Value.Replace(String.Format("ext:{0}:PON", flexibeeExternalIdPrefix), ""), out ziskaneId) ? new Nullable<int>(ziskaneId) : null).FirstOrDefault();
                    if (!orderItemId.HasValue)
                        continue;
                    var orderItem = order.OrderItems.Where(i => i.Id == orderItemId.Value).FirstOrDefault();
                    if (orderItem == null)
                        continue;
                    orderItem.UnitPriceExclTax = Decimal.Parse(polozka.cenaMj.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    orderItem.Quantity = (int)Decimal.Parse(polozka.mnozMj.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    orderItem.PriceExclTax = Decimal.Parse(polozka.sumZkl.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    orderItem.PriceInclTax = Decimal.Parse(polozka.sumCelkem.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    orderItem.UnitPriceInclTax = orderItem.PriceInclTax / orderItem.Quantity;
                }
                orderService.UpdateOrder(order);
            }
            else
            {
                result += "Order doesn't exist in Flexibee";
            }

            return result;
        }
    }
}
