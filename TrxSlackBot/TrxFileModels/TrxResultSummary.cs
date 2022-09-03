using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels;

public class TrxResultSummary
{
    [XmlAttribute("outcome")]
    public string Outcome { get; set; }

    [XmlElement("Counters")]
    public TrxCounters Counters { get; set; }

    [XmlElement("Output")]
    public TrxOutput Output { get; set; }
}