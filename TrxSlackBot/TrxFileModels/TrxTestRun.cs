using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels;

[XmlRoot(ElementName = "TestRun", Namespace = "")]
public class TrxTestRun
{
    [XmlAttribute("id")]
    public string TestRunId { get; set; }

    [XmlAttribute("runUser")]
    public string TestRunUser { get; set; }

    [XmlAttribute("name")]
    public string TestRunName { get; set; }

    [XmlElement("Times")]
    public TrxTimes TestRunTimes { get; set; }

    [XmlElement("Results")]
    public TrxUnitTestResultList TestRunResults { get; set; }

    [XmlElement("ResultSummary")]
    public TrxResultSummary TestRunResultSummary { get; set; }

    [XmlElement("TestDefinitions")]
    public TrxTestDefinitions TestDefinitions { get; set; }
}