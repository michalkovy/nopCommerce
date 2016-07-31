using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nop.Plugin.Widgets.Flexibee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml.Linq;
using System.Globalization;

namespace Nop.MichalKovac.Tests
{
    [TestClass]
    public class StockImportTests
    {
        /* performance tests
        [TestMethod]
        public void DeserializationSpeed()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebRequest request = WebRequest.Create(@"https://saskia.cz:5434/c/michal_kovac/cenik.xml?detail=custom:sumDostupMj&limit=0");
            FlexibeeConfiguration.ConfigureFlexibeeRequest(request, false);
            string casovani = "Po konfiguraci: " + sw.Elapsed.ToString() + Environment.NewLine;

            Nop.Plugin.Widgets.Flexibee.Cenik.Export.winstrom cenik;
            using (WebResponse myWebResponse = request.GetResponse())
            {
                casovani += "Po requestu: " + sw.Elapsed.ToString() + Environment.NewLine;
                XmlSerializer serializer = new XmlSerializer(typeof(Nop.Plugin.Widgets.Flexibee.Cenik.Export.winstrom));
                using (XmlTextReader xmlReader = new XmlTextReader(myWebResponse.GetResponseStream()))
                {
                    cenik = (Nop.Plugin.Widgets.Flexibee.Cenik.Export.winstrom)serializer.Deserialize(xmlReader);
                }
                casovani += "Po deserializaci: " + sw.Elapsed.ToString() + Environment.NewLine;
            }
            casovani += "Po vycisteni streamu: " + sw.Elapsed.ToString() + Environment.NewLine;
            var data = (from c in cenik.cenik
                       select new
                       {
                           Id = c.id.Where(e => e.Value.StartsWith("ext:CIN:P")).Select(e => Int32.Parse(e.Value.Replace("ext:CIN:P", ""))).FirstOrDefault(),
                           Stav = Decimal.Parse(c.sumDostupMj.Value, NumberStyles.Any, CultureInfo.InvariantCulture)
                       }).Where(d => d.Id != 0);
            casovani += "Po query: " + sw.Elapsed.ToString() + Environment.NewLine;
            StringBuilder ids = new StringBuilder("");
            foreach (var dat in data)
            {
                ids.Append(dat.Id);
                ids.Append(",");
            }
            casovani += "Po foreach: " + sw.Elapsed.ToString() + Environment.NewLine;
            casovani += ids.ToString();
            Console.Write(casovani);
            sw.Stop();
        }

        [TestMethod]
        public void XmlLinqSpeed()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebRequest request = WebRequest.Create(@"https://saskia.cz:5434/c/michal_kovac/cenik.xml?detail=custom:sumDostupMj&limit=0");
            FlexibeeConfiguration.ConfigureFlexibeeRequest(request, false);
            string casovani = "Po konfiguraci: " + sw.Elapsed.ToString() + Environment.NewLine;

            using (WebResponse myWebResponse = request.GetResponse())
            {
                casovani += "Po requestu: " + sw.Elapsed.ToString() + Environment.NewLine;
                XDocument xdoc = XDocument.Load(myWebResponse.GetResponseStream());
                casovani += "Po loadu: " + sw.Elapsed.ToString() + Environment.NewLine;
                var data = from c in xdoc.Elements("cenik")
                   select new { 
                       Id = c.Elements("id").Select(e => e.Value).Where(v => v.StartsWith("ext:CIN:P")).FirstOrDefault(),
                       Stav = Decimal.Parse(c.Element("sumDostupMj").Value) };
                casovani += "Po query: " + sw.Elapsed.ToString() + Environment.NewLine;
                foreach (var dat in data)
                {
                    casovani += dat.Id + ",";
                }
                casovani += Environment.NewLine;
                casovani += "Po foreach: " + sw.Elapsed.ToString() + Environment.NewLine;
            }
            Console.Write(casovani);
            sw.Stop();
        }*/
    }
}
