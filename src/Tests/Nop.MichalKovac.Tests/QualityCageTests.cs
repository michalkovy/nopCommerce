using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nop.Plugin.Misc.WebImporter;
using System.Collections.Generic;

namespace Nop.MichalKovac.Tests
{
    [TestClass]
    public class QualityCageTests
    {
        [TestMethod]
        public void ChinchillaMansion()
        {
            KeyValuePair<string, Uri> category;
            var product = ImportQualityCagesProduct(@"http://qualitycage.com/index.php?main_page=product_info&cPath=129_142_153&products_id=612", out category);
            Assert.AreEqual("Chinchilla Mansion™, Galvanized w/ Powder Coated Tray", product.Name);
            Assert.AreEqual(226.75M* Configurations.GetConfiguration(SupportedSite.QualityCages).EndPriceWithouTaxMultiplication, product.Price);
            Assert.AreEqual("CNS-48", product.Sku);
            Assert.AreEqual("Quality Cage Company Chinchilla Mansion™, Galvanized w/ Powder Coated Tray [CNS-", product.ShortDescription.Substring(0, 80));
            Assert.AreEqual(@"http://qualitycage.com/index.php?main_page=index&cPath=129_142_153", category.Value.AbsoluteUri);
        }

        [TestMethod]
        public void ChinchillaCages()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.QualityCages);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, new Uri(@"http://qualitycage.com/index.php?main_page=index&cPath=129_142_153"));

            var productLinks = loader.GetProductDetailsUris(parser);
            Assert.AreEqual(10, productLinks.Count);

            var nextPage = loader.GetNextPageUri(parser);
            Assert.IsTrue(nextPage.AbsoluteUri.StartsWith(@"http://qualitycage.com/index.php?main_page=index&cPath=129_142_153&sort=20a&page=2"));
        }
        

        private Nop.Core.Domain.Catalog.Product ImportQualityCagesProduct(string url, out KeyValuePair<string, Uri> category)
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.QualityCages);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, new Uri(url));
            category = loader.GetCategoryLink(parser);
            return loader.LoadProduct(parser);
        }
    }
}
