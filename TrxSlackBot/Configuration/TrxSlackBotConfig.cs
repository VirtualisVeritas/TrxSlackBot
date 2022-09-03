﻿namespace TrxSlackBot.Configuration;

public class TrxSlackBotConfig
{
    public string SlackWebhook { get; set; }
    public string? SlackBearerToken { get; set; }
    public string? TrxFile { get; set; }
    public string? DetailsLink { get; set; }
    public static string? ConfigMessageTitle { get; set; }
    public string? MessageTitle => ReceiveMessageTitleFromConfig();
    public bool SendOnlyIfRunHasFails { get; set; }
    public bool SendFailsAsReply { get; set; }
    public string? ChannelId { get; set; }
    public string? ReplyMessageTsId { get; set; }
    public string? ReplyText { get; set; }
    public double WaitSecondsAfterMessageBeforeReply { get; set; }
    public bool SendFailsAsCodeSnipped { get; set; }

    public static TrxSlackBotConfig SlackBotConfigData = TrxSlackBotConfigService.GetTrxSlackBotConfig();

    public string? ReceiveMessageTitleFromConfig()
    {
        var messageTitle = ConfigMessageTitle;
        
        // If Config has no MessageTitle use Trx Filename
        if (messageTitle == null)
        {
            messageTitle = TrxFile;
            var index = messageTitle.IndexOf(".", StringComparison.Ordinal);
            if (index >= 0) messageTitle = messageTitle[..index];
            messageTitle = messageTitle[(messageTitle.LastIndexOf('\\') + 1)..];
        }
        return messageTitle;
    }
}