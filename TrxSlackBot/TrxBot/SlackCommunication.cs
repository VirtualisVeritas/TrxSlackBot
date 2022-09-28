using SlackBotMessages;
using TrxSlackBot.Configuration;

namespace TrxSlackBot.TrxBot;

// ToDo: Restructure Methods with Parameters instead of Separate Methods that all do the same

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
            var slackBotConfig = TrxSlackBotConfig.SlackBotConfigData;
            var messageTsId = slackBotConfig.ReplyMessageTsId;

            if (slackBotConfig.SendOnlyIfRunHasFails && SlackMessageBuilder.FailedTestCount > 0)
            {
                if (!slackBotConfig.SendFailsAsReply && !slackBotConfig.SendFailsAsCodeSnipped)
                {
                    slackMessage = testRunData.BuildSlackMessageWithDetails();
                }

                await client.SendAsync(slackMessage);

                if (slackBotConfig.SendDetailedMessageAsReply && !slackBotConfig.SendFailsAsReply && !slackBotConfig.SendFailsAsCodeSnipped)
                {
                    if (string.IsNullOrEmpty(messageTsId))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(slackBotConfig.SendDetailedMessageAsReplyWaitSecondsForMessage));
                        messageTsId = await GetLatestSlackMessageTsId(slackBotConfig.ChannelId, slackBotConfig.SlackBearerToken);
                    }
                    slackMessage.ThreadReply = messageTsId;
                    
                    if (!string.IsNullOrEmpty(slackBotConfig.HealthCheckWebhook))
                    {
                        var healthCheckClient = new SbmClient(slackBotConfig.HealthCheckWebhook);
                        await healthCheckClient.SendAsync(slackMessage);
                    }
                }
                
                if (slackBotConfig.SendFailsAsReply && !slackBotConfig.SendFailsAsCodeSnipped && !slackBotConfig.SendDetailedMessageAsReply)
                {
                    var waitSeconds = slackBotConfig.WaitSecondsAfterMessageBeforeReply;
                    Thread.Sleep(TimeSpan.FromSeconds(waitSeconds));
                    await SendFailedAsSlackReply();
                }

                if (!slackBotConfig.SendFailsAsReply && slackBotConfig.SendFailsAsCodeSnipped && !slackBotConfig.SendDetailedMessageAsReply)
                {
                    await SendFailedAsSlackSnipped();
                }

                if (slackBotConfig.SendFailsAsReply && slackBotConfig.SendFailsAsCodeSnipped && !slackBotConfig.SendDetailedMessageAsReply)
                {
                    var waitSeconds = slackBotConfig.WaitSecondsAfterMessageBeforeReply;
                    Thread.Sleep(TimeSpan.FromSeconds(waitSeconds));
                    await SendFailedAsSlackReplySnipped();
                }
            }

            if (!slackBotConfig.SendOnlyIfRunHasFails)
            {
                if (!slackBotConfig.SendFailsAsReply && !slackBotConfig.SendFailsAsCodeSnipped && SlackMessageBuilder.FailedTestCount > 0)
                {
                    slackMessage = testRunData.BuildSlackMessageWithDetails();
                }

                if (slackBotConfig.SendDetailedMessageAsReply)
                {
                    if (string.IsNullOrEmpty(messageTsId))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(slackBotConfig.SendDetailedMessageAsReplyWaitSecondsForMessage));
                        messageTsId = await GetLatestSlackMessageTsId(slackBotConfig.ChannelId, slackBotConfig.SlackBearerToken);
                    }
                    slackMessage.ThreadReply = messageTsId;
                }

                await client.SendAsync(slackMessage);

                if (slackBotConfig.SendFailsAsReply && !slackBotConfig.SendFailsAsCodeSnipped)
                {
                    var waitSeconds = slackBotConfig.WaitSecondsAfterMessageBeforeReply;
                    Thread.Sleep(TimeSpan.FromSeconds(waitSeconds));
                    await SendFailedAsSlackReply();
                }

                if (!slackBotConfig.SendFailsAsReply && slackBotConfig.SendFailsAsCodeSnipped)
                {
                    await SendFailedAsSlackSnipped();
                }

                if (slackBotConfig.SendFailsAsReply && slackBotConfig.SendFailsAsCodeSnipped)
                {
                    var waitSeconds = slackBotConfig.WaitSecondsAfterMessageBeforeReply;
                    Thread.Sleep(TimeSpan.FromSeconds(waitSeconds));
                    await SendFailedAsSlackReplySnipped();
                }
            }

            if (slackBotConfig.SendOnlyIfRunHasFails && !slackBotConfig.SendDetailedMessageAsReply && SlackMessageBuilder.FailedTestCount <= 0)
            {
                var healthCheckClient = new SbmClient(slackBotConfig.HealthCheckWebhook);
                await healthCheckClient.SendAsync(slackMessage);
            }

            if (slackBotConfig.SendOnlyIfRunHasFails && slackBotConfig.SendDetailedMessageAsReply && SlackMessageBuilder.FailedTestCount <= 0)
            {
                if (string.IsNullOrEmpty(messageTsId))
                {
                    Thread.Sleep(TimeSpan.FromSeconds(slackBotConfig.SendDetailedMessageAsReplyWaitSecondsForMessage));
                    messageTsId = await GetLatestSlackMessageTsId(slackBotConfig.ChannelId, slackBotConfig.SlackBearerToken);
                }
                slackMessage.ThreadReply = messageTsId;

                if (!string.IsNullOrEmpty(slackBotConfig.HealthCheckWebhook))
                {
                    var healthCheckClient = new SbmClient(slackBotConfig.HealthCheckWebhook);
                    await healthCheckClient.SendAsync(slackMessage);
                }
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

    public static async Task SendDetailedMessageAsSlackReply()
    {
        try
        {
            var testRunData = TrxFileDeserializer.GetTrxTestRunFromConfig();
            var slackBotConfig = TrxSlackBotConfig.SlackBotConfigData;
            var messageTsId = slackBotConfig.ReplyMessageTsId;
            var webHookUrl = slackBotConfig.SlackWebhook;
            var slackMessage = testRunData.BuildSlackMessageWithDetails();
            
            if (string.IsNullOrEmpty(webHookUrl))
            {
                Console.WriteLine("No Slack WebHook in config");
            }
            
            if (string.IsNullOrEmpty(messageTsId))
            {
                Thread.Sleep(TimeSpan.FromSeconds(slackBotConfig.SendDetailedMessageAsReplyWaitSecondsForMessage));
                messageTsId = await GetLatestSlackMessageTsId(slackBotConfig.ChannelId, slackBotConfig.SlackBearerToken);
            }
            slackMessage.ThreadReply = messageTsId;
            await new SbmClient(webHookUrl).SendAsync(slackMessage);
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}