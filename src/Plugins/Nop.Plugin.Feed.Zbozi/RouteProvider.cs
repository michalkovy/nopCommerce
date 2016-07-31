using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Feed.Zbozi
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Feed.Zbozi.Configure",
                 "Plugins/FeedZbozi/Configure",
                 new { controller = "FeedZbozi", action = "Configure" },
                 new[] { "Nop.Plugin.Feed.Zbozi.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
