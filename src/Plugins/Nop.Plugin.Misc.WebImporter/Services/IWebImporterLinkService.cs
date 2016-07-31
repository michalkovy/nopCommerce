using System.Collections.Generic;
using Nop.Plugin.Misc.WebImporter.Domain;

namespace Nop.Plugin.Misc.WebImporter.Services
{
    public interface IWebImporterLinkService
    {
        /// <summary>
        /// Inserts 
        /// </summary>
        /// <param name="heurekaCategory">Map of NopCommerce category to heureka category</param>
        void Insert(WebImporterLink webImporterLink);

        void Update(WebImporterLink webImporterLink);

        void Delete(WebImporterLink webImporterLink);

        Dictionary<string, WebImporterLink> GetAllWebImporterLinks();
    }
}