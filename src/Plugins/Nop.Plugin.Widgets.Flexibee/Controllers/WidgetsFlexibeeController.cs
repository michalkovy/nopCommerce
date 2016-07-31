using System.Web.Mvc;
using Nop.Plugin.Widgets.Flexibee.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using Nop.Core.Domain.Orders;
using System;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Core;
using Nop.Core.Domain;
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Services.Localization;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Directory;
using Nop.Services.Tax;
using Nop.Core.Domain.Directory;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using System.Collections.Specialized;
using Nop.Core.Domain.Tax;
using System.Web;
using Nop.Web.Framework;
using MoreLinq;
using Nop.Services.Stores;
using Nop.Core.Data;
using System.Data.Entity.SqlServer;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.Flexibee.Controllers
{
    public class WidgetsFlexibeeController : Controller
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly FlexibeeSettings _FlexibeeSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly ILocalizationService _localizationService;

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;
        private readonly ITaxService _taxService;
        private readonly CurrencySettings _currencySettings;
        private readonly ICountryService _countryService;
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Category> _categoryRepository;

        CultureInfo usCulture = new CultureInfo("en-US");
        #endregion

        #region Ctor
        public WidgetsFlexibeeController(IWorkContext workContext, ISettingService settingService,
            IOrderService orderService,
            ILogger logger, IWebHelper webHelper,
            ILocalizationService localizationService,
            IProductService productService,
            ICategoryService categoryService, 
            IManufacturerService manufacturerService, IPictureService pictureService,
            ICurrencyService currencyService, ITaxService taxService,
            CurrencySettings currencySettings, ICountryService countryService,
            ILanguageService languageService,
            FlexibeeSettings FlexibeeSettings, StoreInformationSettings storeInformationSettings,
            IStoreContext storeContext, IStoreService storeService,
            IRepository<Product> productRepository, IRepository<ProductCategory> productCategoryRepository,
            IRepository<Category> categoryRepository)
        {
            this._workContext = workContext;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logger;
            this._webHelper = webHelper;
            this._FlexibeeSettings = FlexibeeSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._localizationService = localizationService;
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._taxService = taxService;
            this._currencySettings = currencySettings;
            this._countryService = countryService;
            this._languageService = languageService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._productRepository = productRepository;
            this._productCategoryRepository = productCategoryRepository;
            this._categoryRepository = categoryRepository;
        }
        #endregion

        #region Widget
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.FlexibeeExternalIdPrefix = _FlexibeeSettings.FlexibeeExternalIdPrefix;
            model.ZoneId = _FlexibeeSettings.WidgetZone;
            model.AvailableZones.Add(new SelectListItem() { Text = "<head> HTML tag", Value = "head_html_tag" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Before <body> end HTML tag", Value = "body_end_html_tag_before" });

            return View("~/Plugins/Widgets.Flexibee/Views/WidgetsFlexibee/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        [FormValueRequired("save")]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            _FlexibeeSettings.FlexibeeExternalIdPrefix = model.FlexibeeExternalIdPrefix;
            _FlexibeeSettings.WidgetZone = model.ZoneId;

            _settingService.SaveSetting(_FlexibeeSettings);

            return View("~/Plugins/Widgets.Flexibee/Views/WidgetsFlexibee/Configure.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            string globalScript = "";
            var routeData = ((System.Web.UI.Page)this.HttpContext.CurrentHandler).RouteData;

            /* Disabled because slow
            try
            {

                //Special case, if we are in last step of checkout, upload order to Flexibee
                if (routeData.Values["controller"].ToString().Equals("checkout", StringComparison.InvariantCultureIgnoreCase) &&
                    routeData.Values["action"].ToString().Equals("completed", StringComparison.InvariantCultureIgnoreCase))
                {
                    Order lastOrder = GetLastOrder();

                    var customer = lastOrder.Customer;
                    var flexibeeExternalIdPrefix = _FlexibeeSettings.FlexibeeExternalIdPrefix;
                    CustomerOfOrderExport(_orderService, flexibeeExternalIdPrefix, _countryService, lastOrder, customer, _taxService);
                    OrderExport(_orderService, flexibeeExternalIdPrefix, _countryService, lastOrder, customer, Request);
                }

            }
            catch (Exception ex)
            {
                _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error sending last order to Flexibee", ex.ToString());
            }*/

            return Content(globalScript);
        }

        private Order GetLastOrder()
        {
            return _orderService.SearchOrders(_storeContext.CurrentStore.Id, 0, _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
        }
        #endregion

        #region Upload Orders
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("uploadOrders")]
        public ActionResult UploadOrders(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            model.IntegrationResult = "";

            try
            {
                if (model.OrderId <= 0)
                {
                    var createdFromUtc = DateTime.UtcNow - System.TimeSpan.FromDays(30);
                    var createdToUtc = DateTime.UtcNow;
                    model.IntegrationResult = OrderExporter.OrdersExport(_orderService, model.FlexibeeExternalIdPrefix, _countryService, _taxService, true, createdFromUtc, createdToUtc, _storeService, _languageService);
                }
                else
                {
                    var order = _orderService.GetOrderById(model.OrderId);
                    if (order != null)
                    {
                        model.IntegrationResult = OrderExporter.OrderExport(order, _orderService, model.FlexibeeExternalIdPrefix, _countryService, _taxService, true, _storeService, _languageService);
                    }
                    else
                        model.IntegrationResult = "Objednávka neexistuje";
                }
            }
            catch (Exception exc)
            {
                model.IntegrationResult = exc.Message;
                _logger.Error(exc.Message, exc);
            }

            return View("~/Plugins/Widgets.Flexibee/Views/WidgetsFlexibee/Configure.cshtml", model);
        }
        #endregion

        #region Upload Products
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("uploadProducts")]
        public ActionResult UploadProducts(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            model.IntegrationResult = "";

            try
            {
                var languages = this._languageService.GetAllLanguages();
                var czechLanguageId = languages.First(l => l.LanguageCulture == "cs-CZ").Id;
                var englishLanguageId = languages.First(l => l.LanguageCulture == "en-US").Id;
                var deutschLanguageId = languages.First(l => l.LanguageCulture == "de-DE").Id;

                var products = StockExport(_productService, model, Request, czechLanguageId, englishLanguageId, deutschLanguageId);
                model.IntegrationResult += FlexibeeCommunicator.SendToFlexibee(products, "cenik");

                var strom = StromExport(_categoryRepository, model.FlexibeeExternalIdPrefix);
                model.IntegrationResult += FlexibeeCommunicator.SendToFlexibee(strom, "strom");

                var stromCenik = StromCenikExport(_categoryRepository, _productCategoryRepository, _productRepository, model.FlexibeeExternalIdPrefix);
                model.IntegrationResult += FlexibeeCommunicator.SendToFlexibee(stromCenik, "strom-cenik");
                
                string result = _localizationService.GetResource("Plugins.Widgets.Flexibee.SuccessResult");
                model.IntegrationResult += result;
            }
            catch (Exception exc)
            {
                model.IntegrationResult = exc.Message;
                _logger.Error(exc.Message, exc);
            }

            return View("~/Plugins/Widgets.Flexibee/Views/WidgetsFlexibee/Configure.cshtml", model);
        }

        public class Cat
        {
            public int Id;
            public string Name;
            public int ParentCategoryId;
            public int DisplayOrder;
        }

        public IQueryable<Cat> GetSortedCategories(IRepository<Category> categoryRepository)
        {
            var stromDatabaseItemsAll = categoryRepository.Table.Where(i => i.Published && !i.Deleted).Select(c => new Cat() { Id = c.Id, Name = c.Name, ParentCategoryId = c.ParentCategoryId, DisplayOrder = c.DisplayOrder }).OrderBy(i => i.DisplayOrder);
            var stromDatabaseItemsL1 = stromDatabaseItemsAll.Where(i => i.ParentCategoryId == 0);
            var stromDatabaseItemsL2 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL1
                                       on i.ParentCategoryId equals l.Id
                                       select i;
            var stromDatabaseItemsL3 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL2
                                       on i.ParentCategoryId equals l.Id
                                       select i;
            var stromDatabaseItemsL4 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL3
                                       on i.ParentCategoryId equals l.Id
                                       select i;
            var stromDatabaseItemsL5 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL4
                                       on i.ParentCategoryId equals l.Id
                                       select i;
            var stromDatabaseItemsL6 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL5
                                       on i.ParentCategoryId equals l.Id
                                       select i;
            var stromDatabaseItemsL7 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL6
                                       on i.ParentCategoryId equals l.Id
                                       select i;
            var stromDatabaseItemsL8 = from i in stromDatabaseItemsAll
                                       join l in stromDatabaseItemsL7
                                       on i.ParentCategoryId equals l.Id
                                       select i;
            var stromDatabaseItems = stromDatabaseItemsL1.Concat(stromDatabaseItemsL2)
                .Concat(stromDatabaseItemsL3)
                .Concat(stromDatabaseItemsL4)
                .Concat(stromDatabaseItemsL5)
                .Concat(stromDatabaseItemsL6)
                .Concat(stromDatabaseItemsL7)
                .Concat(stromDatabaseItemsL8);
            return stromDatabaseItems;
        }

        public Flexibee.Strom.Import.winstrom StromExport(IRepository<Category> categoryRepository, string flexibeeExternalPrefix)
        {
            var stromItemsOriginal = GetSortedCategories(categoryRepository);
            var stromItemList = new List<Strom.Import.winstromStrom>(stromItemsOriginal.Count());
            int lastOrder = 0;
            int lastParentCategoryId = 0;
            foreach (var stromItem in stromItemsOriginal)
            {
                if (lastParentCategoryId != stromItem.ParentCategoryId)
                {
                    lastOrder = 0;
                    lastParentCategoryId = stromItem.ParentCategoryId;
                }
                lastOrder = stromItem.DisplayOrder <= lastOrder ? lastOrder + 1 : stromItem.DisplayOrder + 1;
                var newItem = new Flexibee.Strom.Import.winstromStrom()
                {
                    id = new Strom.Import.IdType[] { new Strom.Import.IdType() { Value = String.Format("ext:{0}:STR{1}", flexibeeExternalPrefix, stromItem.Id) } },
                    nazev = new Strom.Import.NazevType() { Value = stromItem.Name },
                    strom = new Strom.Import.StromType() { Value = "code:STR_CEN" },
                    otec = new Strom.Import.OtecType()
                    {
                        Value =
                        stromItem.ParentCategoryId == 0 ?
                        "2" :
                        String.Format("ext:{0}:STR{1}", flexibeeExternalPrefix, stromItem.ParentCategoryId)
                    },
                    poradi = new Strom.Import.PoradiType { Value = lastOrder.ToString() }
                };
                
                stromItemList.Add(newItem);
            }

            return new Strom.Import.winstrom() { strom = stromItemList.ToArray() };
        }

        public Flexibee.StromCenik.Import.winstrom StromCenikExport(IRepository<Category> categoryRepository, IRepository<ProductCategory> productCategoryRepository, IRepository<Product> productRepository, string flexibeeExternalPrefix)
        {
            var stromCenikDatabaseItemsAll = productCategoryRepository.Table.Select(c => new 
            {
                c.Id,
                c.ProductId,
                c.CategoryId
            });

            var stromCenikDatabaseItems = from i in stromCenikDatabaseItemsAll
                                          join c in GetSortedCategories(categoryRepository)
                                          on i.CategoryId equals c.Id
                                          join p in productRepository.Table.Where(pr => pr.Published && !pr.Deleted && pr.ProductTypeId == 5)
                                          on i.ProductId equals p.Id
                                            select i;

            var stromCenikItems = stromCenikDatabaseItems.AsEnumerable().Select(c => new Flexibee.StromCenik.Import.winstromStromcenik()
            {
                id = new StromCenik.Import.IdType[] { new StromCenik.Import.IdType() { Value = String.Format("ext:{0}:STC{1}", flexibeeExternalPrefix, c.Id) } },
                idZaznamu = new StromCenik.Import.IdZaznamuType() { Value = String.Format("ext:{0}:P{1}", flexibeeExternalPrefix, c.ProductId)},
                uzel = new StromCenik.Import.UzelType() { Value = String.Format("ext:{0}:STR{1}", flexibeeExternalPrefix, c.CategoryId) }
            });
            return new StromCenik.Import.winstrom() { stromcenik = stromCenikItems.ToArray() };
        }

        public Flexibee.Cenik.Import.winstrom StockExport(IProductService productService, ConfigurationModel model, HttpRequestBase httpRequest, int czechLanguageId, int englishLanguageId, int deutschLanguageId)
        {
            //var last = DateTime.Now;

            var vysledek = new Flexibee.Cenik.Import.winstrom();
            //vysledek.last = last.ToString(CultureInfo.InvariantCulture);

            var productVariants = productService.SearchProducts(productType: ProductType.SimpleProduct);
            var items = new List<Flexibee.Cenik.Import.winstromCenik>();
            
            foreach (Product productVariant in productVariants)
            {
                var item = new Flexibee.Cenik.Import.winstromCenik
                {
                    id = new[] { new Flexibee.Cenik.Import.IdType() { Value = "ext:" + model.FlexibeeExternalIdPrefix + ":P" + productVariant.Id }, 
                        new Flexibee.Cenik.Import.IdType() { Value = "plu:" + productVariant.Sku }},
                    kod = new Flexibee.Cenik.Import.KodType { Value = String.IsNullOrWhiteSpace(productVariant.ManufacturerPartNumber) ? productVariant.Sku : productVariant.ManufacturerPartNumber },
                    eanKod = new Flexibee.Cenik.Import.EanKodType { Value = productVariant.Gtin },
                    kodPlu = new Flexibee.Cenik.Import.KodPluType { Value = productVariant.Sku },
                    nazev = new Flexibee.Cenik.Import.NazevType { Value = productVariant.GetLocalized(x => x.Name, czechLanguageId).Trim() },
                    nazevA = new Flexibee.Cenik.Import.NazevAType { Value = productVariant.GetLocalized(x => x.Name, englishLanguageId).Trim() },
                    nazevB = new Flexibee.Cenik.Import.NazevBType { Value = productVariant.GetLocalized(x => x.Name, deutschLanguageId).Trim() },
                    popis = new Flexibee.Cenik.Import.PopisType { Value = (productVariant.GetLocalized(x => x.ShortDescription, czechLanguageId) ?? String.Empty).Trim() },
                    popisA = new Flexibee.Cenik.Import.PopisAType { Value = (productVariant.GetLocalized(x => x.ShortDescription, englishLanguageId) ?? String.Empty).Trim() },
                    popisB = new Flexibee.Cenik.Import.PopisBType { Value = (productVariant.GetLocalized(x => x.ShortDescription, deutschLanguageId) ?? String.Empty).Trim() },
                    cenaZakl = new Flexibee.Cenik.Import.CenaZaklType { Value = productVariant.Price.ToString(CultureInfo.InvariantCulture) },
                    nakupCena = productVariant.ProductCost > 0 ?
                                    new Flexibee.Cenik.Import.NakupCenaType { Value = productVariant.ProductCost.ToString(CultureInfo.InvariantCulture) } :
                                    null,
                    typCenyDphK = new Flexibee.Cenik.Import.TypCenyDphKType { Value = "typCeny.bezDph" },
                    typSzbDphK = new Flexibee.Cenik.Import.TypSzbDphKType
                    {
                        Value = productVariant.TaxCategoryId == 7 ? "typSzbDph.dphZakl" :
                                    productVariant.TaxCategoryId == 8 ? "typSzbDph.dphSniz" : "typSzbDph.dphOsv"
                    },
                    typZasobyK = new Flexibee.Cenik.Import.TypZasobyKType { Value = productVariant.IsRental ? Cenik.Import.TypZasobyKTypeInner.typZasobysluzba : Cenik.Import.TypZasobyKTypeInner.typZasobyzbozi },
                    skupZboz = new Flexibee.Cenik.Import.SkupZbozType { Value = productVariant.IsRental ? "code:SLUŽBY" : "code:ZBOŽÍ" },
                    cenJednotka = new Flexibee.Cenik.Import.CenJednotkaType { Value = "1.0" },
                    mj1 = new Flexibee.Cenik.Import.Mj1Type { Value = "code:KS" },
                    skladove = new Cenik.Import.SkladoveType { Value = productVariant.IsRental ? "false" : "true" },
                    dodavatele = new Cenik.Import.winstromCenikDodavatele
                    {
                        dodavatel =
                            productVariant.ProductManufacturers.Select(productManufacturer => new KeyValuePair<ProductManufacturer,string>(productManufacturer, GetSupplierFlexibeeCode(productManufacturer.ManufacturerId))).Where(pair => !String.IsNullOrWhiteSpace(pair.Value)).DistinctBy(pair => pair.Value).Select(pair => new Flexibee.Cenik.Import.winstromCenikDodavateleDodavatel()
                            {
                                id = new[] { new Flexibee.Cenik.Import.DodavateleIdType { Value = "ext:" + model.FlexibeeExternalIdPrefix + ":DOD" + productVariant.Id + "M" + pair.Key.ManufacturerId } },
                                firma = new Flexibee.Cenik.Import.DodavateleFirmaType() { Value = "code:" + pair.Value.Trim() },
                                nakupCena = productVariant.ProductCost > 0 ? new Flexibee.Cenik.Import.DodavateleNakupCenaType() { Value = productVariant.ProductCost.ToString(CultureInfo.InvariantCulture) } : null,
                                kodIndi = new Flexibee.Cenik.Import.DodavateleKodIndiType { Value = productVariant.ManufacturerPartNumber },
                                mena = new string[] { GetCurrencyCode(pair.Key.ManufacturerId) }.Where(currency => !String.IsNullOrWhiteSpace(currency)).Select(currency => new Flexibee.Cenik.Import.DodavateleMenaType { Value = "code:" + currency }).FirstOrDefault()
                            }).ToArray()
                    },
                    vyrobce = new Flexibee.Cenik.Import.VyrobceType { Value = productVariant.ProductManufacturers.Select(productManufacturer => GetManufacturerFlexibeeCode(productManufacturer.ManufacturerId)).Where(code => !String.IsNullOrWhiteSpace(code)).Select(code => "code:" + code.Trim()).FirstOrDefault() ?? String.Empty }
                };
                if (item.dodavatele.dodavatel.Length == 1)
                {
                    item.dodavatele.dodavatel[0].primarni = new Cenik.Import.DodavatelePrimarniType { Value = "true" };
                }

                items.Add(item);
            }
            vysledek.cenik = items.ToArray();
            vysledek.version = Flexibee.Cenik.Import.winstromVersion.Item10;
            return vysledek;
        }
        #endregion

        #region download stock
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("downloadStock")]
        public ActionResult DownloadStock(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            try
            {
                UpdateSklad(model.FlexibeeExternalIdPrefix, _productRepository, _productService);
                string result = _localizationService.GetResource("Plugins.Widgets.Flexibee.SuccessResult");
                model.IntegrationResult = result;
            }
            catch (Exception exc)
            {
                model.IntegrationResult = exc.Message;
                _logger.Error(exc.Message, exc);
            }

            return View("~/Plugins/Widgets.Flexibee/Views/WidgetsFlexibee/Configure.cshtml", model);
        }

        public static void UpdateSklad(string flexibeeExternalIdPrefix, IRepository<Product> productRepository, IProductService productService)
        {
            string extIdPrefix = String.Format("ext:{0}:P", flexibeeExternalIdPrefix);
            var ceniky = FlexibeeCommunicator.ReceiveFromFlexibee<Flexibee.Cenik.Export.winstrom>("cenik.xml?detail=custom:sumDostupMj&limit=0");
            var flexibeeStavy = (from c in ceniky.cenik
                        select new
                        {
                            Id = c.id.Where(e => e.Value.StartsWith(extIdPrefix)).Select(e => Int32.Parse(e.Value.Replace(extIdPrefix, ""))).FirstOrDefault(),
                            Stav = (int)Decimal.Parse(c.sumDostupMj.Value, NumberStyles.Any, CultureInfo.InvariantCulture)
                        }).Where(d => d.Id != 0);

            Dictionary<int, int> currentQuantities = productRepository.Table.Select(p => new { p.Id, p.StockQuantity }).ToDictionary(p => p.Id, p => p.StockQuantity);

            foreach (var flexibeeStav in flexibeeStavy)
            {
                if (currentQuantities[flexibeeStav.Id] != flexibeeStav.Stav)
                {
                    UpdateSkladStav(flexibeeStav.Id, flexibeeStav.Stav, productService);
                }
            }
        }

        private static void UpdateSkladStav(int webId, int stavMj, IProductService productService)
        {
            var productVariant = productService.GetProductById(webId);
            if (productVariant == null) return;
            productVariant.StockQuantity = stavMj;
            productService.UpdateProduct(productVariant);
        }
        #endregion

        #region ManufacturerGrid
        [HttpPost]
        public ActionResult ManufacturerList(DataSourceRequest command)
        {
            var manufacturerModelList = _manufacturerService.GetAllManufacturers(showHidden: true).Select(manufacturer =>
                new ManufacturerModel()
                {
                    ManufacturerId = manufacturer.Id,
                    ManufacturerName = manufacturer.Name,
                    ManufacturerFlexibeeCode = GetManufacturerFlexibeeCode(manufacturer.Id),
                    SupplierFlexibeeCode = GetSupplierFlexibeeCode(manufacturer.Id),
                    CurrencyCode = GetCurrencyCode(manufacturer.Id)
                }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = manufacturerModelList,
                Total = manufacturerModelList.Count
            };

            return new JsonResult
            {
                Data = gridModel
            };
        }


        [HttpPost]
        public ActionResult ManufacturerUpdate(ManufacturerModel model)
        {
            _settingService.SetSetting(string.Format("Widgets.Flexibee.ManufacturerId{0}.ManufacturerFlexibeeCode", model.ManufacturerId), model.ManufacturerFlexibeeCode);

            _settingService.SetSetting(string.Format("Widgets.Flexibee.ManufacturerId{0}.SupplierFlexibeeCode", model.ManufacturerId), model.SupplierFlexibeeCode);

            _settingService.SetSetting(string.Format("Widgets.Flexibee.ManufacturerId{0}.CurrencyCode", model.ManufacturerId), model.CurrencyCode);

            return new NullJsonResult();
        }

        [NonAction]
        protected string GetManufacturerFlexibeeCode(int manufacturerId)
        {
            return this._settingService.GetSettingByKey<string>(string.Format("Widgets.Flexibee.ManufacturerId{0}.ManufacturerFlexibeeCode", manufacturerId));
        }

        [NonAction]
        protected string GetSupplierFlexibeeCode(int manufacturerId)
        {
            return this._settingService.GetSettingByKey<string>(string.Format("Widgets.Flexibee.ManufacturerId{0}.SupplierFlexibeeCode", manufacturerId));
        }

        [NonAction]
        protected string GetCurrencyCode(int manufacturerId)
        {
            return this._settingService.GetSettingByKey<string>(string.Format("Widgets.Flexibee.ManufacturerId{0}.CurrencyCode", manufacturerId));
        }
        #endregion

        #region Update Orders
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("updateStoreOrder")]
        public ActionResult UpdateStoreOrder(ConfigurationModel model)
        {
            var order = _orderService.GetOrderById(model.OrderId);
            if (order != null)
            {
                model.IntegrationResult = OrderImporter.UpdateOrder(order, model.FlexibeeExternalIdPrefix, _productRepository, _orderService);
            }
            else
            {
                model.IntegrationResult = "Objednavka neexistuje";
            }
            return View("~/Plugins/Widgets.Flexibee/Views/WidgetsFlexibee/Configure.cshtml", model);
        }
        #endregion
    }
}