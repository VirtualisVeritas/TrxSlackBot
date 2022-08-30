namespace TrxSlackBot.Configuration;

public sealed class SlackAndTrxConfig
{
    public string SlackWebhook { get; set; }
    public string TrxFile { get; set; }
    public string DetailsLink { get; set; }
    public string MessageTitle { get; set; }
}