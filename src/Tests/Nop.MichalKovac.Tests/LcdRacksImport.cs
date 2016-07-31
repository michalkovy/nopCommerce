using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nop.Plugin.Misc.WebImporter;

namespace Nop.MichalKovac.Tests
{
    [TestClass]
    public class LcdRacksImport
    {
        [TestMethod]
        public void LcdRacksCV500MB()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.LcdRacks);
            var testPage = new Uri(@"http://www.lcdracks.com/servers-cameras/HD-SDI-cameras/CV500-MB-2.php");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            //Assert.IsTrue(product.FullDescription.Trim().StartsWith("<div"), String.Format("Spatny obsah: {0}", product.FullDescription));
            Assert.AreEqual("CV500-MB-2", product.Name);
            Assert.AreEqual("CV500-MB-2", product.Sku);
            Assert.AreEqual(0M, product.Price);
            Assert.AreEqual(0M, product.ProductCost);
            Assert.AreEqual(0M, product.OldPrice);
            Assert.AreEqual("Full-HD 2MP Mini-Broadcast Camera 1080p60/59.94/50fps, 1080i60/59.94/50, 720p60/59.94/50 with 3.7mm HD Prime Lens", product.ShortDescription);

            var pictures = loader.GetPictureUris(parser);
            Assert.AreEqual(1, pictures.Count);
            Assert.AreEqual("http://www.lcdracks.com/servers-cameras/HD-SDI-cameras/CV500-MB-2/CV500-MB-2.jpg", pictures[0].AbsoluteUri);

            Assert.AreEqual("http://www.lcdracks.com/servers-cameras/HD-SDI-cameras/index.php", loader.GetCategoryLink(parser).Value.AbsoluteUri);

            product.Price = 1M;
            product.ProductCost = 2M;
            product.OldPrice = 3M;
            var newProduct = loader.LoadProduct(parser, product);
            Assert.AreEqual(1M, newProduct.Price);
            Assert.AreEqual(2M, newProduct.ProductCost);
            Assert.AreEqual(3M, newProduct.OldPrice);
        }
    }
}
