using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.WebImporter.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public string Result { get; set; }

        [NopResourceDisplayName("Plugins.Misc.WebImporter.PageUrl")]
        public string PageUrl { get; set; }

        [NopResourceDisplayName("Plugins.Misc.WebImporter.UpdatePictures")]
        public bool UpdatePictures { get; set; }

        public int SiteToImport { get; set; }
        [NopResourceDisplayName("Plugins.Misc.WebImporter.SiteToImport")]
        public SelectList AvailableSitesToImport { get; set; }
    }
}