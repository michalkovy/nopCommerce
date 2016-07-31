using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Nop.MichalKovac.Tests
{
    /// <summary>
    /// Summary description for FlexibeeHookTest
    /// </summary>
    [TestClass]
    public class FlexibeeHookTest
    {
        [TestMethod]
        public void HookSerialization()
        {
            Nop.Plugin.Widgets.Flexibee.WinstromHook hook = new Plugin.Widgets.Flexibee.WinstromHook();
            hook.next = "None";
            hook.objednavkyPrijate = new Plugin.Widgets.Flexibee.polozka[] { new Plugin.Widgets.Flexibee.polozka() { inVersion = 6, operation = Plugin.Widgets.Flexibee.OperationEnum.update, ids = new string[] { "as", "123" } } };
            string text = ContractObjectToXml(hook);
        }

        public static string ContractObjectToXml<T>(T obj)
        {
            XmlSerializer dataContractSerializer = new XmlSerializer(obj.GetType());
            String text;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                dataContractSerializer.Serialize(memoryStream, obj);
                byte[] data = new byte[memoryStream.Length];
                Array.Copy(memoryStream.GetBuffer(), data, data.Length);
                text = Encoding.UTF8.GetString(data);
            }

            return text;
        }
    }
}
