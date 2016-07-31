using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.Heureka
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Widgets.Heureka.Configure",
                 "Plugins/WidgetsHeureka/Configure",
                 new { controller = "WidgetsHeureka", action = "Configure" },
                 new[] { "Nop.Plugin.Widgets.Heureka.Controllers" }
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
