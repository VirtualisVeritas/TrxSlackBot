using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels;

public class TrxExecution
{
    [XmlAttribute("id")]
    public string ExecutionId { get; set; }
}