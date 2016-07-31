using System.Web.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Misc.WebImporter.Models;
using System;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Services.Catalog;
using Nop.Plugin.Misc.WebImporter.Services;
using Nop.Services.Localization;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Nop.Services.Media;
using HtmlAgilityPack;
using System.Xml.XPath;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.WebImporter;
using Nop.Services.Seo;
using Nop.Plugin.Misc.WebImporter.Domain;
using Nop.Web.Framework;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Services;

namespace Nop.Plugin.Misc.WebImporter.Controllers
{
    [AdminAuthorize]
    public class MiscWebImporterController : Controller 
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWebImporterSiteService _webImporterSiteService;
        private readonly IWebImporterLinkService _webImporterLinkService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor
        public MiscWebImporterController(
            IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPictureService pictureService, 
            IWorkContext workContext,
            ISettingService settingService,
            IOrderService orderService,
            ILogger logger,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IStoreContext storeContext,
            IWebImporterSiteService webImporterSiteService,
            IWebImporterLinkService webImporterLinkService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IUrlRecordService urlRecordService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _pictureService = pictureService;
            _workContext = workContext;
            _settingService = settingService;
            _orderService = orderService;
            _logger = logger;
            _storeService = storeService;
            _storeContext = storeContext;
            _webImporterSiteService = webImporterSiteService;
            _webImporterLinkService = webImporterLinkService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
        }
        #endregion

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.AvailableSitesToImport = (SupportedSite.Zacuto).ToSelectList();
            model.Result = "";

