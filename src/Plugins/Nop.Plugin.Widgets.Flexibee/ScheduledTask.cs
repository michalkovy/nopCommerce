using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Tasks;

namespace Nop.Plugin.Widgets.Flexibee
{
    public class ScheduledTask : ITask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            var pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();

            //is plugin installed?
            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("Widgets.Flexibee");
            if (pluginDescriptor == null || !pluginDescriptor.Installed)
                return;

            //plugin
            var plugin = pluginDescriptor.Instance() as FlexibeePlugin;
            if (plugin == null)
                return;

            plugin.ScheduledAction();
        }
    }
}