using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels
{
    public class ResultSummary
    {
        [XmlAttribute("outcome")]
        public string Outcome { get; set; }

        [XmlElement("Counters")]
        public Counters Counters { get; set; }

        [XmlElement("Output")]
        public Output Output { get; set; }
    }
}
