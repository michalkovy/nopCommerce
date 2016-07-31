using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Misc.WebImporter.Domain;

namespace Nop.Plugin.Misc.WebImporter.Services
{
    public class WebImporterSiteService : IWebImporterSiteService
    {
        private readonly IRepository<WebImporterSite> _webImporterSiteRepository;

        public WebImporterSiteService(IRepository<WebImporterSite> webImporterSiteRepository)
        {
            _webImporterSiteRepository = webImporterSiteRepository;
        }

        #region Implementation of IViewTrackingService

        /// <summary>
        /// Inserts 
        /// </summary>
        /// <param name="WebImporterSite">Map of NopCommerce category to heureka category</param>
        public void Insert(WebImporterSite webImporterSite)
        {
            _webImporterSiteRepository.Insert(webImporterSite);
        }

        #endregion


        public IList<WebImporterSite> GetAllWebImporterSites()
        {
            return _webImporterSiteRepository.Table.ToList();
        }
    }
}