using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels;

public class TrxTimes
{
    [XmlAttribute(AttributeName = "creation")]
    public string Creation { get; set; }

    [XmlAttribute(AttributeName = "queuing")]
    public string Queuing { get; set; }

    [XmlAttribute(AttributeName = "start")]
    public string Start { get; set; }

    [XmlAttribute(AttributeName = "finish")]
    public string Finish { get; set; }
}