using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels
{
    public class TestDefinitions
    {
        [XmlElement("UnitTest")]
        public List<UnitTest> UnitTests { get; set; }
    }
}
