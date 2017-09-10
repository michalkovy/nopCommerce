using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Nop.Plugin.Misc.WebImporter;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Nop.MichalKovac.Tests
{
    [TestClass]
    public class MichalKovacTests
    {
        [TestMethod]
        public void TestUrlConcatenation()
        {
            var lastOrder = new Order() { Id = 11, BillingAddress = new Address() { Email = "a@b.cz" } };
            lastOrder.OrderItems.Add(new OrderItem() { Id = 1 });
            lastOrder.OrderItems.Add(new OrderItem() { Id = 10 });

            var items = new StringBuilder();
            foreach (var productVariant in lastOrder.OrderItems)
            {
                items.AppendFormat(@"itemId[]={0}&", productVariant.Id);
            }

            var url =
                String.Format(
                    @"http://www.heureka.cz/direct/dotaznik/objednavka.php?id={0}&email={1}&{2}orderid={3}",
                    123456, lastOrder.BillingAddress.Email, items, lastOrder.Id);
            Assert.AreEqual(@"http://www.heureka.cz/direct/dotaznik/objednavka.php?id=123456&email=a@b.cz&itemId[]=1&itemId[]=10&orderid=11", url);
        }

        [TestMethod]
        public void GetCategories()
        {
            var categories = GetWebCategories();
            Assert.AreEqual(3170, categories.Where(c => c.Id == 3323).First().Parent);
        }

        protected class HeurekaCategory
        {
            public int Id { get; set; }
            public int Parent { get; set; }
            public string Name { get; set; }
            public int DisplayOrder { get; set; }
        }

        [TestMethod]
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

        [TestMethod]
        public void ImportKour()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=2&sName=trikové-efekty-vapour-effect-kouř-kod-zboží-01601");
            Assert.AreEqual("Trikové efekty - VAPOUR EFFECT Kouř", product.Name);
            Assert.AreEqual("01601", product.Sku);
            Assert.AreEqual(Math.Round(1226.0M / 1.21M,5), Math.Round(product.Price,5));
            Assert.AreEqual("VAPOUR EFECT - 2x 25 ml velmi užitečný umělý kouř.", product.ShortDescription);
        }

        [TestMethod]
        public void ImportKobold()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=831&sName=kod-zbo%9E%ED-332-0041-x-kobold-dw-575-p-set-%28-vod%EC-odoln%E9-daylight-sv%ECtlo-%29");
            Assert.AreEqual("Kobold DW 575 P SET ( vodě odolné daylight světlo )", product.Name);
            Assert.AreEqual("332-0041-X", product.Sku);
            Assert.AreEqual(179414.0M / 1.21M, product.Price);
            Assert.AreEqual("Souprava obsahuje: Lampa DW 575, IP 54 vč.", product.ShortDescription);
        }

        [TestMethod]
        public void ImportVetrak()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=100&sName=kod-zbo%9E%ED-04376-fotografick%FD-v%ECtr%E1k-condor-foto");
            Assert.AreEqual("Fotografický větrák Condor Foto", product.Name);
            Assert.AreEqual("04376", product.Sku);
            Assert.AreEqual(10900.0M / 1.21M, product.Price);
            Assert.AreEqual("Po vzoru hollywoodských filmových studií získáte silný proud vzduchu napodobující vítr.", product.ShortDescription);
        }

        [TestMethod]
        public void ImportPrechodPozadi()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=1158&sName=kod-zbo%9E%ED-166c-p%D8echodov%C9-pozad%CD-110x160-cm-modr%C1-%96-zelen%C1");
            Assert.AreEqual("166C", product.Sku);
            Assert.AreEqual("PŘECHODOVÉ POZADÍ 110x160 cm MODRÁ – ZELENÁ", product.Name);
        }

        [TestMethod]
        public void ImportPrechodPozadi2()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=1162&sName=kod-228c-p%D8echodov%C9-pozad%CD-110x160-cm-%C8ern%C1-%96-sv%CCtle-modr%C1");
            Assert.AreEqual("228C", product.Sku);
            Assert.AreEqual("PŘECHODOVÉ POZADÍ 110x160 cm ČERNÁ – SVĚTLE MODRÁ", product.Name);
        }
        
        [TestMethod]
        public void ImportPrechodPozadi3()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=1148&sName=kod-zbo%9E%ED-106c-%96-p%D8echodov%C9-pozad%CD-110x160-cm-oran%8Eov%C1-b%CDl%C1");
            Assert.AreEqual("106C", product.Sku);
            Assert.AreEqual("PŘECHODOVÉ POZADÍ 110x160 cm ORANŽOVÁ - BÍLÁ", product.Name);
            Assert.AreEqual(1, product.VendorId);
            Assert.AreEqual(2, product.WarehouseId);
        }
        
        
        [TestMethod]
        public void ImportKufrik()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=204&sName=kod-zbo%9E%ED-1.2008-o-si-typ-05-%96-oran%9Eov%FD-mal%FD-pevn%FD-kuf%F8%EDk-v%E8.-p%ECnov%E9-vlo%9Eky");
            Assert.AreEqual("1.2008/O/SI", product.Sku);
            Assert.AreEqual("TYP 05 – Oranžový malý pevný kufřík vč. pěnové vložky", product.Name);
        }

        [TestMethod]
        public void TestProfifotoPictures()
        {
            var testPage = new Uri(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=1029&sName=kod-zbo%9E%ED-fv-bsle3200-%96-photoflex-starlite-body-ql");
            var settings = Configurations.GetConfiguration(SupportedSite.Profifoto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var pictures = loader.GetPictureUris(parser);
            Assert.AreEqual(@"http://profifoto.cz/pfshop/files/pflx_04.jpg", pictures[0].AbsoluteUri);
            Assert.AreEqual(8, pictures.Count);
            Assert.AreEqual(@"http://profifoto.cz/pfshop/files/pflx_06.jpg", pictures[2].AbsoluteUri);
        }

        [TestMethod]
        public void TestZacutoProxy()
        {
            var productWeb = Configurations.GetClient(Configurations.GetConfiguration(SupportedSite.Zacuto)).DownloadString(@"http://store.zacuto.com");

            Assert.IsTrue(productWeb.Contains("Zacuto"), "actual text: {0}", productWeb);
        }

        [TestMethod]
        public void ImportZacutoC100Zfinder()
        {

            var product = ImportZacutoProduct(@"http://store.zacuto.com/c100-z-finder/");
            Assert.AreEqual("C100 Z-Finder", product.Name);
            Assert.AreEqual("Z-FIND-C1", product.Sku);
            Assert.AreEqual(365M * Configurations.GetConfiguration(SupportedSite.Zacuto).EndPriceWithouTaxMultiplication, product.Price);
            Assert.AreEqual(365M * 0.65M, product.ProductCost);
            Assert.AreEqual("The C100 Z-Finder Pro is an optical viewfinder, specifically designed to attach to the Canon C100 cinema camera LCD.", product.ShortDescription);
            //Assert.AreEqual("Z-FIND-C1, c100 Z-finder pro, canon c100, viewfinder, zacuto, zfinder, c100, c100 zfinder", product.MetaKeywords);
        }

        [TestMethod]
        public void ImportZacutoDslrRecoil()
        {

            var product = ImportZacutoProduct(@"http://store.zacuto.com/dslr-recoil/");
            Assert.AreEqual(1695M* Configurations.GetConfiguration(SupportedSite.Zacuto).EndPriceWithouTaxMultiplication, product.Price);
            Assert.AreEqual(0M, product.OldPrice);
            Assert.AreEqual(1695M * 0.65M, product.ProductCost);

            var testPage = new Uri(@"http://store.zacuto.com/dslr-recoil/");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Zacuto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);
            
            Assert.AreEqual("Zacuto", loader.GetManufacturerName(parser));

            var category = loader.GetCategoryLink(parser);
            Assert.AreEqual("Additional Rigs", category.Key);
            Assert.AreEqual(@"http://store.zacuto.com/additional-rigs/", category.Value.AbsoluteUri);
        }

        [TestMethod]
        public void ImportZacutoScorpion()
        {

            var product = ImportZacutoProduct(@"http://store.zacuto.com/scorpion/");
            Assert.AreEqual(2565M * Configurations.GetConfiguration(SupportedSite.Zacuto).EndPriceWithouTaxMultiplication, product.Price);
            //Assert.AreEqual(2700M * Configurations.GetConfiguration(SupportedSite.Zacuto).EndPriceWithouTaxMultiplication, product.OldPrice);
            //Assert.AreEqual(2700M * 0.65M, product.ProductCost);
            Assert.AreEqual(2565M * 0.65M, product.ProductCost);
        }

        [TestMethod]
        public void ImportZacutoZFocusV2()
        {

            var product = ImportZacutoProduct(@"http://store.zacuto.com/z-focus-v2/");
            Assert.AreEqual("Z-Focus V2", product.Name);
            Assert.AreEqual("Z-FF-2", product.Sku);
            Assert.AreEqual(1050M* Configurations.GetConfiguration(SupportedSite.Zacuto).EndPriceWithouTaxMultiplication, product.Price);
            Assert.AreEqual("An extremely accurate follow focus that allows the operator or assistant to pull focus. Includes a complete gear driven, multiple gear pitches and more.", product.ShortDescription);
            Assert.AreEqual("zacuto, dslr rigs, optical viewfinder, evf, Z-Finder, camera accessories, Z-Focus V2", product.MetaKeywords);
        }

        [TestMethod]
        public void ImportZacutoYCable()
        {

            var product = ImportZacutoProduct(@"https://store.zacuto.com/canon-y-cable/");
            Assert.AreEqual("Canon Y Cable for Canon zoom and focus controls", product.ShortDescription);
        }

        [TestMethod]
        public void ImportZacutoMarauder()
        {
            var product = ImportZacutoProduct(@"http://store.zacuto.com/marauder/");
            Assert.IsTrue(product.FullDescription.Contains(@"<a href=""http://store.zacuto.com/striker/"" target=""_blank"">Striker</a>"));

            var testPage = new Uri(@"http://store.zacuto.com/marauder/");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Zacuto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var pictures = loader.GetPictureUris(parser);
            foreach (var picture in pictures)
            {
                WebClient client2 = Configurations.GetClient(settings);
                var downloadedPicture = client2.DownloadData(picture);
                var mimeType = client.ResponseHeaders[HttpResponseHeader.ContentType];
                Assert.IsTrue(downloadedPicture.Length > 50);
                Assert.IsTrue(!String.IsNullOrWhiteSpace(mimeType));
                ValidatePicture(downloadedPicture, mimeType);
            }
        }

        
        [TestMethod]
        public void ImportZacutoLancImage()
        {
            var testPage = new Uri(@"http://store.zacuto.com/lanc-extension-cable-13.8/");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Zacuto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            product.FullDescription = ReplaceUrls(product.FullDescription, parser, loader);

            var pictures = loader.GetPictureUris(parser);
            foreach (var picture in pictures)
            {
                WebClient client2 = Configurations.GetClient(settings);
                var downloadedPicture = client2.DownloadData(picture);
                var mimeType = client.ResponseHeaders[HttpResponseHeader.ContentType];
                Assert.IsTrue(downloadedPicture.Length > 50);
                Assert.IsTrue(!String.IsNullOrWhiteSpace(mimeType));
                ValidatePicture(downloadedPicture, mimeType);
            }
        }

        private string ReplaceUrls(string html, PageParser originalPage, PageLoader loader)
        {
            var matches = Regex.Matches(html, @"href=""([^""]*)""");
            foreach (Match match in matches)
            {
                var url = match.Groups[1].Value;
                var absoluteUri = originalPage.GetAbsoluteUri(url);
                var parser = PageParser.CreatePageParser(Configurations.GetClient(Configurations.GetConfiguration(SupportedSite.Zacuto)), absoluteUri);
                string sku = loader.GetProductSku(parser);
                Assert.IsFalse(String.IsNullOrWhiteSpace(sku), "Detected sku is empty");
                /*if (!String.IsNullOrWhiteSpace(sku))
                {
                    var correspondingProduct = _productService.GetProductBySku(sku);
                    if (correspondingProduct != null)
                    {
                        var productUrl = string.Format("{0}{1}", @"http://saskia.pro/", correspondingProduct.GetSeName(2));
                        html = html.Replace(url, productUrl);
                    }
                }*/
            }
            return html;
        }

        /// <summary>
        /// Validates input picture dimensions
        /// </summary>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary or throws an exception</returns>
        private byte[] ValidatePicture(byte[] pictureBinary, string mimeType)
        {
            using (var stream1 = new MemoryStream(pictureBinary))
            {
                using (var b = new Bitmap(stream1))
                {
                    var maxSize = 1000;
                    if ((b.Height <= maxSize) && (b.Width <= maxSize))
                        return pictureBinary;

                    var newSize = CalculateDimensions(b.Size, maxSize);
                    using (var newBitMap = new Bitmap(newSize.Width, newSize.Height))
                    {
                        using (var g = Graphics.FromImage(newBitMap))
                        {
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.DrawImage(b, 0, 0, newSize.Width, newSize.Height);

                            using (var stream2 = new MemoryStream())
                            {
                                var ep = new EncoderParameters();
                                //ep.Param[0] = new EncoderParameter(Encoder.Quality, _mediaSettings.DefaultImageQuality);
                                ImageCodecInfo ici = GetImageCodecInfoFromMimeType(mimeType);
                                if (ici == null)
                                    ici = GetImageCodecInfoFromMimeType("image/jpeg");
                                newBitMap.Save(stream2, ici, ep);
                                return stream2.GetBuffer();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the first ImageCodecInfo instance with the specified mime type.
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>ImageCodecInfo</returns>
        private ImageCodecInfo GetImageCodecInfoFromMimeType(string mimeType)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            foreach (var ici in info)
                if (ici.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
                    return ici;
            return null;
        }

        /// <summary>
        /// Calculates picture dimensions whilst maintaining aspect
        /// </summary>
        /// <param name="originalSize">The original picture size</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns></returns>
        private Size CalculateDimensions(Size originalSize, int targetSize)
        {
            var newSize = new Size();
            if (originalSize.Height > originalSize.Width) // portrait 
            {
                newSize.Width = (int)(originalSize.Width * (float)(targetSize / (float)originalSize.Height));
                newSize.Height = targetSize;
            }
            else // landscape or square
            {
                newSize.Height = (int)(originalSize.Height * (float)(targetSize / (float)originalSize.Width));
                newSize.Width = targetSize;
            }
            return newSize;
        }

        [TestMethod]
        public void TestZacutoPictures()
        {
            var testPage = new Uri(@"http://store.zacuto.com/c100-z-finder/");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Zacuto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);
            
            var pictures = loader.GetPictureUris(parser);
            Assert.AreEqual(@"https://store.zacuto.com/images/products/z-find-c1.jpg", pictures[0].AbsoluteUri);
            Assert.AreEqual(8, pictures.Count);
            Assert.AreEqual(@"https://store.zacuto.com/images/products/secondary/z-find-c1-5.jpg", pictures[2].AbsoluteUri);
        }

        private Nop.Core.Domain.Catalog.Product ImportZacutoProduct(string url)
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Zacuto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, new Uri(url));
            return loader.LoadProduct(parser);
        }

        private Nop.Core.Domain.Catalog.Product ImportProfifotoProduct(string url)
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Profifoto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, new Uri(url));
            return loader.LoadProduct(parser);
        }

        [TestMethod]
        public void TestAbsoluteUri()
        {
            string a = @"http://store.zacuto.com/fs700-grip-relocator/";
            var baseUri = new Uri(@"http://store.zacuto.com/");
            var newUri = new Uri(baseUri, a);
            Assert.AreEqual(@"http://store.zacuto.com/fs700-grip-relocator/", newUri.AbsoluteUri);
        }

        [TestMethod]
        public void ImportZacutoArm()
        {
            var testPage = new Uri(@"http://store.zacuto.com/zamerican-v3-large/");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Zacuto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            var product = loader.LoadProduct(parser);
            Assert.AreEqual(0.45359237M, product.Weight);
        }

        [TestMethod]
        public void ImportZacutoCategory()
        {
            var testPage = new Uri(@"https://store.zacuto.com/follow-focus/");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Zacuto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);

            Assert.IsNull(loader.GetNextPageUri(parser));
            

            testPage = new Uri(@"https://store.zacuto.com/z-finder-products/");
            parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);
            var nextPage = loader.GetNextPageUri(parser);
            Assert.AreEqual(@"https://store.zacuto.com/z-finder-products-page-2/", nextPage.AbsoluteUri);
            var uris = loader.GetProductDetailsUris(parser);
            Assert.AreEqual(18, uris.Count);
        }

        [TestMethod]
        public void ImportZacutoSite()
        {
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Zacuto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, new Uri(settings.BaseUrl));

            var categoryLinks = loader.GetCategoryLinks(parser);
            Assert.IsTrue(categoryLinks.Count > 20 && categoryLinks.Count < 34);
        }

        [TestMethod]
        public void ImportProfifotoPozadi()
        {
            var testPage = new Uri(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=1162&sName=kod-228c-p%D8echodov%C9-pozad%CD-110x160-cm-%C8ern%C1-%96-sv%CCtle-modr%C1");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Profifoto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);
            Assert.AreEqual(new Uri("http://profifoto.cz/pfshop/index.php?p=p_148&sName=přechodová-pozadí-1-1x1-6-m").AbsoluteUri, loader.GetCategoryLink(parser).Value.AbsoluteUri);

            var pictures = loader.GetPictureUris(parser);
            Assert.AreEqual(0, pictures.Count);
        }

        [TestMethod]
        public void ImportFlashCasa()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=201&sName=objednac%ED-%E8%EDslo-04340-excella-flash-casa-250-ws--v%E8etn%EC-z%E1kladn%EDho-reflektoru--v%FDbojky--%9E%E1rovky--synchroniza%E8n%EDho-kabelu-a-s%ED%9Dov%E9ho-kabelu");
            Assert.AreEqual("Excella Flash Casa 250 Ws, včetně základního reflektoru, výbojky, žárovky, synchronizačního kabelu a síťového kabelu", product.Name);
            Assert.AreEqual("04340", product.Sku);
        }
        
        [TestMethod]
        public void ImportMinicomSestava()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=402&sName=sestava-minicom-expert-kit-rfs-ateli%E9rov%E1-z%E1bleskov%E1-sv%ECtla-objednac%ED-%E8%EDslo-31.497.10");
            Assert.AreEqual("Sestava Minicom Expert Kit RFS - Ateliérová záblesková světla", product.Name);
            Assert.AreEqual("31.497.10", product.Sku);
        }

        [TestMethod]
        public void ImportRotoaSestava()
        {
            var product = ImportProfifotoProduct(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=47&sName=objednac%ED-kod-rotoa-mini-kit-foba");
            Assert.AreEqual("Mini-Kit Foba", product.Name);
            Assert.AreEqual("ROTOA", product.Sku);
        }

        [TestMethod]
        public void ImportProfifotoStock()
        {
            var testPage = new Uri(@"http://profifoto.cz/pfshop/index.php?p=p_144&sName=condor-foto");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Profifoto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);
            var availableUrls =loader.GetStockAvailableProductDetailsUris(parser);

            Assert.IsTrue(availableUrls.Count < 15 && availableUrls.Count > 5);
        }

        [TestMethod]
        public void ImportProfifotoStock2()
        {
            var testPage = new Uri(@"http://profifoto.cz/pfshop/index.php?p=p_20&sName=condor-foto");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Profifoto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);
            var availableUrls =loader.GetStockAvailableProductDetailsUris(parser);

            Assert.IsTrue(availableUrls.Count < 13 && availableUrls.Count > 5);
        }

        [TestMethod]
        public void ImportProfifotoStock3()
        {
            var testPage = new Uri(@"http://profifoto.cz/pfshop/index.php?p=p_145&sName=condor-foto");
            var settings = Nop.Plugin.Misc.WebImporter.Configurations.GetConfiguration(SupportedSite.Profifoto);
            var loader = new Nop.Plugin.Misc.WebImporter.PageLoader(settings);
            var client = Configurations.GetClient(settings);
            var parser = Nop.Plugin.Misc.WebImporter.PageParser.CreatePageParser(client, testPage);
            var availableUrls =loader.GetStockAvailableProductDetailsUris(parser);

            Assert.AreEqual(2, availableUrls.Count);
        }
        

        
    }
}