            return View("~/Plugins/Misc.WebImporter/Views/MiscWebImporter/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("import")]
        public ActionResult Import(ConfigurationModel model)
        {
            try
            {
                //ImportSites();
                //ImportProfifoto();
                //ImportZacutoCategory(model.PageUrl);
                foreach (var site in Enum.GetValues(typeof(SupportedSite)))
                {
                    ImportSite(model.UpdatePictures, (SupportedSite)site);
                }
                model.Result = "Úspěch";
            }
            catch (Exception exc)
            {
                model.Result = exc.Message;
                _logger.Error(exc.Message, exc);
            }
            model.AvailableSitesToImport = ((SupportedSite)model.SiteToImport).ToSelectList();
            return View("~/Plugins/Misc.WebImporter/Views/MiscWebImporter/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("importSite")]
        public ActionResult ImportSite(ConfigurationModel model)
        {
            try
            {
                ImportSite(model.UpdatePictures, (SupportedSite)model.SiteToImport);
                model.Result = "Úspěch";
            }
            catch (Exception exc)
            {
                model.Result = exc.Message;
                _logger.Error(exc.Message, exc);
            }
            model.AvailableSitesToImport = ((SupportedSite)model.SiteToImport).ToSelectList();
            return View("~/Plugins/Misc.WebImporter/Views/MiscWebImporter/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("importCategory")]
        public ActionResult ImportCategory(ConfigurationModel model)
        {
            try
            {
                ImportCategory(model.PageUrl, model.UpdatePictures, (SupportedSite)model.SiteToImport);
                model.Result = "Úspěch";
            }
            catch (Exception exc)
            {
                model.Result = exc.Message;
                _logger.Error(exc.Message, exc);
            }
            model.AvailableSitesToImport = ((SupportedSite)model.SiteToImport).ToSelectList();
            return View("~/Plugins/Misc.WebImporter/Views/MiscWebImporter/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("importProduct")]
        public ActionResult ImportProduct(ConfigurationModel model)
        {
            try
            {
                var url = new Uri(model.PageUrl);
                var configuration = Configurations.GetConfiguration((SupportedSite)model.SiteToImport);
                var webImporterLinks = _webImporterLinkService.GetAllWebImporterLinks();
                var importer = new ProductImporter(_productService, _categoryService, _manufacturerService, _pictureService, _logger, _languageService, _localizedEntityService, configuration, _storeService, _storeMappingService, _urlRecordService, webImporterLinks);
                importer.ImportOneProduct(url, null, model.UpdatePictures);

                model.Result = "Úspěch";
            }
            catch (Exception exc)
            {
                model.Result = exc.Message;
                _logger.Error(exc.Message, exc);
            }
            model.AvailableSitesToImport = ((SupportedSite)model.SiteToImport).ToSelectList();
            return View("~/Plugins/Misc.WebImporter/Views/MiscWebImporter/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("importProfifotoStockAvailability")]
        public ActionResult ImportProfifotoStockAvailability(ConfigurationModel model)
        {
            try
            {
                ImportProfifotoStockAvailability();
                model.Result = "Úspěch";
            }
            catch (Exception exc)
            {
                model.Result = exc.Message;
                _logger.Error(exc.Message, exc);
            }
            model.AvailableSitesToImport = ((SupportedSite)model.SiteToImport).ToSelectList();
            return View("~/Plugins/Misc.WebImporter/Views/MiscWebImporter/Configure.cshtml", model);
        }

        #region WebImporterLinkGrid
        [HttpPost]
        public ActionResult WebLinkList(DataSourceRequest command)
        {
            var webLinkModelList = _webImporterLinkService.GetAllWebImporterLinks().Select(link =>
                new WebLinkModel()
                {
                    Id = link.Value.Id,
                    Url = link.Value.Url,
                    CategoryId = link.Value.CategoryId,
                    ManufacturerId = link.Value.ManufacturerId
                }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = webLinkModelList,
                Total = webLinkModelList.Count
            };

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        public ActionResult WebLinkUpdate(WebLinkModel model)
        {
            var webLinkToUpdate = _webImporterLinkService.GetAllWebImporterLinks().Where(link => model.Id == link.Value.Id).Select(link => link.Value).First();

            if (webLinkToUpdate != null)
            {
                webLinkToUpdate.CategoryId = model.CategoryId;
                webLinkToUpdate.ManufacturerId = model.ManufacturerId;
                webLinkToUpdate.Url = model.Url;

                _webImporterLinkService.Update(webLinkToUpdate);
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult WebLinkDelete(WebLinkModel model)
        {
            var webLinkToDelete = _webImporterLinkService.GetAllWebImporterLinks().Where(link => model.Id == link.Value.Id).Select(link => link.Value).First();

            if (webLinkToDelete != null)
            {
                _webImporterLinkService.Delete(webLinkToDelete);
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult WebLinkInsert(WebLinkModel model)
        {
            var webLinkToInsert = new WebImporterLink()
            {
                CategoryId = model.CategoryId,
                ManufacturerId = model.ManufacturerId,
                Url = model.Url
            };

            _webImporterLinkService.Insert(webLinkToInsert);

            return new NullJsonResult();
        }
        #endregion

        private void ImportSite(bool updatePictures, SupportedSite site)
        {
            var configuration = Configurations.GetConfiguration(site);
            var webImporterLinks = _webImporterLinkService.GetAllWebImporterLinks();
            var importer = new ProductImporter(_productService, _categoryService, _manufacturerService, _pictureService, _logger, _languageService, _localizedEntityService, configuration, _storeService, _storeMappingService, _urlRecordService, webImporterLinks);
            importer.TraverseSite(updatePictures);
        }

        private void ImportCategory(string url, bool updatePictures, SupportedSite site)
        {
            var uri = new Uri(url);
            var configuration = Configurations.GetConfiguration(site);
            var webImporterLinks = _webImporterLinkService.GetAllWebImporterLinks();
            var importer = new ProductImporter(_productService, _categoryService, _manufacturerService, _pictureService, _logger, _languageService, _localizedEntityService, configuration, _storeService, _storeMappingService, _urlRecordService, webImporterLinks);
            importer.TraverseCategory(uri, "", updatePictures);
        }

        private void ImportProfifotoStockAvailability()
        {
            var configuration = Configurations.GetConfiguration(SupportedSite.Profifoto);
            var webImporterLinks = _webImporterLinkService.GetAllWebImporterLinks();
            var importer = new ProductImporter(_productService, _categoryService, _manufacturerService, _pictureService, _logger, _languageService, _localizedEntityService, configuration, _storeService, _storeMappingService, _urlRecordService, webImporterLinks);
            importer.TraverseSiteForStockAvailability();
        }
    }
}
