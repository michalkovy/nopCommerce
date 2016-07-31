using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nop.Plugin.Misc.WebImporter;

namespace Nop.MichalKovac.Tests
{
    [TestClass]
    public class MxlMicsImport
    {
        [TestMethod]
        public void MxlMicsMainCategoryUrls()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.MxlMics);
            var testPage = new Uri(settings.BaseUrl);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var links = loader.GetCategoryLinks(parser);
            Assert.AreEqual(11, links.Count);
            Assert.IsTrue(links.ContainsKey("USB"));
            Assert.AreEqual(@"http://www.mxlmics.com/microphones/usb/", links["USB"].AbsoluteUri);
        }

        [TestMethod]
        public void MxlMicsUsbUrls()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.MxlMics);
            var testPage = new Uri(@"http://www.mxlmics.com/microphones/usb/");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var links = loader.GetProductDetailsUris(parser);
            Assert.AreEqual(21, links.Count);
            Assert.AreEqual(@"http://www.mxlmics.com/microphones/usb/009/", links[3].AbsoluteUri);
        }

        [TestMethod]
        public void MxlMicsFR500WK()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.MxlMics);
            var testPage = new Uri(@"http://www.mxlmics.com/microphones/field-recording/FR-500WK/");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            Assert.IsTrue(product.FullDescription.Trim().StartsWith("<div"), String.Format("Spatny obsah: {0}", product.FullDescription));
            Assert.AreEqual("MXL FR-500WK Professional Portable Wireless Audio System", product.Name);
            Assert.AreEqual("MXL-FR-500WK", product.Sku);
            Assert.AreEqual(0M, product.Price);
            Assert.AreEqual(0M, product.ProductCost);
            Assert.AreEqual(0M, product.OldPrice);
            Assert.AreEqual("The MXL FR-500WK microphone set is a professional portable wireless audio system design for on-set field work.", product.ShortDescription);

            var pictures = loader.GetPictureUris(parser);
            Assert.AreEqual(6, pictures.Count);
            Assert.AreEqual("http://www.mxlmics.com/microphones/field-recording/FR-500WK/images/FR-500WK-display.jpg", pictures[0].AbsoluteUri);
            Assert.AreEqual("http://www.mxlmics.com/microphones/field-recording/FR-500WK/images/TransmitterwithMic.jpg", pictures[1].AbsoluteUri);

            Assert.AreEqual("http://www.mxlmics.com/microphones/field-recording/", loader.GetCategoryLink(parser).Value.AbsoluteUri);

            product.Price = 1M;
            product.ProductCost = 2M;
            product.OldPrice = 3M;
            var newProduct = loader.LoadProduct(parser, product);
            Assert.AreEqual(1M, newProduct.Price);
            Assert.AreEqual(2M, newProduct.ProductCost);
            Assert.AreEqual(3M, newProduct.OldPrice);
        }

        [TestMethod]
        public void MxlMicsUsb009()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.MxlMics);
            var testPage = new Uri(@"http://www.mxlmics.com/microphones/usb/009/");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            Assert.IsTrue(product.FullDescription.Trim().StartsWith("<h2"), String.Format("Spatny obsah: {0}", product.FullDescription));
            Assert.AreEqual("MXL USB.009 24-bit/96kHz USB Microphone", product.Name);
            Assert.AreEqual("MXL-USB.009", product.Sku);
            Assert.AreEqual(0M, product.Price);
            Assert.AreEqual(0M, product.ProductCost);
            Assert.AreEqual(0M, product.OldPrice);
            Assert.AreEqual("The MXL USB.009 is a large-capsule, 24-bit USB condenser microphone that brings exceptional quality to vocal recordings.", product.ShortDescription);

            var pictures = loader.GetPictureUris(parser);
            Assert.AreEqual(4, pictures.Count);
            Assert.AreEqual("http://www.mxlmics.com/microphones/usb/009/images/USB_009_large.jpg", pictures[0].AbsoluteUri);
            Assert.AreEqual("http://www.mxlmics.com/microphones/usb/009/images/hr3.jpg", pictures[1].AbsoluteUri);

            Assert.AreEqual("http://www.mxlmics.com/microphones/usb/", loader.GetCategoryLink(parser).Value.AbsoluteUri);
        }

        [TestMethod]
        public void MxlMicsFR366K()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.MxlMics);
            var testPage = new Uri(@"http://www.mxlmics.com/microphones/field-recording/FR-366k/");
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            Assert.AreEqual("MXL-FR-366K", product.Sku);
            
            var pictures = loader.GetPictureUris(parser);
            Assert.AreEqual(1, pictures.Count);
            Assert.AreEqual("http://www.mxlmics.com/microphones/field-recording/FR-366k/images/Lav_kit.jpg", pictures[0].AbsoluteUri);
        }
    }
}
