using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
//using Autofac.Integration.Mvc;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.Heureka.Data;
using Nop.Plugin.Widgets.Heureka.Domain;
using Nop.Plugin.Widgets.Heureka.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.Heureka
{
    public class ProductViewTrackerDependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_heureka_category";

        #region Implementation of IDependencyRegistrar

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<HeurekaCategoryService>().As<IHeurekaCategoryService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<HeurekaCategoryObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<HeurekaCategory>>()
                .As<IRepository<HeurekaCategory>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }

        #endregion

    }
}