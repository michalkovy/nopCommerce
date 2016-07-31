
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Heureka
{
    public class HeurekaSettings : ISettings
    {
        public bool Enabled { get; set; }
        public string HeurekaPrivateKey { get; set; }
        public string ZboziCzCode { get; set; }
    }
}