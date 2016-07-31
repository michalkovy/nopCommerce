using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Tasks;

namespace Nop.Plugin.Feed.Zbozi
{
    public class StaticFileGenerationTask : ITask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            var pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();

            //is plugin installed?
            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("PromotionFeed.Zbozi");
            if (pluginDescriptor == null || !pluginDescriptor.Installed)
                return;

            //plugin
            var plugin = pluginDescriptor.Instance() as ZboziService;
            if (plugin == null)
                return;

            plugin.GenerateStaticFile();
        }
    }
}