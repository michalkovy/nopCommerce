using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Nop.Plugin.Misc.WebImporter
{
    public class PageParser
    {
        private HtmlDocument document;
        private XPathNavigator navigator;
        private static XPathExpression baseExpression = XPathExpression.Compile("string(/html/head/base/@href)");
        private Uri baseUri;

        private PageParser(string htmlPage, Uri pageAddress)
        {
            document = new HtmlDocument();
            document.LoadHtml(htmlPage);
            navigator = document.CreateNavigator();
            var baseUrlInPage = (string)navigator.Evaluate(baseExpression);
            baseUri = String.IsNullOrWhiteSpace(baseUrlInPage) ? pageAddress : new Uri(baseUrlInPage);
        }

        public static PageParser CreatePageParser(WebClient webClient, Uri pageAddress)
        {
            string htmlPage;
            try
            {
                htmlPage = webClient.DownloadString(pageAddress);
            }
            catch (WebException)
            {
                //try twice
                htmlPage = webClient.DownloadString(pageAddress);
            }
            return new PageParser(htmlPage, pageAddress);
        }

        public Uri GetAbsoluteUri(string url)
        {
            return new Uri(baseUri, url);
        }

        public Uri GetUri(XPathExpression xpath)
        {
            var url = GetString(xpath);
            return String.IsNullOrWhiteSpace(url) ? null : GetAbsoluteUri(url);
        }

        public List<Uri> GetUris(XPathExpression xpath)
        {
            var nodes = (XPathNodeIterator)navigator.Evaluate(xpath);
            var result = new List<Uri>();
            foreach (XPathNavigator productNode in nodes)
            {
                if (!String.IsNullOrWhiteSpace(productNode.Value))
                {
                    result.Add(GetAbsoluteUri(WebUtility.HtmlDecode(productNode.Value)));
                }
            }
            return result;
        }

        public string GetString(XPathExpression xpath)
        {
            return WebUtility.HtmlDecode((string)navigator.Evaluate(xpath));
        }

        public string GetClearedString(XPathExpression xpath)
        {
            var firstResult = GetString(xpath);
            return firstResult == null ? null : Regex.Replace(firstResult, @"\s+", " ").Trim();
        }

        public decimal GetDecimal(XPathExpression xpath)
        {
            var result = (double)navigator.Evaluate(xpath);
            return Double.IsNaN(result) ? Decimal.MinValue :
                result == Double.MaxValue ? Decimal.MaxValue :
                result == Double.MinValue ? Decimal.MinValue : (decimal)result;
        }

        public string GetHtmlAndInnerTextFromNodes(string xpath, out string innerText)
        {
            var nodes = document.DocumentNode.SelectNodes(xpath);
            if (nodes == null)
            {
                innerText = "";
                return "";
            }
            
            innerText = nodes.Aggregate("", (actual, next) => actual == "" ? next.InnerText : actual + " " + next.InnerText);
            return nodes.Aggregate("", (actual, next) => actual == "" ? next.InnerHtml : actual + "<br/>" + next.InnerHtml);
        }

        public KeyValuePair<string, Uri> GetLink(string xpath)
        {
            var linkNode = document.DocumentNode.SelectSingleNode(xpath);
            return new KeyValuePair<string, Uri>(
                    linkNode.InnerText,
                    GetAbsoluteUri(Regex.Replace(WebUtility.HtmlDecode(linkNode.Attributes["href"].Value), "&zenid=.*", "")));
        }

        public Dictionary<string, Uri> GetLinks(string xpath)
        {
            var result = new Dictionary<string, Uri>();
            var linkNodes = document.DocumentNode.SelectNodes(xpath);

            foreach (var node in linkNodes)
            {
                var name = node.InnerText;
                var uri = GetAbsoluteUri(WebUtility.HtmlDecode(node.Attributes["href"].Value));
                result[name] = uri;
            }
            return result;
        }
    }
}
