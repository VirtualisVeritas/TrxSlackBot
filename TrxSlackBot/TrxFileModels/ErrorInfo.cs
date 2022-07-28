using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels
{
    public class ErrorInfo
    {
        [XmlElement("Message")]
        public string Message { get; set; }

        [XmlElement("StackTrace")]
        public string StackTrace { get; set; }
    }
}
