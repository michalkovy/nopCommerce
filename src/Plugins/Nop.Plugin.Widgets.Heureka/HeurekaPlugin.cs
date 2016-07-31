using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.Heureka.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Widgets.Heureka
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class HeurekaPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly HeurekaCategoryObjectContext _context;

        #endregion

        #region Ctor
        public HeurekaPlugin(ISettingService settingService, HeurekaCategoryObjectContext context)
        {
            this._settingService = settingService;
            _context = context;
        }

        #endregion

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "body_end_html_tag_before" };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsHeureka";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.Heureka.Controllers" }, { "area", null }};
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget Zone</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsHeureka";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.Heureka.Controllers" }, { "area", null }, { "widgetZone", widgetZone } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new HeurekaSettings
                {
                    HeurekaPrivateKey = ""
                };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Heureka.Enabled", "Heureka Integration Enabled");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Heureka.Enabled.Hint", "Should we send data to Heureka for this store?");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Heureka.HeurekaPrivateKey", "Heureka Private Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Heureka.HeurekaPrivateKey.Hint", "Enter integration key for Heureka.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Heureka.ZboziCzCode", "Heureka HTML code for Zbozi.cz");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Heureka.ZboziCzCode.Hint", "Enter integration embedded code for Zbozi.cz to send data for integration.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Heureka.SynchronizeCategories", "Synchronize Categories");
            
            _context.Install();
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Widgets.Heureka.Enabled");
            this.DeletePluginLocaleResource("Plugins.Widgets.Heureka.Enabled.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Heureka.HeurekaPrivateKey");
            this.DeletePluginLocaleResource("Plugins.Widgets.Heureka.HeurekaPrivateKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Heureka.ZboziCzCode");
            this.DeletePluginLocaleResource("Plugins.Widgets.Heureka.ZboziCzCode.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Heureka.SynchronizeCategories");
            
            _context.Uninstall();
            base.Uninstall();
        }
    }
}
