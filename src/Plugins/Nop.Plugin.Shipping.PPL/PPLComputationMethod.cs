//------------------------------------------------------------------------------
// Contributor(s): mb 10/20/2010. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Routing;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.PPL
{
    /// <summary>
    /// PPL computation method
    /// </summary>
    public class PPLComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly IMeasureService _measureService;
        private readonly IShippingService _shippingService;
        private readonly ISettingService _settingService;
        private readonly PPLSettings _PPLSettings;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public PPLComputationMethod(IMeasureService measureService,
            IShippingService shippingService, ISettingService settingService,
            PPLSettings PPLSettings, ICountryService countryService,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            IOrderTotalCalculationService orderTotalCalculationService, ILogger logger,
            ILocalizationService localizationService)
        {
            this._measureService = measureService;
            this._shippingService = shippingService;
            this._settingService = settingService;
            this._PPLSettings = PPLSettings;
            this._countryService = countryService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._logger = logger;
            this._localizationService = localizationService;
        }
        #endregion

        #region Methods

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            var byWeight = _shippingService.LoadShippingRateComputationMethodBySystemName("Shipping.ByWeight");
            return byWeight.GetShippingOptions(getShippingOptionRequest);
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return null;
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
            controllerName = "ShippingPPL";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Shipping.PPL.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new PPLSettings();
            _settingService.SaveSetting(settings);

            //tracker events
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.InputDepo", "Zásilka přijata od odesílatele do systému PPL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Weighed", "Zásilka vážena na vstupním depu.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Central", "Zásilka se nachází na centrálním překladišti.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.OutputDepo", "Zásilka se nachází na doručujícím depu.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Loading", "Zásilka je doručována.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.NotDeliv", "Zásilka nedoručena. Důvod: {0}");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Deliv", "Zásilka doručena. Převzal: {0}");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Export", "Export");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Import", "Import");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.TakeMoney", "Převzetí dobírkové částky řidičem.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.MoneyToBank", "Odeslán platební příkaz s dobírkovou částkou.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.PPL.Tracker.MoneyToCustomer", "Dobírková částka přisána na účet odesílatele.");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<PPLSettings>();

            //tracker events
            this.DeletePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Departed");
            this.DeletePluginLocaleResource("Plugins.Shipping.PPL.Tracker.ExportScanned");
            this.DeletePluginLocaleResource("Plugins.Shipping.PPL.Tracker.OriginScanned");
            this.DeletePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Arrived");
            this.DeletePluginLocaleResource("Plugins.Shipping.PPL.Tracker.NotDelivered");
            this.DeletePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Booked");
            this.DeletePluginLocaleResource("Plugins.Shipping.PPL.Tracker.Delivered");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodType.Realtime;
            }
        }

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get { return new PPLShipmentTracker(_logger, _localizationService, _PPLSettings); }
        }

        #endregion
    }
}