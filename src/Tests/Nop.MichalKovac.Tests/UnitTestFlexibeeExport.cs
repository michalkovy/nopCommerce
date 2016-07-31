using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Nop.Plugin.Widgets.Flexibee;
using System.Globalization;
using Nop.Core.Infrastructure;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Events;

namespace Nop.MichalKovac.Tests
{
    [TestClass]
    public class UnitTestFlexibeeExport
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            FlexibeeCommunicator.FlexibeeServer = "saskia.cz:443";
        }

        /*
        [TestMethod]
        public void TestRealizace()
        {
            string filePath = "FlexibeeLastRealizaceImport.xml";
            var realizXml = File.ReadAllText(filePath, Encoding.UTF8);

            Console.WriteLine(FlexibeeCommunicator.SendXmlToFlexibee(realizXml, "objednavka-prijata", writeToFile: false));
            
            WebRequest request = WebRequest.Create(@"http://saskia.cz:5434/c/michal_kovac/objednavka-prijata.xml");
            FlexibeeConfiguration.ConfigureFlexibeeRequest(request);
            using (var stream = request.GetRequestStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(realizXml);
                }
            }
            try
            {
                using (WebResponse myWebResponse = request.GetResponse())
                {
                    using (var reader = myWebResponse.GetResponseStream())
                    {
                        using (var stream = new MemoryStream())
                        {
                            reader.CopyTo(stream);
                            byte[] bytes = stream.ToArray();
                            string text = System.Text.Encoding.UTF8.GetString(bytes);
                            Assert.AreEqual("Success", text);
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Assert.AreEqual("Success", text);
                    }
                }
            }
        }*/

        /*
        [TestMethod]
        public void TestStrom()
        {
            string filePath = "FlexibeeLaststromImport.xml";
            var stromXml = File.ReadAllText(filePath, Encoding.UTF8);
            
            Console.WriteLine(FlexibeeCommunicator.SendXmlToFlexibee(stromXml, "strom", writeToFile: false));

            
            filePath = "FlexibeeLaststrom-cenikImport.xml";
            stromXml = File.ReadAllText(filePath, Encoding.UTF8);

            Console.WriteLine(FlexibeeCommunicator.SendXmlToFlexibee(stromXml, "strom-cenik", writeToFile: false));
        }*/

        /* vytvari rezervace ve flexibee
        [TestMethod]
        public void TestRezervace()
        {
            string filePath = "FlexibeeLastRezervaceImport.xml";
            var rezervaceXml = File.ReadAllText(filePath);

            WebRequest request = WebRequest.Create(@"http://saskia.cz:5434/c/michal_kovac/rezervace.xml");
            FlexibeeConfiguration.ConfigureFlexibeeRequest(request);
            using (var stream = request.GetRequestStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(rezervaceXml);
                }
            }
            try
            {
                using (WebResponse myWebResponse = request.GetResponse())
                {
                    using (var reader = myWebResponse.GetResponseStream())
                    {
                        using (var stream = new MemoryStream())
                        {
                            reader.CopyTo(stream);
                            byte[] bytes = stream.ToArray();
                            string text = System.Text.Encoding.UTF8.GetString(bytes);
                            Assert.IsTrue(text.Contains("<created>4</created>"));
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Assert.Fail(text);
                    }
                }
            }
        }

        [TestMethod]
        public void TestEmail()
        {
            SendEmailWithInvoice(3359, "michal.kovac@saskia.cz", "CIN");
        }*/


        [TestMethod]
        [DeploymentItem("FlexibeeDeleteId.xml")]
        public void TestOdebraniId()
        {
            string filePath = "FlexibeeDeleteId.xml";
            var realizXml = File.ReadAllText(filePath, Encoding.UTF8);

            Console.WriteLine(FlexibeeCommunicator.SendXmlToFlexibee(realizXml, "objednavka-prijata", writeToFile: false));
        }


        private void SendEmailWithInvoice(int orderId, string emailTo, string flexibeeExternalIdPrefix)
        {
            string email = "V příloze Vám posíláme zálohový daňový doklad, který i obsahuje údaje pro platbu.";
            string subject = "Zalohovy danovy doklad";

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                WebRequest request = WebRequest.Create(
                    String.Format(@"https://{4}/c/michal_kovac/faktura-vydana/ext:{0}:FVO{1}/odeslani-dokladu.xml?to={2}&subject={3}",
                    flexibeeExternalIdPrefix,
                    orderId,
                    HttpUtility.UrlEncode(emailTo),
                    HttpUtility.UrlEncode(subject),
                    FlexibeeCommunicator.FlexibeeServer));
                FlexibeeConfiguration.ConfigureFlexibeeRequest(request, true);
                using (var stream = request.GetRequestStream())
                {
                    using (var writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        writer.Write(email);
                    }
                }
                using (WebResponse myWebResponse = request.GetResponse())
                {
                    using (var reader = myWebResponse.GetResponseStream())
                    {
                        using (var stream = new MemoryStream())
                        {
                            reader.CopyTo(stream);
                            byte[] bytes = stream.ToArray();
                            string text = System.Text.Encoding.UTF8.GetString(bytes);
                            Assert.AreEqual("Success", text);
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Assert.AreEqual("Success", text);
                    }
                }
            }
        }

        [TestMethod]
        public void TestRezervaceImport()
        {
            var rezervace = FlexibeeCommunicator.ReceiveFromFlexibee<Nop.Plugin.Widgets.Flexibee.Rezervace.Export.winstrom>("rezervace.xml?detail=full&limit=0").rezervace.Select(r => new { manufacturerPartNumber = r.cenik.Value.Replace("code:", ""), mnozstvi = r.mnozstvi.Value }).GroupBy(a => a.manufacturerPartNumber).Select(g => new { g.Key, mnozstvi = g.Sum(i => i.mnozstvi) }).ToDictionary(p => p.Key, p => (int)p.mnozstvi);

            Assert.IsTrue(rezervace.Count() > 0);

            Nop.Plugin.Widgets.Flexibee.Cenik.Export.winstrom ceniky = FlexibeeCommunicator.ReceiveFromFlexibee<Nop.Plugin.Widgets.Flexibee.Cenik.Export.winstrom>("cenik.xml?detail=custom:sumDostupMj&limit=0");
            foreach (Nop.Plugin.Widgets.Flexibee.Cenik.Export.winstromCenik cenik in ceniky.cenik)
            {
                int? webId = null;
                foreach (var id in cenik.id)
                {
                    if (id.Value.StartsWith("ext:CIN:P"))
                    {
                        webId = Int32.Parse(id.Value.Split(':')[2].Substring(1));
                    }
                }
                if (webId == null) continue;

                int stav = 0;
                if (cenik.skladKarty != null && cenik.skladKarty.skladovakarta != null)
                {
                    foreach (Nop.Plugin.Widgets.Flexibee.Cenik.Export.winstromCenikSkladKartySkladovakarta karta in cenik.skladKarty.skladovakarta)
                    {
                        double stavDouble;
                        if (Double.TryParse(karta.stavMJ.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out stavDouble))
                        {
                            stav += (int)Math.Round(stavDouble);
                        }
                    }
                    UpdateSkladStav(webId.Value, stav, rezervace);
                }
            }
        }



        private static void UpdateSkladStav(int webId, int stavMj, Dictionary<string, int> rezervace)
        {
            int rezervaceMnozstvi = 0;
            rezervace.TryGetValue("sdf", out rezervaceMnozstvi);
            stavMj -= rezervaceMnozstvi;
        }

        [TestMethod]
        public void StornoObjednavky()
        {
            Nop.Plugin.Widgets.Flexibee.OrderExporter.ReservationStorno("CIN", new Core.Domain.Orders.Order() {Id = 3363}, false);
        }

        [TestMethod]
        public void ReservationStornoOld()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebRequest request = WebRequest.Create(String.Format(@"https://{1}/c/michal_kovac/rezervace/(poznamka='Objednávka {0}').xml?detail=custom&limit=0", 3363, FlexibeeCommunicator.FlexibeeServer));
            FlexibeeConfiguration.ConfigureFlexibeeRequest(request, false);
            string xml = "";
            using (WebResponse myWebResponse = request.GetResponse())
            {
                using (var reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    xml = reader.ReadToEnd();
                }
            }

            xml = xml.Replace("<rezervace>", "<rezervace action=\"delete\">");
        }

        /*[TestMethod]
        public void FlexibeeExportTest()
        {
            List<Nop.Plugin.Widgets.Flexibee.ObjednavkaPrijata.Import.winstromObjednavkaprijataPolozkyObchDokladuObjednavkaprijatapolozka> polozky;
            string connectionString = "Data Source=localhost;Initial Catalog=NopCommerce2;Integrated Security=True;Persist Security Info=False;MultipleActiveResultSets=True";
            var nopObjectContext = new NopObjectContext(connectionString);
            var orderRepository = new EfRepository<Order>(nopObjectContext);
            var orderItemRepository = new EfRepository<OrderItem>(nopObjectContext);
            var orderNoteRepository = new EfRepository<OrderNote>(nopObjectContext);
            var productRepository = new EfRepository<Product>(nopObjectContext);
            var recurringPaymentRepository = new EfRepository<RecurringPayment>(nopObjectContext);
            var customerRepository = new EfRepository<Customer>(nopObjectContext);
            var returnRequest = new EfRepository<ReturnRequest>(nopObjectContext);
            var subscribtionService = new SubscriptionService();
            var eventPublisher = new EventPublisher(subscribtionService);
            var orderService = new OrderService(orderRepository, orderItemRepository,
                orderNoteRepository, productRepository, recurringPaymentRepository,
                customerRepository, returnRequest, eventPublisher);
            var order = orderService.GetOrderById(3693);
            var result = Nop.Plugin.Widgets.Flexibee.OrderExporter.GetOrderData("s", "cz",
             "cz", order, order.Customer, 1, out polozky);
            Assert.AreEqual(result.objednavkaprijata[0].polozkyObchDokladu.objednavkaprijatapolozka.Length, 5);
            //OrderExport(string flexibeeExternalIdPrefix, ICountryService countryService, Order order, Nop.Core.Domain.Customers.Customer customer, ITaxService taxService, out polozky);
        }*/
    }
}
