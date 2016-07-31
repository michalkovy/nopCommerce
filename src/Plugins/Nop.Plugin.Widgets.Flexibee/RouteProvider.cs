using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.Flexibee
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Widgets.Flexibee.Configure",
                 "Plugins/WidgetsFlexibee/Configure",
                 new { controller = "WidgetsFlexibee", action = "Configure" },
                 new[] { "Nop.Plugin.Widgets.Flexibee.Controllers" }
            );

            //routes.MapRoute("Plugin.Widgets.Flexibee.PublicInfo",
            //     "Plugins/WidgetsFlexibee/PublicInfo",
            //     new { controller = "WidgetsFlexibee", action = "PublicInfo" },
            //     new[] { "Nop.Plugin.Widgets.Flexibee.Controllers" }
            //);
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
