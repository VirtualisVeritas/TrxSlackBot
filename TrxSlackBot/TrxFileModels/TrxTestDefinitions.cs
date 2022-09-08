using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels;

public class TrxTestDefinitions
{
    [XmlElement("UnitTest")]
    public List<TrxUnitTest> UnitTests { get; set; }
}