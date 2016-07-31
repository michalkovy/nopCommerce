using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Serialization;

namespace Nop.Plugin.Widgets.Flexibee
{
    [ServiceContract]
    public interface IFlexibeeHook
    {
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Xml, UriTemplate = "/update", BodyStyle = WebMessageBodyStyle.Bare)]
        [XmlSerializerFormat]
        void Update(WinstromHook changes);
    }

    [DataContract(Name = "winstrom")]
    [XmlSerializerFormat]
    [XmlRoot("winstrom")]
    public class WinstromHook
    {
        [DataMember, XmlAttribute]
        public int globalVersion;

        [XmlElement("objednavka-prijata")]
        public polozka[] objednavkyPrijate;

        [XmlElement("objednavka-prijata-polozka")]
        public polozka[] objednavkyPrijatePolozky;

        [DataMember]
        public string next;
    }

    public class polozka
    {
        [DataMember, XmlAttribute("in-version")]
        public int inVersion;

        [DataMember, XmlAttribute]
        public OperationEnum operation;

        [XmlElement("id")]
        public string[] ids;
    }

    [DataContract(Name = "operation")]
    public enum OperationEnum
    {
        [EnumMember]
        update,
        [EnumMember]
        create,
        [EnumMember]
        delete
    }
}
