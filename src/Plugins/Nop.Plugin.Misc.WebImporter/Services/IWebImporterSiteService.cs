using System.Collections.Generic;
using Nop.Plugin.Misc.WebImporter.Domain;

namespace Nop.Plugin.Misc.WebImporter.Services
{
    public interface IWebImporterSiteService
    {
        /// <summary>
        /// Inserts 
        /// </summary>
        /// <param name="heurekaCategory">Map of NopCommerce category to heureka category</param>
        void Insert(WebImporterSite webImporterSite);

        IList<WebImporterSite> GetAllWebImporterSites();
    }
}