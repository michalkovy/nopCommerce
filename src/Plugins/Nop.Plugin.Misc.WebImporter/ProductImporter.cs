using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.WebImporter.Domain;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nop.Services.Seo;
using ImageResizer;
using System.IO;

namespace Nop.Plugin.Misc.WebImporter
{
    public class ProductImporter
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ILogger _logger;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly WebImporterSite _configuration;
        private readonly PageLoader _loader;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private Dictionary<string, WebImporterLink> _webImporterLinks;

        public ProductImporter(IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPictureService pictureService, 
            ILogger logger,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            WebImporterSite configuration,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            Dictionary<string, WebImporterLink> webImporterLinks)
        {
            _productService = productService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _pictureService = pictureService;
            _logger = logger;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _configuration = configuration;
            _loader = new PageLoader(_configuration, _localizedEntityService);
            _storeService = storeService;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _webImporterLinks = webImporterLinks;
        }

        public WebClient GetClient()
        {
            return Configurations.GetClient(_configuration);
        }

        public void ImportOneProduct(Uri productUrl, string categoryUrl=null, bool updatePictures = true)
        {
            var parser = PageParser.CreatePageParser(GetClient(), productUrl);
            var manufacturerName = _loader.GetManufacturerName(parser);

            if (_configuration.SkipManufacturerNames != null && !String.IsNullOrWhiteSpace(manufacturerName) && _configuration.SkipManufacturerNames.Contains(manufacturerName))
                return;

            var sku = _loader.GetProductSku(parser);
            var correspondingProduct = _productService.GetProductBySku(sku);
            bool update = correspondingProduct != null;

            var updatedProduct = _loader.LoadProduct(parser, correspondingProduct);
            UriSkuCache[productUrl.AbsoluteUri] = updatedProduct.Sku;
            updatedProduct.FullDescription = ReplaceUrls(updatedProduct.FullDescription, parser);
            
            if (update)
            {
                _productService.UpdateProduct(updatedProduct);
            }
            else
            {
                _productService.InsertProduct(updatedProduct);
                //search engine name
                var seName = updatedProduct.ValidateSeName("", updatedProduct.Name, true);
                _urlRecordService.SaveSlug(updatedProduct, seName, 0);
            }

            SaveStoreMappings(updatedProduct, _configuration.StoreIds);

            if (updatePictures || !update)
            {
                var pictures = _loader.GetPictureUris(parser);
                InsertNewPictures(updatedProduct.Id, pictures);
            }

            var categoryLink = _loader.GetCategoryLink(parser).Value.AbsoluteUri;
            InsertNewCategory(updatedProduct, categoryLink);
            if (categoryUrl != null)
            {
                if (!AreCategoriesSame(categoryLink, categoryUrl))
                    InsertNewCategory(updatedProduct, categoryUrl);
            }
            InsertNewManufacturer(updatedProduct, manufacturerName, categoryLink);
        }


