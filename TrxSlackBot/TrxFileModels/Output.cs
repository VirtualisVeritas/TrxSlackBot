using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels
{
    public class Output
    {
        [XmlElement("StdOut")]
        public string StdOut { get; set; }

        [XmlElement("stdErr")]
        public string StdErr { get; set; }

        [XmlElement("ErrorInfo")]
        public ErrorInfo ErrorInfo { get; set; }
    }
}
