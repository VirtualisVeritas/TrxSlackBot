using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using SlackBotMessages;
using SlackBotMessages.Models;
using TrxSlackBot.Configuration;
using TrxSlackBot.TrxFileModels;
using File = System.IO.File;

namespace TrxSlackBot;

public static class TrxSlackDeserializer
{
    private static readonly SlackAndTrxConfig SlackAndTrxConfig = SlackAndTrxConfigService.GetSlackAndTrxConfig();

    public static TestRun Deserialize(string filePath)
    {
        try
        {
            RemoveXmlnsAndRewriteFile(filePath);
            var xs = new XmlSerializer(typeof(TestRun));
            using Stream sr = File.OpenRead(filePath);
            return (TestRun)xs.Deserialize(sr)!;
        }
        catch (Exception e)
        {
            if (filePath.Equals(""))
            {
                Console.WriteLine("Empty TRX FilePath in Config");
            }
            else
            {
                Console.WriteLine(e);
            }
            throw;
        }
    }

    public static TestRun DeserializeFromConfig() => Deserialize(SlackAndTrxConfig.TrxFile);

    private static void RemoveXmlnsAndRewriteFile(string filePath)
    {
        var rgx = new Regex("xmlns=\".*?\" ?");
        var fileContent = rgx.Replace(File.ReadAllText(filePath), string.Empty);
        var xDoc = XDocument.Parse(fileContent);
        xDoc.Save(filePath);
    }

    public static string GetDuration(this TestRun testRun)
    {
        var start = DateTime.Parse(testRun.Times.Start);
        var finish = DateTime.Parse(testRun.Times.Finish);
        var duration = (decimal)finish.Subtract(start).TotalMinutes;
        var durationRounded = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);
        return durationRounded.ToString(CultureInfo.CurrentCulture);
    }

    public static decimal GetPercentPassed(this TestRun testRun)
    {
        var testCounters = testRun.ResultSummary.Counters;
        var testPercent = (int)((double)testCounters.Passed / testCounters.Executed * 100);
        return testPercent;
    }

    public static List<string> GetErrorMessagesOfFailedTest(this TestRun testRun)
    {
        return testRun.Results.UnitTestResults
            .Where(x => x.Outcome.Equals("Failed"))
            .Select(text => text.Output.ErrorInfo.Message).ToList();
    }

    public static Dictionary<string, string> GetFailedTestNameAndError(this TestRun testRun)
    {
        var allFailedData = 
            testRun.Results.UnitTestResults.Where(x => x.Outcome.Equals("Failed"));

        return allFailedData.ToDictionary(dataSet => 
            dataSet.TestName, dataSet => dataSet.Output.ErrorInfo.Message);
    }

    public static string GetTestNameAndErrorMessage(this TestRun testRun)
    {
        var failedError = testRun.GetFailedTestNameAndError();
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
                myStringBuilder.Append(";");
            }

            myStringBuilder.AppendFormat($"Failed Test Name: {pair.Key} Failed Test Error {pair.Value}");
        }

        return myStringBuilder.ToString();
    }

    public static string ReceiveSlackMoodColor(this TestRun testRun)
    {
        var percentPassed = testRun.GetPercentPassed();
        return percentPassed switch
        {
            100 => "good",
            > 65 and < 100 => "warning",
            <= 65 => "danger"
        };
    }

    public static Message BuildSlackMessage(this TestRun testRun)
    {
        var testNameAndFails = testRun.GetFailedTestNameAndError();
        var testCounters = testRun.ResultSummary.Counters;
        var trxFilePath = SlackAndTrxConfig.TrxFile;
        var index = trxFilePath.IndexOf(".", StringComparison.Ordinal);
        if (index >= 0)
            trxFilePath = trxFilePath[..index];

        trxFilePath = trxFilePath[(trxFilePath.LastIndexOf('\\') + 1)..];
        var message = new Message(trxFilePath)
        {
            Attachments = new List<Attachment>
            {
                new Attachment
                {
                    Color = ReceiveSlackMoodColor(testRun),
                    Fields = new List<Field>
                    {
                        new Field
                        {
                            Title = "Test Report",
                            Value = $"Test Reporting {SlackAndTrxConfig.DetailsLink}",
                            Short = false
                        },
                        new Field
                        {
                            Title = "Total Tests: " + testCounters.Total +
                                    $" -  {testRun.GetPercentPassed()}%",
                            Value = "",
                            Short = true
                        },
                        new Field
                        {
                            Title = ":screwdriver:" + " Skipped: " + $"{testCounters.Skipped()}",
                            Value = "",
                            Short = true
                        },
                        new Field
                        {
                            Title = Emoji.WhiteCheckMark + " Passed: " + $"{testCounters.Passed}",
                            Value = "",
                            Short = true
                        },
                        new Field
                        {
                            Title = Emoji.Warning + " Failed: " + $" {testCounters.Failed}",
                            Value = "",
                            Short = true
                        },
                        new Field
                        {
                            Title = Emoji.AlarmClock + " Duration: " + $"{GetDuration(testRun)} Minutes",
                            Value = "",
                            Short = true
                        },

                        new Field
                        {
                            Title = Emoji.X + " Failed Test Details:\n\n\n",
                            Value = string.Join("\n\n",
                                testNameAndFails.Select(x =>
                                        $"*Test {x.Key}*" + "\n\n*Error Message:* " + $"```{x.Value}```" + " ")
                                    .ToArray()),
                            Short = true
                        }
                    }
                }
            }
        };
        return message;
    }

    public static async Task SendSlackMessage()
    {
        try
        {
            var webHookUrl = SlackAndTrxConfig.SlackWebhook;
            if (webHookUrl == null || webHookUrl == "")
            {
                Console.WriteLine("No Slack WebHook in config");
            }
            var testRunData = DeserializeFromConfig();
            var client = new SbmClient(webHookUrl);
            var slackMessage = testRunData.BuildSlackMessage();
            await client.SendAsync(slackMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}