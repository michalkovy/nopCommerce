using System;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Nop.Plugin.Widgets.Flexibee
{
    public class OrderCancelledEventConsumer : IConsumer<OrderCancelledEvent>
    {
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;

        public OrderCancelledEventConsumer(IPluginFinder pluginFinder, 
            IOrderService orderService,
            IStoreContext storeContext)
        {
            this._pluginFinder = pluginFinder;
            this._storeContext = storeContext;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(OrderCancelledEvent eventMessage)
        {
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Widgets.Flexibee");
            if (pluginDescriptor == null)
                return;
            if (!_pluginFinder.AuthenticateStore(pluginDescriptor, _storeContext.CurrentStore.Id))
                return;

            var plugin = pluginDescriptor.Instance() as FlexibeePlugin;
            if (plugin == null)
                return;

            var order = eventMessage.Order;

            //cancell flexibee order
            plugin.CancellOrder(order);
        }
    }
}