using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Widgets.Heureka.Models;
using Nop.Plugin.Widgets.Heureka.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Core.Domain.Orders;
using System;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Common;
using Nop.Core;
using System.Linq;


namespace Nop.Plugin.Widgets.Heureka.Controllers
{
    public class WidgetsHeurekaController : BasePluginController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ICategoryService _categoryService;
        private readonly IHeurekaCategoryService _heurekaCategoryService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor
        public WidgetsHeurekaController(IWorkContext workContext, ISettingService settingService,
            IOrderService orderService,
            ILogger logger,
            IStoreService storeService,
            IStoreContext storeContext,
            ICategoryService categoryService,
            IHeurekaCategoryService heurekaCategoryService,
            ILanguageService languageService)
        {
            _workContext = workContext;
            _settingService = settingService;
            _orderService = orderService;
            _logger = logger;
            _storeService = storeService;
            _storeContext = storeContext;
            _categoryService = categoryService;
            _heurekaCategoryService = heurekaCategoryService;
            _languageService = languageService;
        }
        #endregion

        #region Widget
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var heurekaSettings = _settingService.LoadSetting<HeurekaSettings>(storeScope);
            var model = new ConfigurationModel();
            model.HeurekaPrivateKey = heurekaSettings.HeurekaPrivateKey;
            model.ZboziCzCode = heurekaSettings.ZboziCzCode;
            model.Enabled = heurekaSettings.Enabled;

