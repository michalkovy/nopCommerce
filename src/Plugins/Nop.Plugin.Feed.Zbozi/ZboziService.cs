using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Shipping;
using Nop.Core.Html;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Tax;
using System.Xml;
using Nop.Core.Domain.Tasks;
using Nop.Services.Tasks;
using System.Web;
using Nop.Services.Common;
using Nop.Core.Domain.Stores;
using Nop.Services.Stores;
using System.Linq;
using Nop.Core.Data;
using System.Xml.Linq;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using System.Data.Entity.SqlServer;
using Nop.Services.Logging;

namespace Nop.Plugin.Feed.Zbozi
{
    public class ZboziService : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly ZboziSettings _ZboziSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILanguageService _languageService;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Picture> _pictureRepository;
        private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly ILogger _logger;

        #endregion

        #region Ctor
        public ZboziService(IProductService productService,
            ICategoryService categoryService, 
            IManufacturerService manufacturerService, IPictureService pictureService,
            ICurrencyService currencyService, IWebHelper webHelper,
            ISettingService settingService, ITaxService taxService,
            ZboziSettings ZboziSettings, CurrencySettings currencySettings,
            IScheduleTaskService scheduleTaskService, IStoreService storeService,
            IUrlRecordService urlRecordService, ILanguageService languageService,
            IRepository<Product> productRepository, IRepository<ProductCategory> productCategoryRepository,
            IRepository<Category> categoryRepository, IRepository<StoreMapping> storeMappingRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository, IRepository<UrlRecord> urlRecordRepository,
            IRepository<ProductManufacturer> productManufacturerRepository, IRepository<Manufacturer> manufacturerRepository,
            IRepository<Picture> pictureRepository, IRepository<ProductPicture> productPictureRepository,
            ILogger logger)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._taxService = taxService;
            this._ZboziSettings = ZboziSettings;
            this._currencySettings = currencySettings;
            this._scheduleTaskService = scheduleTaskService;
            this._storeService = storeService;
            this._urlRecordService = urlRecordService;
            this._languageService = languageService;
            this._productRepository = productRepository;
            this._productCategoryRepository = productCategoryRepository;
            this._categoryRepository = categoryRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._urlRecordRepository = urlRecordRepository;
            this._productManufacturerRepository = productManufacturerRepository;
            this._manufacturerRepository = manufacturerRepository;
            this._pictureRepository = pictureRepository;
            this._productPictureRepository = productPictureRepository;
            this._logger = logger;
        }

        #endregion

        #region Utilities

        private Nop.Core.Domain.Directory.Currency GetUsedCurrency()
        {
            var currency = _currencyService.GetCurrencyById(_ZboziSettings.CurrencyId);
            if (currency == null || !currency.Published)
                currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            return currency;
        }

        private static string RemoveSpecChars(string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            s = s.Replace(';', ',');
            s = s.Replace('\r', ' ');
            s = s.Replace('\n', ' ');
            return s;
        }

        private IList<Category> GetCategoryBreadCrumb(Category category, bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var breadCrumb = new List<Category>();

            while (category != null && //category is not null
                !category.Deleted && //category is not deleted
                (category.Published || showHidden)) //category is published
            {
                breadCrumb.Add(category);
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
            }
            breadCrumb.Reverse();
            return breadCrumb;
        }

        private ScheduleTask FindStaticFileGenerationTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.Feed.Zbozi.StaticFileGenerationTask, Nop.Plugin.Feed.Zbozi");
        }

        private ScheduleTask FindPictureFilesGenerationTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.Feed.Zbozi.PictureFilesGenerationTask, Nop.Plugin.Feed.Zbozi");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "FeedZbozi";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Feed.Zbozi.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Generate a feed
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Generated feed</returns>
        public void GenerateFeed(string zboziFilePath, string heurekaFilePath, Store store)
        {
            if (String.IsNullOrWhiteSpace(zboziFilePath))
                throw new ArgumentNullException("zboziFilePath");

            if (store == null)
                throw new ArgumentNullException("store");

            int czechLanguageId = _languageService.GetAllLanguages().First(l => l.LanguageCulture == "cs-CZ").Id;

            var shippingSettings = _settingService.LoadSetting<ShippingSettings>(store.Id);

            var productForStore = from p in _productRepository.Table
                    join sm in _storeMappingRepository.Table
                    on new { c1 = p.Id, c2 = "Product" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                    from sm in p_sm.DefaultIfEmpty()
                    where (!p.LimitedToStores || store.Id == sm.StoreId) && p.Published && !p.Deleted
                    select p;
            var productCategoryBreadCrumbs =
                 from c in GetSortedCategories(czechLanguageId)
                 join pc in _productCategoryRepository.Table
                 on c.Id equals pc.CategoryId
                 group c.BreadCrumb by pc.ProductId into g
                 select new { ProductId = g.Key, CategoryBreadCrumbs = g.ToList() };

            var slug = GetSlug(czechLanguageId, "Product");
            var localizedProductName = GetLocalizedValue(czechLanguageId, "Product", "Name");
            var localizedProductShortDescription = GetLocalizedValue(czechLanguageId, "Product", "ShortDescription");
            var localizedProductFullDescription = GetLocalizedValue(czechLanguageId, "Product", "FullDescription");
            var manufacturerNames = GetManufacturers(czechLanguageId);

            var pictureProductUrls = GetPictureUrls(czechLanguageId, store.Url);

            var products =
                from p in productForStore.Where(p => p.ParentGroupedProductId == 0)
                join p2 in productForStore.Where(p3 => p3.VisibleIndividually == false)
                on p.Id equals p2.ParentGroupedProductId into ppv
                from pv in ppv.DefaultIfEmpty(p)
                join catB in productCategoryBreadCrumbs
                on pv.Id equals catB.ProductId
                let slu = slug.Where(sl => p.Id == sl.EntityId).Select(s => s.Slug).DefaultIfEmpty(p.Name).FirstOrDefault()
                let manufacturer = manufacturerNames.Where(lv => pv.Id == lv.ProductId).Select(lv => lv.ManufacturerName).FirstOrDefault()
                let name = localizedProductName.Where(lv => pv.Id == lv.EntityId).Select(lv => lv.Value).DefaultIfEmpty(pv.Name).FirstOrDefault()
                let nameWithManufacturer = SqlFunctions.PatIndex("%" + manufacturer + "%", name) > 0 ? name : manufacturer + " " + name
                let shortDescription = localizedProductShortDescription.Where(lv => pv.Id == lv.EntityId).Select(lv => lv.Value).DefaultIfEmpty(pv.ShortDescription).FirstOrDefault()
                let fullDescription = localizedProductFullDescription.Where(lv => pv.Id == lv.EntityId).Select(lv => lv.Value).DefaultIfEmpty(pv.FullDescription).FirstOrDefault()
                let pictureUrlAll = pictureProductUrls.Where(pp => pp.ProductId == pv.Id).Union(pictureProductUrls.Where(pp => pp.ProductId == p.Id)).Select(pp => pp.PictureUrl)
                let pictureUrl = pictureUrlAll.FirstOrDefault()
                let priceWithVat = pv.TaxCategoryId == 9 ? pv.Price : pv.TaxCategoryId == 7 ? 1.21M * pv.Price : 1.15M * pv.Price
                select new ShopItem()
                {
                    Id = pv.Id,
                    Name = nameWithManufacturer,
                    Decription = shortDescription + " " + fullDescription,
                    Url = pv.ParentGroupedProductId == 0 ?
                        store.Url + slu :
                        store.Url + slu + "#addtocart_" + SqlFunctions.StringConvert((decimal)pv.Id).Trim() + "_EnteredQuantity",
                    OnStock = pv.StockQuantity > 0,
                    ImgUrl = pictureUrl,
                    Price = pv.Price,
                    Vat = p.TaxCategoryId == 9 ? "0" : p.TaxCategoryId == 7 ? "0.21" : "0.15",
                    PriceWithVat = priceWithVat,
                    Manufacturer = manufacturer,
                    ManufacturerPartNumber = p.ManufacturerPartNumber,
                    Ean = pv.Gtin == null ? null : pv.Gtin.Length == 13 ? pv.Gtin : pv.Gtin.Length == 12 ? "0" + pv.Gtin : null,
                    ExtraMessage = (shippingSettings.FreeShippingOverXEnabled &&
                        (shippingSettings.FreeShippingOverXIncludingTax ?
                        priceWithVat > shippingSettings.FreeShippingOverXValue
                        : pv.Price > shippingSettings.FreeShippingOverXValue))
                        ? "free_delivery" : "free_store_pickup",
                    Categories = catB.CategoryBreadCrumbs,
                    AlternativePictures = pictureUrlAll.Where(pic => pic != pictureUrl).ToList()
                };

            FinishZboziFeedGeneration(products, zboziFilePath);
            FinishHeurekaFeedGeneration(products, heurekaFilePath);
        }

        protected class ShopItem
        {
            public int Id;
            public string Name;
            public string Decription;
            public string Url;
            public bool OnStock;
            public string ImgUrl;
            public decimal Price;
            public string Vat;
            public decimal PriceWithVat;
            public string Manufacturer;
            public string ManufacturerPartNumber;
            public string Ean;
            public string ExtraMessage;
            public List<string> Categories;
            public List<string> AlternativePictures;
        }

        private void FinishZboziFeedGeneration(IQueryable<ShopItem> products, string zboziFilePath)
        {
            var productsExported = products.AsEnumerable().Select(p =>
                            new XElement("SHOPITEM",
                                new XElement("ITEM_ID", p.Id),
                                new XElement("PRODUCTNAME", p.Name),
                                new XElement("DESCRIPTION", p.Decription),
                                new XElement("URL", p.Url),
                                new XElement("PRICE_VAT", String.Format("{0:0}", p.PriceWithVat)),
                                new XElement("DELIVERY_DATE", p.OnStock ? "0" : null),
                                new XElement("IMGURL", p.ImgUrl),
                                new XElement("EAN", p.Ean),
                                new XElement("PRODUCTNO", p.ManufacturerPartNumber),
                                new XElement("MANUFACTURER", p.Manufacturer),
                                new XElement("PRODUCT", p.Name),  //TODO: add manufacturer to name
                                new XElement("ITEM_TYPE", "new"),
                                new XElement("EXTRA_MESSAGE", p.ExtraMessage),
                                new XElement("SHOP_DEPOTS", p.OnStock ? "2650899" : null)
                                //p.Categories.Select(c => new XElement("CATEGORYTEXT", c)) //ma i seznam, ale je potreba pridat
                            ));

            XNamespace ns = "http://www.zbozi.cz/ns/offer/1.0";
            XDocument document = new XDocument(
                new XElement(ns + "SHOP",
                    productsExported
                )
            );
            document.Save(zboziFilePath);
        }

        private void FinishHeurekaFeedGeneration(IQueryable<ShopItem> products, string heurekaFilePath)
        {
            var productsExported = products.AsEnumerable().Select(p =>
                            new XElement("SHOPITEM",
                                new XElement("ITEM_ID", p.Id),
                                new XElement("PRODUCTNAME", p.Name),
                                new XElement("PRODUCT", p.Name),  //TODO: add manufacturer to name
                                new XElement("DESCRIPTION", p.Decription),
                                new XElement("URL", p.Url),
                                new XElement("IMGURL", p.ImgUrl),
                                p.AlternativePictures.Select(ap => new XElement("IMGURL_ALTERNATIVE", ap)),
                                new XElement("PRICE_VAT", String.Format("{0:0}", p.PriceWithVat)),
                                new XElement("ITEM_TYPE", "new"),
                                new XElement("MANUFACTURER", p.Manufacturer),
                                p.Categories.Select(c => new XElement("CATEGORYTEXT", c)),
                                new XElement("EAN", p.Ean),
                                new XElement("DELIVERY_DATE", p.OnStock ? "0" : null),
                                new XElement("PRODUCTNO", p.ManufacturerPartNumber) //heureca example has it but not in spec
                            ));

            XDocument document = new XDocument(
                new XElement("SHOP",
                    productsExported
                )
            );
            document.Save(heurekaFilePath);
        }

        public class LocalizationEntity
        {
            public int EntityId;
            public string Value;
        }

        private IQueryable<LocalizationEntity> GetLocalizedValue(int languageId, string localeKeyGroup, string localeKey)
        {
            return (from lp in _localizedPropertyRepository.Table
                    where lp.LocaleKeyGroup == localeKeyGroup &&
                          lp.LanguageId == languageId &&
                          lp.LocaleKey == localeKey
                    select new LocalizationEntity(){ EntityId = lp.EntityId, Value = lp.LocaleValue});
        }

        public class SlugEntity
        {
            public int EntityId;
            public string Slug;
        }

        private IQueryable<SlugEntity> GetSlug(int languageId, string entityName)
        {
            var getSlug = from ur in _urlRecordRepository.Table
                          where ur.EntityName == entityName &&
                          ur.LanguageId == languageId &&
                          ur.IsActive
                          select new SlugEntity() { EntityId = ur.EntityId, Slug = ur.Slug };

            return languageId == 0 ?
                getSlug :
                getSlug.Concat(GetSlug(0, entityName));
        }

        public class ManufacturerEntity
        {
            public int ProductId;
            public string ManufacturerName;
        }

        private IQueryable<ManufacturerEntity> GetManufacturers(int languageId)
        {
            var localizedManufacturerName = GetLocalizedValue(languageId, "Manufacturer", "Name");

            return  from pm in _productManufacturerRepository.Table
                    join m in _manufacturerRepository.Table
                    on pm.ManufacturerId equals m.Id
                    let lv = localizedManufacturerName.Where(l => l.EntityId == m.Id).Select(l => l.Value).DefaultIfEmpty(m.Name).FirstOrDefault()
                    where lv != "ostatní"
                    select new ManufacturerEntity() { ManufacturerName = lv, ProductId = pm.ProductId };
        }

        public class Cat
        {
            public int Id;
            public int ParentCategoryId;
            public string BreadCrumb;
            public bool Heureka;
            public bool Published;
        }

        private IQueryable<Cat> GetSortedCategories(int languageId)
        {
            var localizedCategoryName = GetLocalizedValue(languageId, "Category", "Name");
            var stromDatabaseItemsAll = _categoryRepository.Table
                .Where(i => !i.Deleted)
                .Select(c => c.Name == "Heureka.cz" ? new { DisplayOrder = 0, c.Id, c.Name, c.ParentCategoryId, Published = false, Heureka = true } : new { c.DisplayOrder, c.Id, c.Name, c.ParentCategoryId, Published = c.Published, Heureka = false })
                .OrderBy(i => i.DisplayOrder)
                .Select(c => new Cat()
                    {
                        Id = c.Id,
                        BreadCrumb = localizedCategoryName.Where(lv => lv.EntityId == c.Id).Select(l => l.Value).DefaultIfEmpty(c.Name).FirstOrDefault(),
                        ParentCategoryId = c.ParentCategoryId,
                        Heureka = c.Heureka,
                        Published = c.Published
                    });
            var stromDatabaseItemsL1 = stromDatabaseItemsAll.Where(i => i.ParentCategoryId == 0 && (i.Heureka || i.Published));
            var stromDatabaseItemsL2 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL1
                                       on i.ParentCategoryId equals l.Id
                                       where l.Heureka || i.Published
                                       select new Cat { Id =  i.Id, BreadCrumb = l.BreadCrumb + " | " + i.BreadCrumb, ParentCategoryId = i.ParentCategoryId, Heureka = l.Heureka, Published = i.Published};
            var stromDatabaseItemsL3 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL2
                                       on i.ParentCategoryId equals l.Id
                                       where l.Heureka || i.Published
                                       select new Cat { Id = i.Id, BreadCrumb = l.BreadCrumb + " | " + i.BreadCrumb, ParentCategoryId = i.ParentCategoryId, Heureka = l.Heureka, Published = i.Published };
            var stromDatabaseItemsL4 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL3
                                       on i.ParentCategoryId equals l.Id
                                       where l.Heureka || i.Published
                                       select new Cat { Id = i.Id, BreadCrumb = l.BreadCrumb + " | " + i.BreadCrumb, ParentCategoryId = i.ParentCategoryId, Heureka = l.Heureka, Published = i.Published };
            var stromDatabaseItemsL5 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL4
                                       on i.ParentCategoryId equals l.Id
                                       where l.Heureka || i.Published
                                       select new Cat { Id = i.Id, BreadCrumb = l.BreadCrumb + " | " + i.BreadCrumb, ParentCategoryId = i.ParentCategoryId, Heureka = l.Heureka, Published = i.Published };
            var stromDatabaseItemsL6 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL5
                                       on i.ParentCategoryId equals l.Id
                                       where l.Heureka || i.Published
                                       select new Cat { Id = i.Id, BreadCrumb = l.BreadCrumb + " | " + i.BreadCrumb, ParentCategoryId = i.ParentCategoryId, Heureka = l.Heureka, Published = i.Published };
            var stromDatabaseItemsL7 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL6
                                       on i.ParentCategoryId equals l.Id
                                       where l.Heureka || i.Published
                                       select new Cat { Id = i.Id, BreadCrumb = l.BreadCrumb + " | " + i.BreadCrumb, ParentCategoryId = i.ParentCategoryId, Heureka = l.Heureka, Published = i.Published };
            var stromDatabaseItemsL8 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL7
                                       on i.ParentCategoryId equals l.Id
                                       where l.Heureka || i.Published
                                       select new Cat { Id = i.Id, BreadCrumb = l.BreadCrumb + " | " + i.BreadCrumb, ParentCategoryId = i.ParentCategoryId, Heureka = l.Heureka, Published = i.Published };
            var stromDatabaseItems = stromDatabaseItemsL1.Concat(stromDatabaseItemsL2)
                .Concat(stromDatabaseItemsL3)
                .Concat(stromDatabaseItemsL4)
                .Concat(stromDatabaseItemsL5)
                .Concat(stromDatabaseItemsL6)
                .Concat(stromDatabaseItemsL7)
                .Concat(stromDatabaseItemsL8);
            return stromDatabaseItems;
        }

        public class PictureEntiry
        {
            public int ProductId;
            public string PictureUrl;
        }

        private IQueryable<PictureEntiry> GetPictureUrls(int languageId, string storeUrl)
        {
            return  from pp in _productPictureRepository.Table
                    join p in _pictureRepository.Table
                    on pp.PictureId equals p.Id
                    let pictureIdString = SqlFunctions.StringConvert((decimal)p.Id).Trim()
                    let mimeSubstring = p.MimeType.Substring(SqlFunctions.CharIndex("/", p.MimeType).Value)
                    select new PictureEntiry(){ ProductId = pp.ProductId,
                                                PictureUrl = storeUrl + 
                                                    "content/images/thumbs/" + 
                                                    SqlFunctions.Replicate("0", 7 - pictureIdString.Length) + 
                                                    pictureIdString + 
                                                    ((p.SeoFilename == null || p.SeoFilename == "") ? "" : "_" + p.SeoFilename) + 
                                                    "_300." +
                                                    (mimeSubstring == "pjpeg" ? "jpg" :
                                                        mimeSubstring == "x-png" ? "png" :
                                                        mimeSubstring == "x-icon" ? "ico" : mimeSubstring)
                    };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ZboziSettings
                {
                ProductPictureSize = 125,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.ClickHere", "Click here");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.Currency", "Currency");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.Currency.Hint", "Select the default currency that will be used to generate the feed.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.Generate", "Generate feed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.RegenerateUrls", "Regenerate URLs");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.ProductPictureSize", "Product thumbnail image size");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.ProductPictureSize.Hint", "The default size (pixels) for product thumbnail images.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.SuccessResult", "Zbozi feed has been successfully generated. {0} to see generated feed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.TaskEnabled", "Automatically generate a file");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.TaskEnabled.Hint", "Check if you want a file to be automatically generated.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.GenerateStaticFileEachMinutes", "A task period (minutes)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.GenerateStaticFileEachMinutes.Hint", "Specify a task period in minutes (generation of a new Zbozi file).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Zbozi.TaskRestart", "If a task settings ('Automatically generate a file') have been changed, please restart the application");

            //install a schedule task
            var task = FindStaticFileGenerationTask();
            if (task == null)
            {
                task = new ScheduleTask
                {
                    Name = "Zbozi static file generation",
                    //each 60 minutes
                    Seconds = 3600,
                    Type = "Nop.Plugin.Feed.Zbozi.StaticFileGenerationTask, Nop.Plugin.Feed.Zbozi",
                    Enabled = false,
                    StopOnError = false,
                };
                _scheduleTaskService.InsertTask(task);
            }

            task = FindPictureFilesGenerationTask();
            if (task == null)
            {
                task = new ScheduleTask
                {
                    Name = "Zbozi picture files generation",
                    //daily
                    Seconds = 86400,
                    Type = "Nop.Plugin.Feed.Zbozi.PictureFilesGenerationTask, Nop.Plugin.Feed.Zbozi",
                    Enabled = false,
                    StopOnError = false,
                };
                _scheduleTaskService.InsertTask(task);
            }

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.ClickHere");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.Currency");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.Currency.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.Generate");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.RegenerateUrls");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.ProductPictureSize");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.ProductPictureSize.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.SuccessResult");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.TaskEnabled");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.TaskEnabled.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.GenerateStaticFileEachMinutes");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.GenerateStaticFileEachMinutes.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Zbozi.TaskRestart");

            //Remove scheduled task
            var task = FindStaticFileGenerationTask();
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            task = FindPictureFilesGenerationTask();
            if (task != null)
                _scheduleTaskService.DeleteTask(task);
            
            base.Uninstall();
        }

        /// <summary>
        /// Generate a static file for zbozi
        /// </summary>
        public virtual void GenerateStaticFile()
        {
            foreach(var store in _storeService.GetAllStores().Where(s => s.Name != "Saskia.pro" && s.Name != "Saskia.ru"))
            {
                string zboziFileName = string.Format("Zbozi_{0}.xml", SeoExtensions.GetSeName(store.Name));
                string heurekaFileName = string.Format("Heureka_{0}.xml", SeoExtensions.GetSeName(store.Name));
                string zboziFilePath = string.Format("{0}content\\files\\exportimport\\{1}", HttpRuntime.AppDomainAppPath, zboziFileName);
                string heurekaFilePath = string.Format("{0}content\\files\\exportimport\\{1}", HttpRuntime.AppDomainAppPath, heurekaFileName);
                GenerateFeed(zboziFilePath, heurekaFilePath, store);
            }
        }

        public virtual void RegenerateUrls()
        {
            var products = _productService.SearchProducts(showHidden: true);
            var languages = _languageService.GetAllLanguages(true);
            foreach (var product in products)
            {
                _urlRecordService.SaveSlug(product, product.ValidateSeName("", product.Name, true), 0);
                foreach (var lang in languages)
                {
                    var name = product.GetLocalized(x => x.Name, lang.Id, false, false);
                    if(!String.IsNullOrEmpty(name) && name != product.Name)
                        _urlRecordService.SaveSlug(product, product.ValidateSeName("", name, true), lang.Id);
                }
            }

            var manufacturers = _manufacturerService.GetAllManufacturers(showHidden: true);
            foreach (var manufacturer in manufacturers)
            {
                _urlRecordService.SaveSlug(manufacturer, manufacturer.ValidateSeName("", manufacturer.Name, true), 0);
                foreach (var lang in languages)
                {
                    var name = manufacturer.GetLocalized(x => x.Name, lang.Id, false, false);
                    if (!String.IsNullOrEmpty(name) && name != manufacturer.Name)
                        _urlRecordService.SaveSlug(manufacturer, manufacturer.ValidateSeName("", name, true), lang.Id);
                }
            }

            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var category in categories)
            {
                _urlRecordService.SaveSlug(category, category.ValidateSeName("", category.Name, true), 0);
                foreach (var lang in languages)
                {
                    var name = category.GetLocalized(x => x.Name, lang.Id, false, false);
                    if (!String.IsNullOrEmpty(name) && name != category.Name)
                        _urlRecordService.SaveSlug(category, category.ValidateSeName("", name, true), lang.Id);
                }
            }
        }

        public void GenerateAllPicturesForFeeds()
        {
            var pictureIds =  
                    (from p in _pictureRepository.Table
                    join pp in _productPictureRepository.Table
                    on p.Id equals pp.PictureId
                    join prod in _productRepository.Table
                    on pp.ProductId equals prod.Id
                    where !prod.Deleted && prod.Published
                    orderby p.Id
                    select p.Id).Distinct().ToList();

            foreach (var id in pictureIds)
            {
                try
                {
                    _pictureService.GetPictureUrl(id, 300);
                }
                catch(Exception e)
                {
                    try
                    {
                        _logger.Error(e.Message, e);
                    }
                    catch(Exception)
                    {}
                }
            }
        }
        #endregion
    }
}
