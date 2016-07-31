using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Flexibee.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            AvailableZones = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.ChooseZone")]
        public string ZoneId { get; set; }
        public IList<SelectListItem> AvailableZones { get; set; }

        public string IntegrationResult { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Flexibee.FlexibeeExternalIdPrefix")]
        [AllowHtml]
        public string FlexibeeExternalIdPrefix { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Flexibee.OrderId")]
        public int OrderId { get; set; }
    }
}