using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Widgets.Heureka.Domain;

namespace Nop.Plugin.Widgets.Heureka.Services
{
    public class HeurekaCategoryService : IHeurekaCategoryService
    {
        private readonly IRepository<HeurekaCategory> _heurekaCategoryRepository;

        public HeurekaCategoryService(IRepository<HeurekaCategory> heurekaCategoryRepository)
        {
            _heurekaCategoryRepository = heurekaCategoryRepository;
        }

        #region Implementation of IViewTrackingService

        /// <summary>
        /// Inserts 
        /// </summary>
        /// <param name="heurekaCategory">Map of NopCommerce category to heureka category</param>
        public void Insert(HeurekaCategory heurekaCategory)
        {
            _heurekaCategoryRepository.Insert(heurekaCategory);
        }

        #endregion


        public IList<HeurekaCategory> GetAllHeurekaCategories()
        {
            return _heurekaCategoryRepository.Table.ToList();
        }
    }
}