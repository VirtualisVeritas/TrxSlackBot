using SlackBotMessages;
using TrxSlackBot.Configuration;

namespace TrxSlackBot.TrxBot;

public static class SlackCommunication
{
    public static async Task<string?> GetLatestSlackMessageTsId(string? channelId, string? bearerToken)
    {
        try
        {
            var getConversationHistory = await SbmClient.GetConversationHistoryAsync(channelId, bearerToken);
            return getConversationHistory.ChannelMessages.FirstOrDefault()?.MessageTsId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task SendSlackMessage()
    {
        try
        {
            var webHookUrl = TrxSlackBotConfig.SlackBotConfigData.SlackWebhook;
            if (string.IsNullOrEmpty(webHookUrl))
            {
                Console.WriteLine("No Slack WebHook in config");
            }

            var testRunData = TrxFileDeserializer.GetTrxTestRunFromConfig();
            var client = new SbmClient(webHookUrl);
            var slackMessage = testRunData.BuildSlackMessageNoFailDetails();

            if (TrxSlackBotConfig.SlackBotConfigData.SendOnlyIfRunHasFails && SlackMessageBuilder.FailedTestCount > 0)
            {
                if (!TrxSlackBotConfig.SlackBotConfigData.SendFailsAsReply && !TrxSlackBotConfig.SlackBotConfigData.SendFailsAsCodeSnipped)
                {
                    slackMessage = testRunData.BuildSlackMessageWithDetails();
                }

                await client.SendAsync(slackMessage);

                if (TrxSlackBotConfig.SlackBotConfigData.SendFailsAsReply && !TrxSlackBotConfig.SlackBotConfigData.SendFailsAsCodeSnipped)
                {
                    var waitSeconds = TrxSlackBotConfig.SlackBotConfigData.WaitSecondsAfterMessageBeforeReply;
                    Thread.Sleep(TimeSpan.FromSeconds(waitSeconds));
                    await SendFailedAsSlackReply();
                }

                if (!TrxSlackBotConfig.SlackBotConfigData.SendFailsAsReply && TrxSlackBotConfig.SlackBotConfigData.SendFailsAsCodeSnipped)
                {
                    await SendFailedAsSlackSnipped();
                }

                if (TrxSlackBotConfig.SlackBotConfigData.SendFailsAsReply && TrxSlackBotConfig.SlackBotConfigData.SendFailsAsCodeSnipped)
                {
                    var waitSeconds = TrxSlackBotConfig.SlackBotConfigData.WaitSecondsAfterMessageBeforeReply;
                    Thread.Sleep(TimeSpan.FromSeconds(waitSeconds));
                    await SendFailedAsSlackReplySnipped();
                }
            }

            if (!TrxSlackBotConfig.SlackBotConfigData.SendOnlyIfRunHasFails)
            {
                if (!TrxSlackBotConfig.SlackBotConfigData.SendFailsAsReply && !TrxSlackBotConfig.SlackBotConfigData.SendFailsAsCodeSnipped && SlackMessageBuilder.FailedTestCount > 0)
                {
                    slackMessage = testRunData.BuildSlackMessageWithDetails();
                }

                await client.SendAsync(slackMessage);

                if (TrxSlackBotConfig.SlackBotConfigData.SendFailsAsReply && !TrxSlackBotConfig.SlackBotConfigData.SendFailsAsCodeSnipped)
                {
                    var waitSeconds = TrxSlackBotConfig.SlackBotConfigData.WaitSecondsAfterMessageBeforeReply;
                    Thread.Sleep(TimeSpan.FromSeconds(waitSeconds));
                    await SendFailedAsSlackReply();
                }

                if (!TrxSlackBotConfig.SlackBotConfigData.SendFailsAsReply && TrxSlackBotConfig.SlackBotConfigData.SendFailsAsCodeSnipped)
                {
                    await SendFailedAsSlackSnipped();
                }

                if (TrxSlackBotConfig.SlackBotConfigData.SendFailsAsReply && TrxSlackBotConfig.SlackBotConfigData.SendFailsAsCodeSnipped)
                {
                    var waitSeconds = TrxSlackBotConfig.SlackBotConfigData.WaitSecondsAfterMessageBeforeReply;
                    Thread.Sleep(TimeSpan.FromSeconds(waitSeconds));
                    await SendFailedAsSlackReplySnipped();
                }
            }

            if (TrxSlackBotConfig.SlackBotConfigData.SendOnlyIfRunHasFails && SlackMessageBuilder.FailedTestCount <= 0)
            {
                // await client.SendAsync(new SlackMessage($"DEBUG - NO FAILED TESTS"));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task SendFailedAsSlackReply()
    {
        try
        {
            var testRunData = TrxFileDeserializer.GetTrxTestRunFromConfig();
            var webHookUrl = TrxSlackBotConfig.SlackBotConfigData.SlackWebhook;
            if (string.IsNullOrEmpty(webHookUrl))
            {
                Console.WriteLine("No Slack WebHook in config");
            }

            var client = new SbmClient(webHookUrl);
            var messageReply = await testRunData.BuildFailedMessageReply();
            await client.SendAsync(messageReply);
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task SendFailedAsSlackSnipped()
    {
        try
        {
            var slackBearerToken = TrxSlackBotConfig.SlackBotConfigData.SlackBearerToken;
            if (string.IsNullOrEmpty(slackBearerToken))
            {
                Console.WriteLine("No BearerToken in config");
            }
            var fileUploadMessage = SlackMessageBuilder.BuildCodeSnippedMessage();
            await SbmClient.SendFileUploadAsync(slackBearerToken, fileUploadMessage);
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task SendFailedAsSlackReplySnipped()
    {
        try
        {
            var slackBearerToken = TrxSlackBotConfig.SlackBotConfigData.SlackBearerToken;
            if (string.IsNullOrEmpty(slackBearerToken))
            {
                Console.WriteLine("No BearerToken in config");
            }
            var fileUploadMessage = await SlackMessageBuilder.BuildCodeSnippedReplyMessage();
            await SbmClient.SendFileUploadAsync(slackBearerToken, fileUploadMessage);
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}