using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Nop.Plugin.Widgets.Flexibee
{
    public static class FlexibeeCommunicator
    {
        public static string FlexibeeServer { get; set; } = "localhost:5434";
        public static string SendToFlexibee<T>(T data, string flexibeeType, string parameters = ".xml")
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string serializedData;
            using (StringWriter writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, data);
                serializedData = writer.ToString();
            }
            return SendXmlToFlexibee(serializedData, flexibeeType, parameters);
        }

        public static string SendXmlToFlexibee(string xmlData, string flexibeeType, string parameters = ".xml", bool writeToFile = true)
        {
            string returnedData = "";
            try
            {
                if (xmlData != null && writeToFile)
                {
                    string filePath = string.Format("{0}content\\files\\exportimport\\FlexibeeLast{1}Import.xml", HttpRuntime.AppDomainAppPath, flexibeeType);
                    File.WriteAllText(filePath, xmlData, Encoding.UTF8);
                }

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                WebRequest request = WebRequest.Create(String.Format(@"https://{0}/c/michal_kovac/{1}{2}", FlexibeeServer, flexibeeType, parameters));
                FlexibeeConfiguration.ConfigureFlexibeeRequest(request, xmlData != null);
                if (xmlData != null)
                {
                    using (var writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8))
                    {
                        writer.Write(xmlData);
                    }
                }
                using (WebResponse myWebResponse = request.GetResponse())
                {
                    using (var reader = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        returnedData = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (var reader = new StreamReader(e.Response.GetResponseStream(), Encoding.UTF8))
                    {
                        returnedData = reader.ReadToEnd();
                    }
                }
                else
                {
                    returnedData = e.ToString();
                }
            }
            return returnedData;
        }

        public static T ReceiveFromFlexibee<T>(string subquery)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebRequest request = WebRequest.Create(String.Format(@"https://{0}/c/michal_kovac/{1}", FlexibeeServer, subquery));
            FlexibeeConfiguration.ConfigureFlexibeeRequest(request, false);
            using (WebResponse myWebResponse = request.GetResponse())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (XmlTextReader xmlReader = new XmlTextReader(myWebResponse.GetResponseStream()))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
