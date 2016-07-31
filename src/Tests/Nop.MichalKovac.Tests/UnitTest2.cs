using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.XPath;
using HtmlAgilityPack;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace Nop.MichalKovac.Tests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestProfifotoCategoryUrls()
        {
            var rootPage = new Uri(@"http://profifoto.cz/pfshop/index.php");
            var categoryUrlsExpression = @"/html/body/div[@id='body']/div[@id='column']/dl[@id='menu1']/dt/a";

            var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0;)");
            var productWeb = client.DownloadString(rootPage);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(productWeb);
            var navigator = doc.DocumentNode.CreateNavigator();

            var categoryLinkNodes = doc.DocumentNode.SelectNodes(categoryUrlsExpression);

            var categories = new Dictionary<string, Uri>();
            foreach(var categoryNode in categoryLinkNodes)
            {
                categories[categoryNode.InnerText] = new System.Uri(rootPage, WebUtility.HtmlDecode(categoryNode.Attributes["href"].Value));
            }

            Assert.IsTrue(categories.ContainsKey("Trikové efekty"));
            Assert.AreEqual(@"http://profifoto.cz/pfshop/index.php?p=p_19&sName=trikov%C3%A9-efekty", categories["Trikové efekty"].AbsoluteUri);
        }

        [TestMethod]
        public void TestProfifotoProductDetailUrls()
        {
            var pageWithProducts = new Uri(@"http://profifoto.cz/pfshop/index.php?p=p_19&sName=trikov%C3%A9-efekty");
            var productDetailExpression = @"/html/body/div[@id='body']/div[@id='content']/div[@id='productsList']/div[@class='entry']/h2/a";
            var nextPageExpression = @"/html/body/div[@id='body']/div[@id='content']/div[@id='productsList']/div[@id='pages']/a[starts-with(., 'Dal')]";

            var client = new WebClient();

            var productLinks = new List<Uri>();

            TraverseCategory(pageWithProducts, client, productDetailExpression, nextPageExpression, ref productLinks);
            
            Assert.IsTrue(productLinks.Any(link => link.AbsoluteUri == (new Uri(@"http://profifoto.cz/pfshop/index.php?p=productsMore&iProduct=4&sName=trikové-efekty-grated-ice-effect-ledová-tříšť-kod-zboží-01603")).AbsoluteUri));
            Assert.IsTrue(productLinks.Count > 10);
        }

        private void TraverseCategory(Uri startPage, WebClient client, string productDetailExpression, string nextPageExpression, ref List<Uri> productLinks)
        {
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0;)");
            var productWeb = client.DownloadString(startPage);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(productWeb);

            var productLinkNodes = doc.DocumentNode.SelectNodes(productDetailExpression);
            foreach (var productNode in productLinkNodes)
            {
                productLinks.Add(new System.Uri(startPage, WebUtility.HtmlDecode(productNode.Attributes["href"].Value)));
            }

            var nextPageLinkNode = doc.DocumentNode.SelectSingleNode(nextPageExpression);
            if (nextPageLinkNode != null && nextPageLinkNode.HasAttributes)
            {
                TraverseCategory(new Uri(startPage, WebUtility.HtmlDecode(nextPageLinkNode.Attributes["href"].Value)), client, productDetailExpression, nextPageExpression, ref productLinks);
            }
        }
    }
}
