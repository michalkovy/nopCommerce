
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Flexibee
{
    public class FlexibeeSettings : ISettings
    {
        public string FlexibeeExternalIdPrefix { get; set; }
        public string WidgetZone { get; set; }
    }
}