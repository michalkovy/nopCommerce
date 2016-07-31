using Nop.Web.Framework;

namespace Nop.Plugin.Widgets.Flexibee.Models
{
    public class ManufacturerModel
    {
        public int ManufacturerId { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.Flexibee.Fields.ManufacturerName")]
        public string ManufacturerName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.Flexibee.Fields.ManufacturerFlexibeeCode")]
        public string ManufacturerFlexibeeCode { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.Flexibee.Fields.SupplierFlexibeeCode")]
        public string SupplierFlexibeeCode { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.Flexibee.Fields.CurrencyCode")]
        public string CurrencyCode { get; set; }
    }
}