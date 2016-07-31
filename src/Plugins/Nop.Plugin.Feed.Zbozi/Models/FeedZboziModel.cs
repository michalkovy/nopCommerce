using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.Feed.Zbozi.Models
{
    public class FeedZboziModel
    {
        public FeedZboziModel()
        {
            AvailableCurrencies = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Feed.Zbozi.ProductPictureSize")]
        public int ProductPictureSize { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Zbozi.Currency")]
        public int CurrencyId { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Zbozi.TaskEnabled")]
        public bool TaskEnabled { get; set; }
        [NopResourceDisplayName("Plugins.Feed.Zbozi.GenerateStaticFileEachMinutes")]
        public int GenerateStaticFileEachMinutes { get; set; }

        public IList<SelectListItem> AvailableCurrencies { get; set; }

        public string GenerateFeedResult { get; set; }
    }
}