using Nop.Plugin.Misc.WebImporter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Nop.Core.Domain.Catalog;
using System.Text.RegularExpressions;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.WebImporter
{
    public class PageLoader
    {
        private XPathExpression nextPageExpression;
        private XPathExpression productDetailsExpression;
        private XPathExpression availableProductDetailsExpression;

        private XPathExpression productNameExpression;
        private XPathExpression productSkuExpression;
        private XPathExpression shortDescriptionExpression;
        private XPathExpression priceExpression;
        private XPathExpression priceOrigExpression;
        private XPathExpression priceCostExpression;
        private XPathExpression keywordsExpression;

        private XPathExpression firstImageExpression;
        private XPathExpression otherImagesExpression;

        private readonly ILocalizedEntityService _localizedEntityService;
        private XPathExpression manufacturerExpression;

        private XPathExpression weightExpression;
        private WebImporterSite configuration;

        public PageLoader(WebImporterSite configuration, ILocalizedEntityService localizedEntityService = null)
        {
            //Create argument list and add the parameters.
            XsltArgumentList varList = new XsltArgumentList();

            // Create an instance of custom XsltContext object.  
            // Pass in the XsltArgumentList object  
            // in which the user-defined variable will be defined.                    
            CustomContext context = new CustomContext(new NameTable(), varList);

            // Add a namespace definition for the namespace prefix that qualifies the 
            // user-defined function name in the query expression.
            context.AddNamespace("Extensions", "http://xpathExtensions");

            if (!String.IsNullOrWhiteSpace(configuration.NextPageExpression))
            {
                nextPageExpression = XPathExpression.Compile(configuration.NextPageExpression);
                nextPageExpression.SetContext(context);
            }
            productDetailsExpression = XPathExpression.Compile(configuration.ProductDetailsExpression);
            productDetailsExpression.SetContext(context);
            if (!String.IsNullOrWhiteSpace(configuration.AvailableProductDetailsExpression))
            {
                availableProductDetailsExpression = XPathExpression.Compile(configuration.AvailableProductDetailsExpression);
                availableProductDetailsExpression.SetContext(context);
            }
            productNameExpression = XPathExpression.Compile(configuration.ProductNameExpression);
            productNameExpression.SetContext(context);
            productSkuExpression = XPathExpression.Compile(configuration.ProductSkuExpression);
            productSkuExpression.SetContext(context);
            if (!String.IsNullOrWhiteSpace(configuration.ShortDescriptionExpression))
            {
                shortDescriptionExpression = XPathExpression.Compile(configuration.ShortDescriptionExpression);
                shortDescriptionExpression.SetContext(context);
            }
            else
                shortDescriptionExpression = null;
            if (!String.IsNullOrWhiteSpace(configuration.PriceExpression))
            {
                priceExpression = XPathExpression.Compile(configuration.PriceExpression);
                priceExpression.SetContext(context);
            }
            if (!String.IsNullOrWhiteSpace(configuration.PriceOrigExpression))
            {
                priceOrigExpression = XPathExpression.Compile(configuration.PriceOrigExpression);
                priceOrigExpression.SetContext(context);
            }
            if (!String.IsNullOrWhiteSpace(configuration.PriceCostExpression))
            {
                priceCostExpression = XPathExpression.Compile(configuration.PriceCostExpression);
                priceCostExpression.SetContext(context);
            }
            if (!String.IsNullOrWhiteSpace(configuration.KeywordsExpression))
            {
                keywordsExpression = XPathExpression.Compile(configuration.KeywordsExpression);
                keywordsExpression.SetContext(context);
            }
            else
                keywordsExpression = null;
            firstImageExpression = XPathExpression.Compile(configuration.FirstImageExpression);
            firstImageExpression.SetContext(context);
            otherImagesExpression = XPathExpression.Compile(configuration.OtherImagesExpression);
            otherImagesExpression.SetContext(context);
            if (!String.IsNullOrWhiteSpace(configuration.ManufacturerExpression))
            {
                manufacturerExpression = XPathExpression.Compile(configuration.ManufacturerExpression);
                manufacturerExpression.SetContext(context);
            }
            if (!String.IsNullOrWhiteSpace(configuration.WeightExpression))
            {
                weightExpression = XPathExpression.Compile(configuration.WeightExpression);
                weightExpression.SetContext(context);
            }
            _localizedEntityService = localizedEntityService;
            this.configuration = configuration;
        }

        public Product LoadProduct(PageParser productPage, Product product = null)
        {
            if (product == null)
            {
                product = new Product();
                InitializeProduct(product);
            }
            product.Sku = GetProductSku(productPage);
            product.ManufacturerPartNumber = product.Sku;
            product.Name = productPage.GetClearedString(productNameExpression);
            string innerFullText;
            product.FullDescription = ClearFullDescription(productPage.GetHtmlAndInnerTextFromNodes(configuration.FullDescriptionExpression, out innerFullText));
            if (shortDescriptionExpression == null)
            {
                product.ShortDescription = ConstructShortDescriptionFromFull(innerFullText);
            }
            else
            {
                product.ShortDescription = productPage.GetClearedString(shortDescriptionExpression);
            }
            product.ShortDescription = ClearDescription(product.ShortDescription);
            var oldPrice = product.Price;
            if (priceExpression != null)
            {
                var priceFromPage = productPage.GetDecimal(priceExpression);
                if (priceFromPage != Decimal.MinValue
                    && priceFromPage != Decimal.MaxValue
                    && priceFromPage >= 0M)
                    product.Price = priceFromPage * configuration.EndPriceWithouTaxMultiplication;
            }
            if (priceOrigExpression != null)
            {
                var originalPrice = productPage.GetDecimal(priceOrigExpression);
                if (originalPrice >= 0)
                    product.OldPrice = originalPrice * configuration.EndPriceWithouTaxMultiplication;
            }
            if (product.Price < (oldPrice * 0.99M) && oldPrice > product.OldPrice)
            {
                product.OldPrice = oldPrice;
            }
            if (product.OldPrice <= product.Price)
            {
                product.OldPrice = 0M;
            }
            if (priceCostExpression != null)
            {
                product.ProductCost = productPage.GetDecimal(priceCostExpression);
            }
            product.MetaKeywords = keywordsExpression == null ? product.MetaKeywords : (productPage.GetString(keywordsExpression) ?? "");
            if (weightExpression != null)
            {
                var newWeight = productPage.GetDecimal(weightExpression) * configuration.EndWeightMultiplication;
                if (newWeight >= 0M)
                    product.Weight = newWeight;
            }
            product.VendorId = configuration.VendorId;
            product.WarehouseId = configuration.WarehouseId;

            if (configuration.LanguageId.HasValue && _localizedEntityService != null)
            {
                _localizedEntityService.SaveLocalizedValue(product, x => x.Name, product.Name, configuration.LanguageId.Value);
                _localizedEntityService.SaveLocalizedValue(product, x => x.ShortDescription, product.ShortDescription, configuration.LanguageId.Value);
                _localizedEntityService.SaveLocalizedValue(product, x => x.FullDescription, product.FullDescription, configuration.LanguageId.Value);
            }
            product.UpdatedOnUtc = DateTime.UtcNow;

            return product;
        }

        private void InitializeProduct(Product product)
        {
            product.CreatedOnUtc = DateTime.UtcNow;
            product.AllowCustomerReviews = true;
            product.Published = true;
            product.ProductTemplateId = 2;
            product.LimitedToStores = true;
            product.ProductTypeId = 5;
            product.IsShipEnabled = true;
            product.TaxCategoryId = 7;
            product.ManageInventoryMethodId = 1;
            product.DisplayStockAvailability = true;
            product.DisplayStockQuantity = false;
            product.NotifyAdminForQuantityBelow = 1;
            product.BackorderModeId = 2;
            product.AllowBackInStockSubscriptions = true;
            product.OrderMinimumQuantity = 1;
            product.OrderMaximumQuantity = 10000;
            product.MaximumCustomerEnteredPrice = 1M;
            product.VisibleIndividually = true;
            product.Weight = 1M;
        }

        public string GetProductSku(PageParser productPage)
        {
            var sku = productPage.GetClearedString(productSkuExpression);
            return sku.Length > 20 ? sku.Substring(0, 20) : sku;
        }

        public string GetManufacturerName(PageParser productPage)
        {
            return manufacturerExpression == null ? configuration.ManufacturerName : 
                productPage.GetClearedString(manufacturerExpression);
        }

        public KeyValuePair<string, Uri> GetCategoryLink(PageParser productPage)
        {
            return productPage.GetLink(configuration.CategoryUrlExpression);
        }

        public List<Uri> GetPictureUris(PageParser productPage)
        {
            var result = new List<Uri>();
            Uri firstImage = productPage.GetUri(firstImageExpression);
            if (firstImage != null)
                result.Add(firstImage);

            var otherImages = productPage.GetUris(otherImagesExpression);
            result.AddRange(otherImages);
            return result;
        }

        private string ConstructShortDescriptionFromFull(string fullDescription)
        {
            fullDescription = Regex.Replace(fullDescription, @"\s+", " ");
            fullDescription = Regex.Replace(fullDescription, @"Další odk?az ZDE\.? ?-?>?>?", "");
            return fullDescription.Split(new[] { ". " }, StringSplitOptions.None)[0].Trim() + ".";
        }

        private string ClearDescription(string description)
        {
            if (!String.IsNullOrEmpty(configuration.ManufacturerName))
            {
                description = Regex.Replace(description, @" our ", String.Format(" {0} ", configuration.ManufacturerName));
                description = Regex.Replace(description, @"Our ", String.Format("{0} ", configuration.ManufacturerName));
            }
            return description;
        }

        private string ClearFullDescription(string original)
        {
            return ClearDescription(Regex.Replace(original, @"(<a[^<]*)<a", "$1</a"));
        }

        public Dictionary<string, Uri> GetCategoryLinks(PageParser productPage)
        {
            return productPage.GetLinks(configuration.CategoryUrlsExpression);
        }

        public List<Uri> GetProductDetailsUris(PageParser productPage)
        {
            return productPage.GetUris(productDetailsExpression);
        }

        public Uri GetNextPageUri(PageParser productPage)
        {
            return nextPageExpression == null ? null : productPage.GetUri(nextPageExpression);
        }

        public List<Uri> GetStockAvailableProductDetailsUris(PageParser productPage)
        {
            return productPage.GetUris(availableProductDetailsExpression);
        }
    }
}
