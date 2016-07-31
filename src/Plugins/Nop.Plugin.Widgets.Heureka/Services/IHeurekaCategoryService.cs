using System.Collections.Generic;
using Nop.Plugin.Widgets.Heureka.Domain;

namespace Nop.Plugin.Widgets.Heureka.Services
{
    public interface IHeurekaCategoryService
    {
        /// <summary>
        /// Inserts 
        /// </summary>
        /// <param name="heurekaCategory">Map of NopCommerce category to heureka category</param>
        void Insert(HeurekaCategory heurekaCategory);

        IList<HeurekaCategory> GetAllHeurekaCategories();
    }
}