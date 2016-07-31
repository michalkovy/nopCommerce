using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping.Tracking;
using System.ServiceModel;

namespace Nop.Plugin.Shipping.PPL
{
    public class PPLShipmentTracker : IShipmentTracker
    {
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly PPLSettings _PPLSettings;

        public PPLShipmentTracker(ILogger logger, ILocalizationService localizationService, PPLSettings PPLSettings)
        {
            this._logger = logger;
            this._localizationService = localizationService;
            this._PPLSettings = PPLSettings;
        }

        /// <summary>
        /// Gets if the current tracker can track the tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>True if the tracker can track, otherwise false.</returns>
        public virtual bool IsMatch(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return false;

            long trackingNumberAsNumber;
            if (!Int64.TryParse(trackingNumber, out trackingNumberAsNumber))
                return false;

            if (trackingNumber.Length != 11)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a url for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>A url to a tracking page.</returns>
        public virtual string GetUrl(string trackingNumber)
        {
            string url = "http://ppl.cz/main2.aspx?cls=Package&idSearch={0}";
            url = string.Format(url, trackingNumber);
            return url;
        }

        /// <summary>
        /// Gets all events for a tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>List of Shipment Events.</returns>
        public virtual IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            try
            {
                //use try-catch to ensure exception won't be thrown is web service is not available
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Name = "IEGateSoap";
                var track = new Ppl.EGateSoapClient(binding, new EndpointAddress("http://www.ppl.cz/iegate/IEGate.asmx"));
                var packageInfo = track.GetPackageInfo(trackingNumber);
                return PackageInfoToStatusEvents(packageInfo);
            }
            catch (SoapException ex)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("SoapException Message= {0}.", ex.Message);
                sb.AppendFormat("SoapException Category:Code:Message= {0}.", ex.Detail.LastChild.InnerText);
                //sb.AppendFormat("SoapException XML String for all= {0}.", ex.Detail.LastChild.OuterXml);
                _logger.Error(string.Format("Error while getting PPL shipment tracking info - {0}", trackingNumber), new Exception(sb.ToString()));
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error while getting PPL shipment tracking info - {0}", trackingNumber), exc);
            }
            return new List<ShipmentStatusEvent>();
        }

        private IList<ShipmentStatusEvent> PackageInfoToStatusEvents(Ppl.PackageInfo package)
        {
            var result = new List<ShipmentStatusEvent>();
            string inputCountry = package.InputCountry;
            string outputCountry = String.IsNullOrWhiteSpace(package.OutputCountry) ? inputCountry : package.OutputCountry;
            string recipient = String.Format("{0} {1}", package.RecZipCode, package.RecCity);

            HandleShipmentStatusEvent(result, package.DateInputDepo, inputCountry, "Plugins.Shipping.PPL.Tracker.InputDepo", package.InputDepoName);
            HandleShipmentStatusEvent(result, package.DateWeighed, inputCountry, "Plugins.Shipping.PPL.Tracker.Weighed", package.InputDepoName);
            HandleShipmentStatusEvent(result, package.DateCentral, inputCountry, "Plugins.Shipping.PPL.Tracker.Central", "");
            HandleShipmentStatusEvent(result, package.DateOutputDepo, outputCountry, "Plugins.Shipping.PPL.Tracker.OutputDepo", package.OutputDepoName);
            HandleShipmentStatusEvent(result, package.LoadingDate, outputCountry, "Plugins.Shipping.PPL.Tracker.Loading", package.OutputDepoName);
            HandleShipmentStatusEvent(result, package.DateNotDeliv, outputCountry, "Plugins.Shipping.PPL.Tracker.NotDeliv", recipient, String.Format("{0} ({1})", package.NotDelivName, package.NotDelivCode));
            HandleShipmentStatusEvent(result, package.DateDeliv, outputCountry, "Plugins.Shipping.PPL.Tracker.Deliv", recipient, package.DelivPerson);
            HandleShipmentStatusEvent(result, package.DateExport, inputCountry, "Plugins.Shipping.PPL.Tracker.Export", "");
            HandleShipmentStatusEvent(result, package.DateImport, inputCountry, "Plugins.Shipping.PPL.Tracker.Import", "");
            HandleShipmentStatusEvent(result, package.DateTakeMoney, inputCountry, "Plugins.Shipping.PPL.Tracker.TakeMoney", "");
            HandleShipmentStatusEvent(result, package.DateMoneyToBank, inputCountry, "Plugins.Shipping.PPL.Tracker.MoneyToBank", "");
            HandleShipmentStatusEvent(result, package.DateMoneyToCustomer, inputCountry, "Plugins.Shipping.PPL.Tracker.MoneyToCustomer", "");

            return result;
        }

        private void HandleShipmentStatusEvent(IList<ShipmentStatusEvent> result, string dateFromService, string country, string resourceName, string location, string eventParameter = null)
        {
            DateTime statusTime;
            if (DateTime.TryParse(dateFromService, out statusTime))
            {
                string resource = _localizationService.GetResource(resourceName);
                result.Add(new ShipmentStatusEvent()
                {
                    CountryCode = country,
                    Date = statusTime,
                    EventName = String.IsNullOrEmpty(eventParameter) ? resource : String.Format(resource, eventParameter),
                    Location = location
                });
            }
        }

        public virtual void GetDeliveryAndPaymentDates(string trackingNumber, out DateTime? deliveryDate, out DateTime? paymentDate, out float? weight)
        {
            deliveryDate = null;
            paymentDate = null;
            weight = null;
            try
            {
                //use try-catch to ensure exception won't be thrown is web service is not available
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Name = "IEGateSoap";
                var track = new Ppl.EGateSoapClient(binding, new EndpointAddress("http://www.ppl.cz/iegate/IEGate.asmx"));
                var packageInfo = track.GetPackageInfo(trackingNumber);

                DateTime parsedDate;
                if (DateTime.TryParse(packageInfo.DateDeliv, out parsedDate))
                    deliveryDate = parsedDate;

                if (DateTime.TryParse(packageInfo.DateTakeMoney, out parsedDate))
                    paymentDate = parsedDate;

                if (DateTime.TryParse(packageInfo.DateWeighed, out parsedDate))
                    weight = packageInfo.Weight <= 0 ? (float?)null : packageInfo.Weight;
            }
            catch (SoapException ex)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("SoapException Message= {0}.", ex.Message);
                sb.AppendFormat("SoapException Category:Code:Message= {0}.", ex.Detail.LastChild.InnerText);
                //sb.AppendFormat("SoapException XML String for all= {0}.", ex.Detail.LastChild.OuterXml);
                _logger.Error(string.Format("Error while getting PPL shipment tracking info - {0}", trackingNumber), new Exception(sb.ToString()));
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error while getting PPL shipment tracking info - {0}", trackingNumber), exc);
            }
        }
    }

}