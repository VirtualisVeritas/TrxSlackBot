using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels
{
    public class Results
    {
        [XmlElement("UnitTestResult")]
        public List<UnitTestResult> UnitTestResults { get; set; }
    }
}
