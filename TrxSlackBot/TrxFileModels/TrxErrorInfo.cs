using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels;

public class TrxErrorInfo
{
    [XmlElement("Message")]
    public string ErrorMessage { get; set; }

    [XmlElement("StackTrace")]
    public string ErrorInfoStackTrace { get; set; }
}