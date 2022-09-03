using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels;

public class TrxUnitTestResultList
{
    [XmlElement("UnitTestResult")]
    public List<TrxUnitTestResult> UnitTestResults { get; set; }
}