using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Misc.WebImporter.Domain;

namespace Nop.Plugin.Misc.WebImporter.Services
{
    public class WebImporterLinkService : IWebImporterLinkService
    {
        private readonly IRepository<WebImporterLink> _webImporterLinkRepository;

        public WebImporterLinkService(IRepository<WebImporterLink> webImporterLinkRepository)
        {
            _webImporterLinkRepository = webImporterLinkRepository;
        }

        #region Implementation of IViewTrackingService

        /// <summary>
        /// Inserts 
        /// </summary>
        /// <param name="WebImporterLink">Map of NopCommerce category to heureka category</param>
        public void Insert(WebImporterLink WebImporterLink)
        {
            _webImporterLinkRepository.Insert(WebImporterLink);
        }

        public void Update(WebImporterLink webImporterLink)
        {
            _webImporterLinkRepository.Update(webImporterLink);
        }

        public void Delete(WebImporterLink webImporterLink)
        {
            _webImporterLinkRepository.Delete(webImporterLink);
        }

        #endregion


        public Dictionary<string, WebImporterLink> GetAllWebImporterLinks()
        {
            return _webImporterLinkRepository.Table.ToDictionary(e => new Uri(e.Url).AbsoluteUri);
        }
    }
}