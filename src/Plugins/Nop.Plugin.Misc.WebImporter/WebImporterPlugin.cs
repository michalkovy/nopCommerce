using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Plugin.Misc.WebImporter.Data;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.WebImporter
{
    public class WebImporterPlugin : BasePlugin, IMiscPlugin
    {
        private readonly WebImporterSiteObjectContext _contextSite;
        private readonly WebImporterLinkObjectContext _contextLink;
        private readonly ISettingService _settingService;

        #region Ctor

        public WebImporterPlugin(ISettingService settingService, WebImporterSiteObjectContext contextSite, WebImporterLinkObjectContext contextLink)
        {
            _settingService = settingService;
            _contextSite = contextSite;
            _contextLink = contextLink;
        }

        #endregion

        #region Methods

        public override void Install()
        {
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.Import", "Import All");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.ImportZacutoSite", "Import Zacuto Site");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.ImportProfifotoSite", "Import Profifoto Site");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.ImportZacutoCategory", "Import Zacuto Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.ImportProfifotoCategory", "Import Profifoto Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.ImportZacutoProduct", "Import Zacuto Product");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.ImportProfifotoProduct", "Import Profifoto Product");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.PageUrl", "Import Page URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.PageUrl.Hint", "Both category and product import actions require an URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.UpdatePictures", "Update Pictures");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.UpdatePictures.Hint", "Update pictures on existing products (new products will be downloaded with images anyway)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.WebImporter.ImportProfifotoStockAvailability", "Update Profifoto Stock Av.");

            _contextSite.Install();
            _contextLink.Install();
            base.Install();
        }

        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.Import");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.ImportZacutoSite");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.ImportProfifotoSite");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.ImportZacutoCategory");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.ImportProfifotoCategory");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.ImportZacutoProduct");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.ImportProfifotoProduct");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.PageUrl");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.PageUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.UpdatePictures");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.UpdatePictures.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.WebImporter.ImportProfifotoStockAvailability");

            _contextSite.Uninstall();
            _contextLink.Uninstall();
            base.Uninstall();
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
            controllerName = "MiscWebImporter";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Misc.WebImporter.Controllers" }, { "area", null } };
        }

        #endregion
    }
}
