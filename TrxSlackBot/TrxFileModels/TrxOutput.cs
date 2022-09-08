using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels;

public class TrxOutput
{
    [XmlElement("StdOut")]
    public string StdOut { get; set; }

    [XmlElement("stdErr")]
    public string StdErr { get; set; }

    [XmlElement("ErrorInfo")]
    public TrxErrorInfo TrxErrorInfo { get; set; }
}