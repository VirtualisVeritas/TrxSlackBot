using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels;

public class TrxUnitTest
{
    [XmlAttribute("id")]
    public string UnitTestId { get; set; }

    [XmlAttribute("name")]
    public string UnitTestName { get; set; }

    [XmlAttribute("storage")]
    public string UnitTestStorage { get; set; }

    [XmlElement("Execution")]
    public TrxExecution Execution { get; set; }

    [XmlElement("TestMethod")]
    public TrxTestMethod TestMethod { get; set; }
}