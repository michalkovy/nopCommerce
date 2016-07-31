using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Nop.Plugin.Widgets.Flexibee
{
    public class FlexibeeHook : IFlexibeeHook
    {
        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;

        public FlexibeeHook()
        {
            _pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
            _logger = EngineContext.Current.Resolve<ILogger>();
        }

        public void Update(WinstromHook changes)
        {
            try
            {
                if (changes != null && changes.objednavkyPrijate != null)
                {
                    var updatedOrderIds = changes.objednavkyPrijate.SelectMany(o => o.ids ?? new string[] { })
                        .Where(e => e.StartsWith("ext:CIN:O"))
                        .Select(e => Int32.Parse(e.Replace("ext:CIN:O", "")))
                        .Distinct();
                    Plugin.UpdateOrders(updatedOrderIds);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Exception in FlexibeeHook", e);
            }
        }

        FlexibeePlugin Plugin
        {
            get {
                //is plugin installed?
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Widgets.Flexibee");
                if (pluginDescriptor == null || !pluginDescriptor.Installed)
                    return null;

                //plugin
                var plugin = pluginDescriptor.Instance() as FlexibeePlugin;
                return plugin;
            }
        }
    }
}
