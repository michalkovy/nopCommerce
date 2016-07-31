using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nop.Plugin.Misc.WebImporter;

namespace Nop.MichalKovac.Tests
{
    [TestClass]
    public class SwitronixImport
    {
        [TestMethod]
        public void SwitronixMainCategoryUrls()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Switronix);
            var testPage = new Uri(settings.BaseUrl);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var links = loader.GetCategoryLinks(parser);
            Assert.AreEqual(27, links.Count);
            Assert.IsTrue(links.ContainsKey("BMCC"));
            Assert.AreEqual(@"http://www.switronix.com/products?page=shop.browse&category_id=50", links["BMCC"].AbsoluteUri);
        }

        [TestMethod]
        public void SwitronixGoProRegulatorCable()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Switronix);
            var testPage = new Uri(@"http://www.switronix.com/products?page=shop.product_details&flypage=flypage.tpl&product_id=210&category_id=54");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            Assert.IsTrue(product.FullDescription.StartsWith("<h2>"), String.Format("Spatny obsah: {0}", product.FullDescription));
            Assert.AreEqual("GoPro Regulator Cable Y", product.Name);
            Assert.AreEqual("DV-GP3-XT60", product.Sku);
            Assert.AreEqual(0M, product.Price);
            Assert.AreEqual(0M, product.ProductCost);
            Assert.AreEqual(0M, product.OldPrice);
            Assert.AreEqual("DV-GP3-XT60 Regulator Cable for GoPro Hero 3/3+", product.ShortDescription);

            var pictures = loader.GetPictureUris(parser);
            Assert.AreEqual(4, pictures.Count);
            Assert.AreEqual("http://www.switronix.com/components/com_virtuemart/shop_image/product/GoPRO_Regulator__531f28a337924.jpg", pictures[0].AbsoluteUri);
            Assert.AreEqual("http://www.switronix.com/images/stories/igallery/dv_gp3_xt60/lightbox/DV_GP3_XT60.jpg", pictures[1].AbsoluteUri);

            product.Price = 1M;
            product.ProductCost = 2M;
            product.OldPrice = 3M;
            var newProduct = loader.LoadProduct(parser, product);
            Assert.AreEqual(1M, newProduct.Price);
            Assert.AreEqual(2M, newProduct.ProductCost);
            Assert.AreEqual(3M, newProduct.OldPrice);
        }

        [TestMethod]
        public void SwitronixGoProSolutions()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Switronix);
            var testPage = new Uri(@"http://www.switronix.com/products?page=shop.browse&category_id=54");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var products = loader.GetProductDetailsUris(parser);
            Assert.AreEqual(8, products.Count);
        }

        [TestMethod]
        public void SwitronixBT220()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Switronix);
            var testPage = new Uri(@"http://www.switronix.com/products?page=shop.product_details&flypage=flypage.tpl&product_id=208&category_id=46");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            Assert.IsTrue(product.FullDescription.StartsWith("<h2>"), String.Format("Spatny obsah: {0}", product.FullDescription));
            Assert.AreEqual("TL-BT220", product.Name, "Product name differs");
            Assert.AreEqual("TL-BT220", product.Sku, "Product sku differs");
            Assert.AreEqual(0M, product.Price);
            Assert.AreEqual(0M, product.ProductCost);
            Assert.AreEqual(0M, product.OldPrice);
            Assert.AreEqual("The BOLT 220 is the newest addition to the TorchLED Light line.", product.ShortDescription, "Product short description differs");
        }

        [TestMethod]
        public void SwitronixNextPage()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Switronix);
            var testPage = new Uri(@"http://www.switronix.com/products?page=shop.browse&category_id=41");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            Assert.AreEqual("http://www.switronix.com/products?category_id=41&page=shop.browse&limit=27&start=27", loader.GetNextPageUri(parser).AbsoluteUri);
        }

        [TestMethod]
        public void SwitronixDslrProA()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Switronix);
            var testPage = new Uri(@"http://www.switronix.com/products?page=shop.product_details&flypage=flypage.tpl&product_id=82&category_id=39");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            Assert.IsTrue(product.FullDescription.StartsWith("<h2>"), String.Format("Spatny obsah: {0}", product.FullDescription));
            Assert.AreEqual("DSLR-PRO/A", product.Name, "Product name differs");
            Assert.AreEqual("DSLR-PRO/A", product.Sku, "Product sku differs");
        }

        [TestMethod]
        public void SwitronixRec5Xrx()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Switronix);
            var testPage = new Uri(@"http://www.switronix.com/products?page=shop.product_details&flypage=flypage.tpl&product_id=236&category_id=49");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            Assert.IsTrue(product.FullDescription.StartsWith("<h2>"), String.Format("Spatny obsah: {0}", product.FullDescription));
            Assert.AreEqual("V-Mnt Recon X5 Add. Receiver", product.Name, "Product name differs");
            Assert.AreEqual("REC5-XRX-V", product.Sku, "Product sku differs");
        }

        [TestMethod]
        public void SwitronixPb70Qr()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Switronix);
            var testPage = new Uri(@"http://www.switronix.com/products?page=shop.product_details&flypage=flypage.tpl&product_id=135&category_id=47");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            Assert.IsTrue(product.FullDescription.StartsWith("<h2>"), String.Format("Spatny obsah: {0}", product.FullDescription));
            Assert.AreEqual("PB70QR", product.Name, "Product name differs");
            Assert.AreEqual("PB70QR", product.Sku, "Product sku differs");
        }
    }
}