            return View("~/Plugins/Widgets.Heureka/Views/WidgetsHeureka/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        [FormValueRequired("save")]
        public ActionResult Configure(ConfigurationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var heurekaSettings = _settingService.LoadSetting<HeurekaSettings>(storeScope);
            if (!ModelState.IsValid)
                return Configure();

            heurekaSettings.HeurekaPrivateKey = model.HeurekaPrivateKey;
            heurekaSettings.ZboziCzCode = model.ZboziCzCode;
            heurekaSettings.Enabled = model.Enabled;

            _settingService.SaveSetting(heurekaSettings, storeScope);

            _settingService.ClearCache();

            return View("~/Plugins/Widgets.Heureka/Views/WidgetsHeureka/Configure.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            var heurekaSettings = _settingService.LoadSetting<HeurekaSettings>(_storeContext.CurrentStore.Id);

            if (heurekaSettings.Enabled && HttpContext != null && HttpContext.CurrentHandler != null)
            {
                try
                {
                    var routeData = ((System.Web.UI.Page)HttpContext.CurrentHandler).RouteData;
                    //Special case, if we are in last step of checkout, upload order to Heureka
                    if (routeData != null && routeData.Values["controller"] != null &&
                        routeData.Values["action"] != null &&
                        routeData.Values["controller"].ToString().Equals("checkout", StringComparison.InvariantCultureIgnoreCase) &&
                        routeData.Values["action"].ToString().Equals("completed", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Order lastOrder = GetLastOrder();
                        var heurekaPrivateKey = heurekaSettings.HeurekaPrivateKey;
                        SendDataToHeureka(heurekaPrivateKey, lastOrder);
                        return Content(heurekaSettings.ZboziCzCode);
                    }

                }
                catch (Exception ex)
                {
                    _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error sending last order to Heureka", ex.ToString());
                }
            }

            return Content("");
        }

        private Order GetLastOrder()
        {
            return _orderService.SearchOrders(_storeContext.CurrentStore.Id, 0, _workContext.CurrentCustomer.Id,pageSize: 1)
                    .FirstOrDefault();
        }


        private void SendDataToHeureka(string privateKey, Order lastOrder)
        {
            //int czechLanguageId = _languageService.GetAllLanguages().First(l => l.LanguageCulture == "cs-CZ").Id;
            var items = new StringBuilder();
            foreach (var productVariant in lastOrder.OrderItems)
            {
                /*
                muselo by se rozsirit o nazev vyrobce
                var variant = productVariant.ProductVariant;
                string name = "";
                if (!String.IsNullOrEmpty(variant.GetLocalized(x => x.Name, czechLanguageId)))
                    name = string.Format("{0} ({1})", variant.Product.GetLocalized(x => x.Name, czechLanguageId), variant.GetLocalized(x => x.Name, czechLanguageId));
                else
                    name = variant.Product.GetLocalized(x => x.Name, czechLanguageId);
                items.AppendFormat(@"itemId[]={0}&product[]={1}&", productVariant.ProductVariantId, Url.Encode(name));*/
                items.AppendFormat(@"itemId[]={0}&", productVariant.ProductId);
            }
            
            var url =
                String.Format(
                    @"http://www.heureka.cz/direct/dotaznik/objednavka.php?id={0}&email={1}&{2}orderid={3}",
                    privateKey, lastOrder.BillingAddress.Email, items, lastOrder.Id);
            _logger.InsertLog(LogLevel.Information, url, url, lastOrder.Customer);
            var request = WebRequest.Create(url);
            request.GetResponse();
        }
        #endregion

        #region SynchronizeHeurekaCategories
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("synchronize")]
        public ActionResult Synchronize(ConfigurationModel model)
        {
            try
            {
                SynchronizeHeurekaCategories();
                model.Result = "Úspěch";
            }
            catch (Exception exc)
            {
                model.Result = exc.Message;
                _logger.Error(exc.Message, exc);
            }
            return View("~/Plugins/Widgets.Heureka/Views/WidgetsHeureka/Configure.cshtml", model);
        }

        public void SynchronizeHeurekaCategories()
        {
            var webCategories = GetWebCategories();
            var webCategoriesByHeurekaId = webCategories.ToDictionary(c => c.Id);
            var heurekaCattegories = _heurekaCategoryService.GetAllHeurekaCategories();
            var heurekaCattegoriesByCategoryId = heurekaCattegories.ToDictionary(c => c.CategoryId);
            var heurekaCattegoriesByHeurekaId = heurekaCattegories.ToDictionary(c => c.HeurekaId);
            var categoriesById = _categoryService.GetAllCategories(showHidden: true).Where(c => heurekaCattegoriesByCategoryId.ContainsKey(c.Id)).ToDictionary(c => c.Id);

            //make sure Heureka parent is created
            if (!heurekaCattegoriesByHeurekaId.ContainsKey(0))
            {
                var category = new Core.Domain.Catalog.Category()
                {
                    Name = "Heureka.cz",
                    CategoryTemplateId = 1,
                    PageSize = 50,
                    Published = false,
                    Deleted = false,
                    DisplayOrder = 1000,
                    CreatedOnUtc = DateTime.Now,
                    UpdatedOnUtc = DateTime.Now
                };
                _categoryService.InsertCategory(category);
                categoriesById.Add(category.Id, category);

                var heurekaCategory = new Domain.HeurekaCategory() {HeurekaId = 0, CategoryId = category.Id};
                _heurekaCategoryService.Insert(heurekaCategory);
                heurekaCattegoriesByCategoryId.Add(heurekaCategory.CategoryId, heurekaCategory);
                heurekaCattegoriesByHeurekaId.Add(heurekaCategory.HeurekaId, heurekaCategory);
            }

            //delete old
            foreach (var category in categoriesById.Values.Where(c => !webCategoriesByHeurekaId.ContainsKey(heurekaCattegoriesByCategoryId[c.Id].HeurekaId) && c.ParentCategoryId != 0))
            {
                category.Deleted = true;
                _categoryService.UpdateCategory(category);
            }

            //add new categories
            foreach (var webCategory in webCategories.Where(c => !heurekaCattegoriesByHeurekaId.ContainsKey(c.Id)))
            {
                var category = new Core.Domain.Catalog.Category()
                                {
                                    Name = webCategory.Name,
                                    ParentCategoryId = heurekaCattegoriesByHeurekaId[webCategory.Parent].CategoryId,
                                    DisplayOrder = webCategory.DisplayOrder,
                                    CategoryTemplateId = 1,
                                    PageSize = 50,
                                    Published = false,
                                    Deleted = false,
                                    CreatedOnUtc = DateTime.Now,
                                    UpdatedOnUtc = DateTime.Now
                                };
                _categoryService.InsertCategory(category);
                categoriesById.Add(category.Id, category);

                var heurekaCategory = new Domain.HeurekaCategory()
                    {
                        HeurekaId = webCategory.Id,
                        CategoryId = category.Id
                    };
                _heurekaCategoryService.Insert(heurekaCategory);
                heurekaCattegoriesByCategoryId.Add(heurekaCategory.CategoryId, heurekaCategory);
                heurekaCattegoriesByHeurekaId.Add(heurekaCategory.HeurekaId, heurekaCategory);
            }

            //update current
            foreach (var heurekaCategory in heurekaCattegories.Where(c => webCategoriesByHeurekaId.ContainsKey(c.HeurekaId)))
            {
                var webCategory = webCategoriesByHeurekaId[heurekaCategory.HeurekaId];
                var category = categoriesById[heurekaCategory.CategoryId];
                if (webCategory.Name != category.Name || category.Deleted == true ||
                    heurekaCattegoriesByCategoryId[category.ParentCategoryId].HeurekaId != webCategory.Parent ||
                    category.DisplayOrder != webCategory.DisplayOrder)
                {
                    category.Name = webCategory.Name;
                    category.Deleted = false;
                    category.ParentCategoryId = heurekaCattegoriesByHeurekaId[webCategory.Parent].CategoryId;
                    category.DisplayOrder = webCategory.DisplayOrder;
                    category.UpdatedOnUtc = DateTime.Now;
                    _categoryService.UpdateCategory(category);
                }
            }
        }

        protected class HeurekaCategory
        {
            public int Id { get; set; }
            public int Parent { get; set; }
            public string Name { get; set; }
            public int DisplayOrder { get; set; }
        }

        protected List<HeurekaCategory> GetWebCategories()
        {
            XElement heurekaSekce = XElement.Load(@"http://www.heureka.cz/direct/xml-export/shops/heureka-sekce.xml");
            return (from category in heurekaSekce.Descendants("CATEGORY")
                              select new HeurekaCategory()
                              {
                                  Id = Int32.Parse(category.Element("CATEGORY_ID").Value),
                                  Name = category.Element("CATEGORY_NAME").Value,
                                  Parent =
                                      category.Parent.Name == "HEUREKA"
                                          ? 0
                                          : Int32.Parse(category.Parent.Element("CATEGORY_ID").Value),
                                  DisplayOrder = category.ElementsBeforeSelf().Count() + 1
                              }).ToList();

        }
        #endregion

    }
}