using System.Xml.Serialization;

namespace TrxSlackBot.TrxFileModels
{
    public class Execution
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