        private void SaveStoreMappings(Product product, int[] storeIds)
        {
            product.LimitedToStores = true;
            var existingStoreMappings = _storeMappingService.GetStoreMappings(product);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (storeIds.Contains(store.Id))
                {
                    //new role
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(product, store.Id);
                }
                else
                {
                    //removed role
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        private void InsertNewPictures(int productId, List<Uri> webPictures)
        {
            var currentPictures = _pictureService.GetPicturesByProductId(productId);
            int position = 11;
            foreach (Uri picture in webPictures)
            {
                try
                {
                    WebClient client = GetClient();
                    byte[] downloadedPicture;
                    try
                    {
                        downloadedPicture = client.DownloadData(picture);
                    }
                    catch (WebException e)
                    {
                        _logger.Warning(String.Format("Following picture url failed first time: \"{0}\"", picture.AbsoluteUri), e);
                        //try twice
                        downloadedPicture = client.DownloadData(picture);
                    }
                    var mimeType = client.ResponseHeaders[HttpResponseHeader.ContentType];
                    var samePictures = currentPictures.Where(current => current.PictureBinary.SequenceEqual(downloadedPicture));
                    var modifiedDownloadedPicture = ValidateAndRecreatePicture(downloadedPicture);
                    var equalPictures = samePictures.Concat(currentPictures.Where(current => current.PictureBinary.SequenceEqual(modifiedDownloadedPicture)));
                    if (equalPictures.Count() >= 1)
                    {
                        if (equalPictures.Count() > 1)
                        {
                            var picturesToRemove = equalPictures.Skip(1);
                            foreach (var pictureToRemove in picturesToRemove)
                            {
                                /*var productPicturesToRemove = pictureToRemove.ProductPictures;
                                foreach(var productPictureToRemove in productPicturesToRemove)
                                {
                                    _productService.DeleteProductPicture(productPictureToRemove);
                                }*/
                            
                                _pictureService.DeletePicture(pictureToRemove);
                            }
                        }
                        continue;
                    }

                    var pictureName = Regex.Replace(picture.Segments.Last(), @"\.[^\.]*$", "");
                    var newPicture = _pictureService.InsertPicture(downloadedPicture, mimeType, _pictureService.GetPictureSeName(pictureName), isNew: true, validateBinary: false);
                    _productService.InsertProductPicture(new ProductPicture()
                    {
                        ProductId = productId,
                        PictureId = newPicture.Id,
                        DisplayOrder = position
                    });
                    position += 10;
                }
                catch (WebException e)
                {
                    _logger.Warning(String.Format("Following picture url failed: \"{0}\"", picture.AbsoluteUri), e);
                }
            }
        }

        protected virtual byte[] ValidateAndRecreatePicture(byte[] pictureBinary)
        {
            var destStream = new MemoryStream();
            ImageBuilder.Current.Build(pictureBinary, destStream, new ResizeSettings()
            {
                MaxWidth = 1250,
                MaxHeight = 1250,
                Quality = 100
            });
            return destStream.ToArray();
        }

        private void InsertNewCategory(Product product, string categoryUrl)
        {
            if (_webImporterLinks.ContainsKey(categoryUrl))
            {
                var categoryId = _webImporterLinks[categoryUrl].CategoryId;
                if (!product.ProductCategories.Any(productCategory => productCategory.CategoryId == categoryId))
                {
                    var productCategory = new ProductCategory()
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        IsFeaturedProduct = false,
                        DisplayOrder = 1
                    };
                    _categoryService.InsertProductCategory(productCategory);
                }
            }
            else
            {
                _logger.Warning(String.Format("Following category URL isn't mapped to category: \"{0}\"", categoryUrl));
            }
        }

        private bool AreCategoriesSame(string url1, string url2)
        {
            return _webImporterLinks.ContainsKey(url1) && _webImporterLinks.ContainsKey(url2) &&
                _webImporterLinks[url1].CategoryId == _webImporterLinks[url2].CategoryId;
        }

        private void InsertNewManufacturer(Product product, string manufacturerName, string categoryUrl)
        {
            int? manufacturerId = null;
            if(!String.IsNullOrWhiteSpace(manufacturerName))
            {
                var manufacturers = _manufacturerService.GetAllManufacturers(manufacturerName: manufacturerName, showHidden: true);
                if (manufacturers != null && manufacturers.Count > 0)
                {
                    manufacturerId = manufacturers[0].Id;
                }
                else
                {
                    _logger.Warning(String.Format("Following manufacturer name isn't known: \"{0}\"", manufacturerName));
                }
            }
            else if (_webImporterLinks.ContainsKey(categoryUrl) && _webImporterLinks[categoryUrl].ManufacturerId != 0)
            {
                manufacturerId = _webImporterLinks[categoryUrl].ManufacturerId;
            }
            else
            {
                _logger.Warning(String.Format("Following category URL isn't mapped to manufacturer: \"{0}\"", categoryUrl));
            }

            if (manufacturerId.HasValue)
            {
                if (!product.ProductManufacturers.Any(productManufacturer => productManufacturer.ManufacturerId == manufacturerId))
                {
                    var productManufacturer = new ProductManufacturer()
                    {
                        ProductId = product.Id,
                        ManufacturerId = manufacturerId.Value,
                        DisplayOrder = 1,
                        IsFeaturedProduct = false
                    };
                    _manufacturerService.InsertProductManufacturer(productManufacturer);
                }
            }
        } 

        private string ReplaceUrls(string html, PageParser originalPage)
        {
            var matches = Regex.Matches(html, @"href=""([^""]*)""");
            foreach (Match match in matches)
            {
                bool replaced = false;
                var url = match.Groups[1].Value;
                var absoluteUri = originalPage.GetAbsoluteUri(url);
                var origivalUri = originalPage.GetAbsoluteUri("");

                if (url.StartsWith("#"))
                    continue;

                if (_webImporterLinks.ContainsKey(absoluteUri.AbsoluteUri))
                {
                    var categoryId = _webImporterLinks[absoluteUri.AbsoluteUri].CategoryId;
                    var productUrl = string.Format("/{0}", _categoryService.GetCategoryById(categoryId).GetSeName(2));
                    html = html.Replace(url, productUrl);
                    replaced = true;
                }
                else if (origivalUri.Host == absoluteUri.Host)
                {

                    try
                    {
                        var correspondingProduct = GetProductFromUri(absoluteUri);
                        if (correspondingProduct != null)
                        {
                            var productUrl = string.Format("/{0}", correspondingProduct.GetSeName(2));
                            html = html.Replace(url, productUrl);
                            replaced = true;
                        }
                    }
                    catch (WebException e)
                    {
                        _logger.Information(String.Format("Following url doesn't match a product: \"{0}\"", absoluteUri.AbsoluteUri), e);
                    }

                    if(!replaced)
                    {
                        //just make sure absolute Uri is present
                        html = html.Replace(url, absoluteUri.AbsoluteUri);
                    }
                }
            }

            matches = Regex.Matches(html, @"src=""([^""]*)""");
            foreach (Match match in matches)
            {
                var url = match.Groups[1].Value;
                var absoluteUri = originalPage.GetAbsoluteUri(url);
                html = html.Replace(url, absoluteUri.AbsoluteUri);
            }
            return html;
        }

        private static Dictionary<string, string> UriSkuCache = new Dictionary<string, string>();
        
        private Product GetProductFromUri(Uri uri)
        {
            if (uri == null)
                return null;

            string sku = null;
            if (!UriSkuCache.TryGetValue(uri.AbsoluteUri, out sku))
            {
                var parser = PageParser.CreatePageParser(GetClient(), uri);
                sku = _loader.GetProductSku(parser);
                if (!String.IsNullOrWhiteSpace(sku))
                {
                    UriSkuCache[uri.AbsoluteUri] = sku;
                }
                else
                {
                    sku = null;
                }
            }

            return sku != null ? _productService.GetProductBySku(sku) : null;
        }

        public void TraverseSite(bool updatePictures = true)
        {
            var parser = PageParser.CreatePageParser(GetClient(), new Uri(_configuration.BaseUrl));
            var categoryLinks = _loader.GetCategoryLinks(parser);
            foreach (var link in categoryLinks)
            {
                TraverseCategory(link.Value, link.Key, updatePictures);
            }
        }

        public void TraverseCategory(Uri startPage, string categoryName, bool updatePictures = true, string topCategoryUrl = null)
        {
            if (topCategoryUrl == null)
                topCategoryUrl = startPage.AbsoluteUri;
            var parser = PageParser.CreatePageParser(GetClient(), startPage);
            var productDetailUris = _loader.GetProductDetailsUris(parser);
            foreach (var productUrl in productDetailUris)
            {
                try
                {
                    ImportOneProduct(productUrl, topCategoryUrl, updatePictures);
                }
                catch (Exception e)
                {
                    _logger.Warning(String.Format("Following product URL failed: \"{0}\"", productUrl.AbsoluteUri), e);
                }
            }
            var nextPageUri = _loader.GetNextPageUri(parser);
            if (nextPageUri != null)
            {
                TraverseCategory(nextPageUri, categoryName, updatePictures, topCategoryUrl);
            }
        }

        public void TraverseSiteForStockAvailability()
        {
            var parser = PageParser.CreatePageParser(GetClient(), new Uri(_configuration.BaseUrl));
            var categoryLinks = _loader.GetCategoryLinks(parser);
            foreach (var link in categoryLinks)
            {
                TraverseCategoryForStockAvailability(link.Value);
            }
        }

        public void TraverseCategoryForStockAvailability(Uri startPage, string topCategoryUrl = null)
        {
            if (topCategoryUrl == null)
                topCategoryUrl = startPage.AbsoluteUri;
            var parser = PageParser.CreatePageParser(GetClient(), startPage);
            var productDetailUris = _loader.GetStockAvailableProductDetailsUris(parser);
            foreach (var productUrl in productDetailUris)
            {
                try
                {
                    var parser2 = PageParser.CreatePageParser(GetClient(), productUrl);
                    var sku = _loader.GetProductSku(parser2);
                    var correspondingProduct = _productService.GetProductBySku(sku);
                    if (correspondingProduct != null && correspondingProduct.StockQuantity < 1)
                    {
                        correspondingProduct.StockQuantity = 1;
                        _productService.UpdateProduct(correspondingProduct);
                    }
                }
                catch (Exception e)
                {
                    _logger.Warning(String.Format("Following product URL failed: \"{0}\"", productUrl.AbsoluteUri), e);
                }
            }
            var nextPageUri = _loader.GetNextPageUri(parser);
            if (nextPageUri != null)
            {
                TraverseCategoryForStockAvailability(nextPageUri, topCategoryUrl);
            }
        }
    }
}
