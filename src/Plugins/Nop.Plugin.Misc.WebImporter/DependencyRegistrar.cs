using System;
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
//using Autofac.Integration.Mvc;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.WebImporter.Data;
using Nop.Plugin.Misc.WebImporter.Domain;
using Nop.Plugin.Misc.WebImporter.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.WebImporter
{
    public class ProductViewTrackerDependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME_SITE = "nop_object_context_web_importer_site";
        private const string CONTEXT_NAME_LINK = "nop_object_context_web_importer_link";

        #region Implementation of IDependencyRegistrar

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<WebImporterSiteService>().As<IWebImporterSiteService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<WebImporterSiteObjectContext>(builder, CONTEXT_NAME_SITE);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<WebImporterSite>>()
                .As<IRepository<WebImporterSite>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME_SITE))
                .InstancePerLifetimeScope();

            builder.RegisterType<WebImporterLinkService>().As<IWebImporterLinkService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<WebImporterLinkObjectContext>(builder, CONTEXT_NAME_LINK);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<WebImporterLink>>()
                .As<IRepository<WebImporterLink>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME_LINK))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }

        #endregion
    }
}