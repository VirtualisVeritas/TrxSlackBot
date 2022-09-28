using System.Globalization;
using System.Text;
using SlackBotMessages;
using SlackBotMessages.Models;
using TrxSlackBot.Configuration;
using TrxSlackBot.TrxFileModels;

namespace TrxSlackBot.TrxBot;

// ToDo: Restructure Methods with Parameters instead of Separate Methods that all do the same

public static class SlackMessageBuilder
{
    public static int FailedTestCount { get; set; }
    private static readonly TrxSlackBotConfig SlackAndTrxConfig = TrxSlackBotConfigService.GetTrxSlackBotConfig();
    
    public static string GetDuration(this TrxTestRun trxTestRun)
    {
        var start = DateTime.Parse(trxTestRun.TestRunTimes.Start);
        var finish = DateTime.Parse(trxTestRun.TestRunTimes.Finish);
        var duration = (decimal)finish.Subtract(start).TotalMinutes;
        var durationRounded = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);
        return durationRounded.ToString(CultureInfo.CurrentCulture);
    }

    public static decimal GetPercentPassed(this TrxTestRun trxTestRun)
    {
        var testCounters = trxTestRun.TestRunResultSummary.Counters;
        var testPercent = (int)((double)testCounters.Passed / testCounters.Executed * 100);
        return testPercent;
    }

    public static List<string> GetErrorMessagesOfFailedTest(this TrxTestRun trxTestRun)
    {
        return trxTestRun.TestRunResults.UnitTestResults
            .Where(x => x.Outcome.Equals("Failed"))
            .Select(text => text.Output.TrxErrorInfo.ErrorMessage).ToList();
    }

    public static Dictionary<string, string> GetFailedTestNameAndError(this TrxTestRun trxTestRun)
    {
        var allFailedData = 
            trxTestRun.TestRunResults.UnitTestResults.Where(x => x.Outcome.Equals("Failed"));

        return allFailedData.ToDictionary(
            testResult => testResult.TestName, 
            testResult => testResult.Output.TrxErrorInfo.ErrorMessage);
    }

    public static string GetTestNameAndErrorMessage(this TrxTestRun trxTestRun)
    {
        var failedError = trxTestRun.GetFailedTestNameAndError();
        var first = true;
        var myStringBuilder = new StringBuilder();
        foreach (var pair in failedError)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                myStringBuilder.Append(';');
            }

            myStringBuilder.AppendFormat($"Failed Test Name: {pair.Key} Failed Test Error {pair.Value}");
        }

        return myStringBuilder.ToString();
    }

    public static string ReceiveSlackMoodColor(this TrxTestRun trxTestRun)
    {
        var percentPassed = trxTestRun.GetPercentPassed();
        return percentPassed switch
        {
            100 => "good",
            > 65 and < 100 => "warning",
            <= 65 => "danger"
        };
    }

    public static string? ReceiveMessageTitle()
    {
        var messageTitle = SlackAndTrxConfig.MessageTitle;
        if (string.IsNullOrEmpty(messageTitle))
        {
            messageTitle = SlackAndTrxConfig.TrxFile;
            var index = messageTitle.IndexOf(".", StringComparison.Ordinal);
            if (index >= 0) messageTitle = messageTitle[..index];
            messageTitle = messageTitle[(messageTitle.LastIndexOf('\\') + 1)..];
        }
        return messageTitle;
    }

    public static SlackMessage BuildSlackMessageNoFailDetails(this TrxTestRun trxTestRun)
    {
        var testCounters = trxTestRun.TestRunResultSummary.Counters;
        FailedTestCount = testCounters.Failed;
        var message = new SlackMessage($"{ReceiveMessageTitle()}")
        {
            Attachments = new List<Attachment>
            {
                new Attachment
                {
                    Color = ReceiveSlackMoodColor(trxTestRun),
                    Fields = new List<Field>
                    {
                        new Field
                        {
                            Title = $"✨ Test Run Overview (Tests Available: {testCounters.Total}) ✨",
                            Value = $"*{testCounters.Executed}* Tests executed within *{GetDuration(trxTestRun)}* Minutes",
                            Short = false
                        },
                        new Field
                        {
                            Title = $"✅ Passed: *{testCounters.Passed}* ({trxTestRun.GetPercentPassed()}%)",
                            Short = false
                        },
                        new Field
                        {
                            Title = $"🪛 Skipped: *{testCounters.Skipped()}*",
                            Short = false
                        },
                        new Field
                        {
                            Title = $"❌ Failed: *{testCounters.Failed}*"
                        },
                        new Field
                        {
                            Title = "For more test run details go to:",
                            Value = $"{SlackAndTrxConfig.DetailsLink}",
                            Short = false
                        }
                    }
                }
            }
        };
        return message;
    }

    public static SlackMessage BuildSlackMessageWithDetails(this TrxTestRun trxTestRun)
    {
        var testNameAndFails = trxTestRun.GetFailedTestNameAndError();
        var testCounters = trxTestRun.TestRunResultSummary.Counters;
        FailedTestCount = testCounters.Failed;
        var message = new SlackMessage($"{ReceiveMessageTitle()}")
        {
            Attachments = new List<Attachment>
            {
                new Attachment
                {
                    Color = ReceiveSlackMoodColor(trxTestRun),
                    Fields = new List<Field>
                    {
                        new Field
                        {
                            Title = $"✨ Test Run Overview (Tests Available: {testCounters.Total}) ✨",
                            Value = $"*{testCounters.Executed}* Tests executed within *{GetDuration(trxTestRun)}* Minutes",
                            Short = false
                        },
                        new Field
                        {
                            Title = $"✅ Passed: *{testCounters.Passed}* ({trxTestRun.GetPercentPassed()}%)",
                            Short = false
                        },
                        new Field
                        {
                            Title = $"🪛 Skipped: *{testCounters.Skipped()}*",
                            Short = false
                        },
                        new Field
                        {
                            Title = $"❌ Failed: *{testCounters.Failed}*"
                        },
                        new Field
                        {
                            Title = "Failed Test Details:",
                            Value = string.Join("", testNameAndFails.Select(x => $"_Test Name:_ *{x.Key}* \n ```{x.Value}``` ").ToArray()),
                            Short = false
                        },
                        new Field
                        {
                            Title = "For more test run details go to:",
                            Value = $"{SlackAndTrxConfig.DetailsLink}",
                            Short = false
                        },
                    }
                }
            }
        };
        return message;
    }

    public static MessageReply BuildSimpleMessageReply()
    {
        var messageReply = new MessageReply($"{ReceiveMessageTitle()}")
        {
            ChannelId = SlackAndTrxConfig.ChannelId,
            ParentMessageTsId = SlackAndTrxConfig.ReplyMessageTsId,
            ReplyText = SlackAndTrxConfig.ReplyText
        };
        return messageReply;
    }

    public static async Task<SlackMessage> BuildFailedMessageReply(this TrxTestRun trxTestRun)
    {
        var testNameAndFails = trxTestRun.GetFailedTestNameAndError();
        var messageTsId = SlackAndTrxConfig.ReplyMessageTsId;
        if (string.IsNullOrEmpty(messageTsId))
        {
            messageTsId = await GetLatestSlackMessageTsId(SlackAndTrxConfig.ChannelId, SlackAndTrxConfig.SlackBearerToken);
        }
        
        var slackFailed = new SlackMessage("Failed Test Details")
        {
            ThreadReply = messageTsId,
            Attachments = new List<Attachment>
            {
                new Attachment
                {
                    Color = ReceiveSlackMoodColor(trxTestRun),
                    Fields = new List<Field>
                    {
                        new Field
                        {
                            Title = "Failed Test Details:",
                            Value = string.Join("", testNameAndFails.Select(x => $"_Test Name:_ {x.Key} \n ```{x.Value}``` ").ToArray()),
                            Short = false
                        }
                    }
                }
            }
        };
        return slackFailed;
    }

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
    
    public static string BuildFailedTestString(this TrxTestRun trxTestRun) => 
        string.Join("", trxTestRun.GetFailedTestNameAndError()
            .Select(x => $"{x.Key} \n{x.Value}").ToArray());

    public static KeyValuePair<string, string>[] BuildCodeSnippedMessage()
    {
        var testRunData = TrxFileDeserializer.GetTrxTestRunFromConfig();
        var fileType = SlackAndTrxConfig.SnippedSlackFileType;

        if (string.IsNullOrEmpty(fileType))
        {
            fileType = "json";
        }

        return new SlackFiles
        {
            SlackToken = SlackAndTrxConfig.SlackBearerToken,
            InitialComment = SlackAndTrxConfig.SnippedInitialComment,
            SlackChannels = SlackAndTrxConfig.ChannelId,
            SlackFileContent = testRunData.BuildFailedTestString(),
            SlackFileType = fileType,
            SlackFileName = SlackAndTrxConfig.SnippedSlackFileName,
            SlackFilePostTitle = SlackAndTrxConfig.SnippedSlackFilePostTitle,
            SlackFile = "" // either this or content
        }.GenerateSlackFile();
    }

    public static async Task<KeyValuePair<string, string>[]> BuildCodeSnippedReplyMessage()
    {
        var testRunData = TrxFileDeserializer.GetTrxTestRunFromConfig();
        var messageTsId = SlackAndTrxConfig.ReplyMessageTsId;
        var fileType = SlackAndTrxConfig.SnippedSlackFileType;

        if (string.IsNullOrEmpty(fileType))
        {
            fileType = "json";
        }

        if (string.IsNullOrEmpty(messageTsId))
        {
            messageTsId = await GetLatestSlackMessageTsId(SlackAndTrxConfig.ChannelId, SlackAndTrxConfig.SlackBearerToken);
        }

        return new SlackFiles
        {
            SlackToken = SlackAndTrxConfig.SlackBearerToken,
            InitialComment = SlackAndTrxConfig.SnippedInitialComment,
            SlackChannels = SlackAndTrxConfig.ChannelId,
            SlackFileContent = testRunData.BuildFailedTestString(),
            SlackFileType = fileType,
            SlackFileName = SlackAndTrxConfig.SnippedSlackFileName,
            SlackFilePostTitle = SlackAndTrxConfig.SnippedSlackFilePostTitle,
            ThreadTs = messageTsId,
            SlackFile = "" // either this or content
        }.GenerateSlackFile();
    }
}