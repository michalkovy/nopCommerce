using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.Heureka.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Heureka.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Heureka.HeurekaPrivateKey")]
        [AllowHtml]
        public string HeurekaPrivateKey { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Heureka.ZboziCzCode")]
        [AllowHtml]
        public string ZboziCzCode { get; set; }

        public string Result { get; set; }
    }
}