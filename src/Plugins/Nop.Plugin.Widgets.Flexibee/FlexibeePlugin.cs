using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Directory;
using Nop.Core;
using Nop.Services.Tax;
using Nop.Core.Domain.Directory;
using System;
using Nop.Services.Orders;
using Nop.Services.Tasks;
using Nop.Core.Domain.Tasks;
using Nop.Services.Stores;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.Flexibee.Controllers;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Widgets.Flexibee
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class FlexibeePlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly CurrencySettings _currencySettings;
        private readonly FlexibeeSettings _flexibeeSettings;
        private readonly IOrderService _orderService;
        private readonly ICountryService _countryService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IStoreService _storeService;
        private readonly ILanguageService _languageService;
        private readonly IRepository<Product> _productRepository;

        #endregion

        #region Ctor
        public FlexibeePlugin(IProductService productService,
            ICategoryService categoryService, 
            IManufacturerService manufacturerService, IPictureService pictureService,
            ICurrencyService currencyService, IWebHelper webHelper,
            ISettingService settingService, ITaxService taxService,
            CurrencySettings currencySettings, FlexibeeSettings flexibeeSettings,
            IOrderService orderService, ICountryService countryService,
            IScheduleTaskService scheduleTaskService,
            IStoreService storeService,
            ILanguageService languageService,
            IRepository<Product> productRepository)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._taxService = taxService;
            this._currencySettings = currencySettings;
            this._flexibeeSettings = flexibeeSettings;
            _orderService = orderService;
            _countryService = countryService;
            _scheduleTaskService = scheduleTaskService;
            _storeService = storeService;
            _languageService = languageService;
            _productRepository = productRepository;
        }

        #endregion

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return !string.IsNullOrWhiteSpace(_flexibeeSettings.WidgetZone)
                       ? new List<string>() { _flexibeeSettings.WidgetZone }
                       : new List<string>() { "head_html_tag" };
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
            controllerName = "WidgetsFlexibee";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.Flexibee.Controllers" }, { "area", null }};
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsFlexibee";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.Flexibee.Controllers" }, { "area", null }, { "widgetZone", widgetZone } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new FlexibeeSettings()
                {
                    FlexibeeExternalIdPrefix = ""
                };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Flexibee.UploadOrders", "Upload Orders");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Flexibee.UploadProducts", "Upload Products");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Flexibee.DownloadStock", "Download Stock Quantities");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Flexibee.SuccessResult", "Integration has been successfull.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Flexibee.FlexibeeExternalIdPrefix", "External ID Prefix");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Flexibee.FlexibeeExternalIdPrefix.Hint", "Enter unique prefix, don't change later.");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Widgets.Flexibee.Fields.ManufacturerName", "Manufacturer");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Widgets.Flexibee.Fields.ManufacturerFlexibeeCode", "Manufacturer Flexibee Code");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Widgets.Flexibee.Fields.SupplierFlexibeeCode", "Supplier Flexibee Code");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Widgets.Flexibee.Fields.CurrencyCode", "Currency Code");

            //install a schedule task
            var task = FindScheduledTask();
            if (task == null)
            {
                task = new ScheduleTask
                {
                    Name = "Scheduled Flexibee Integration",
                    //each 30 minutes
                    Seconds = 1800,
                    Type = "Nop.Plugin.Widgets.Flexibee.ScheduledTask, Nop.Plugin.Widgets.Flexibee",
                    Enabled = false,
                    StopOnError = false,
                };
                _scheduleTaskService.InsertTask(task);
            }
            task = FindScheduledExportOrdersTask();
            if (task == null)
            {
                task = new ScheduleTask
                {
                    Name = "Export orders to Flexibee",
                    //each 2 minutes
                    Seconds = 120,
                    Type = "Nop.Plugin.Widgets.Flexibee.OrdersExportTask, Nop.Plugin.Widgets.Flexibee",
                    Enabled = false,
                    StopOnError = false,
                };
                _scheduleTaskService.InsertTask(task);
            }

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Widgets.Flexibee.UploadOrders");
            this.DeletePluginLocaleResource("Plugins.Widgets.Flexibee.UploadProducts");
            this.DeletePluginLocaleResource("Plugins.Widgets.Flexibee.DownloadStock");
            this.DeletePluginLocaleResource("Plugins.Widgets.Flexibee.SuccessResult");
            this.DeletePluginLocaleResource("Plugins.Widgets.Flexibee.FlexibeeExternalIdPrefix");
            this.DeletePluginLocaleResource("Plugins.Widgets.Flexibee.FlexibeeExternalIdPrefix.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.Widgets.Flexibee.Fields.ManufacturerName");
            this.DeletePluginLocaleResource("Nop.Plugin.Widgets.Flexibee.Fields.ManufacturerFlexibeeCode");
            this.DeletePluginLocaleResource("Nop.Plugin.Widgets.Flexibee.Fields.SupplierFlexibeeCode");
            this.DeletePluginLocaleResource("Nop.Plugin.Widgets.Flexibee.Fields.CurrencyCode");

            //Remove scheduled task
            var task = FindScheduledTask();
            if (task != null)
                _scheduleTaskService.DeleteTask(task);
            task = FindScheduledExportOrdersTask();
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            base.Uninstall();
        }

        private ScheduleTask FindScheduledTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.Widgets.Flexibee.ScheduledTask, Nop.Plugin.Widgets.Flexibee");
        }

        private ScheduleTask FindScheduledExportOrdersTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.Widgets.Flexibee.OrdersExportTask, Nop.Plugin.Widgets.Flexibee");
        }

        public void CancellOrder(Order order)
        {
            OrderExporter.OrderCancel("CIN", order);
        }

        public void ScheduledAction()
        {
            WidgetsFlexibeeController.UpdateSklad("CIN", _productRepository, _productService);
        }

        protected DateTime GetLastAutomaticOrdersExport()
        {
            return this._settingService.GetSettingByKey<DateTime>("Widgets.Flexibee.LastAutomaticOrdersExport", DateTime.UtcNow);
        }

        protected void SetLastAutomaticOrdersExport(DateTime lastAutomaticOrdersExport)
        {
            _settingService.SetSetting("Widgets.Flexibee.LastAutomaticOrdersExport", lastAutomaticOrdersExport);
        }

        public void AutomaticOrdersExport()
        {
            var createdFromUtc = GetLastAutomaticOrdersExport();
            var createdToUtc = DateTime.UtcNow - TimeSpan.FromMinutes(1);

            OrderExporter.OrdersExport(_orderService, "CIN", _countryService, _taxService, false, createdFromUtc, createdToUtc, _storeService, _languageService);

            SetLastAutomaticOrdersExport(createdToUtc);
        }

        public void UpdateOrders(IEnumerable<int> orderIds)
        {
            foreach (int orderId in orderIds)
            {
                var order = _orderService.GetOrderById(orderId);
                if (order != null)
                {
                    OrderImporter.UpdateOrder(order, "CIN", _productRepository, _orderService);
                }
            }
        }
    }
}
