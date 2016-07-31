using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.WebImporter.Models
{
    public class WebLinkModel : BaseNopModel
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Plugins.Misc.WebImporter.WebLinkModel.Url")]
        public string Url { get; set; }

        [NopResourceDisplayName("Plugins.Misc.WebImporter.WebLinkModel.Category")]
        public int CategoryId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.WebImporter.WebLinkModel.Manufacturer")]
        public int ManufacturerId { get; set; }
    }
}