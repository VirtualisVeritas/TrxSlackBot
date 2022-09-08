using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using TrxSlackBot.Configuration;
using TrxSlackBot.TrxFileModels;
using File = System.IO.File;

namespace TrxSlackBot.TrxBot;

public static class TrxFileDeserializer
{
    private static readonly TrxSlackBotConfig TrxSlackBotConfig = TrxSlackBotConfigService.GetTrxSlackBotConfig();
    
    private static void RemoveXmlnsAndRewriteFile(string? filePath)
    {
        try
        {
            var rgx = new Regex("xmlns=\".*?\" ?");
            var fileContent = rgx.Replace(File.ReadAllText(filePath), string.Empty);
            var xDoc = XDocument.Parse(fileContent);
            xDoc.Save(filePath);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static TrxTestRun DeserializeTrxFile(string? trxFilePath)
    {
        try
        {
            RemoveXmlnsAndRewriteFile(trxFilePath);
            var xmlSerializer = new XmlSerializer(typeof(TrxTestRun));
            using Stream sr = File.OpenRead(trxFilePath!);
            return (TrxTestRun)xmlSerializer.Deserialize(sr)!;
        }
        catch (Exception e)
        {
            if (trxFilePath == null || trxFilePath.Equals(""))
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

    public static TrxTestRun GetTrxTestRunFromConfig() => DeserializeTrxFile(TrxSlackBotConfig.TrxFile);

    public static string GetDurationFromTrx(this TrxTestRun trxTestRun)
    {
        var start = DateTime.Parse(trxTestRun.TestRunTimes.Start);
        var finish = DateTime.Parse(trxTestRun.TestRunTimes.Finish);
        var duration = (decimal)finish.Subtract(start).TotalMinutes;
        var durationRounded = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);
        return durationRounded.ToString(CultureInfo.CurrentCulture);
    }

    public static decimal GetPercentPassedFromTrx(this TrxTestRun trxTestRun)
    {
        var testCounters = trxTestRun.TestRunResultSummary.Counters;
        var testPercent = (int)((double)testCounters.Passed / testCounters.Executed * 100);
        return testPercent;
    }

    public static List<string> GetErrorMessagesOfFailedTestFromTrx(this TrxTestRun trxTestRun)
    {
        return trxTestRun.TestRunResults.UnitTestResults
            .Where(x => x.Outcome.Equals("Failed"))
            .Select(text => text.Output.TrxErrorInfo.ErrorMessage).ToList();
    }

    public static Dictionary<string, string> GetFailedTestNameAndErrorFromTrx(this TrxTestRun trxTestRun)
    {
        var allFailedData = 
            trxTestRun.TestRunResults.UnitTestResults.Where(x => x.Outcome.Equals("Failed"));

        return allFailedData.ToDictionary(
            testResult => testResult.TestName, 
            testResult => testResult.Output.TrxErrorInfo.ErrorMessage);
    }

    public static string GetTestNameAndErrorMessageFromTrx(this TrxTestRun trxTestRun)
    {
        var failedError = trxTestRun.GetFailedTestNameAndErrorFromTrx();
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

    public static string ReceiveSlackMoodColorFromTrx(this TrxTestRun trxTestRun)
    {
        var percentPassed = trxTestRun.GetPercentPassed();
        return percentPassed switch
        {
            100 => "good",
            > 65 and < 100 => "warning",
            <= 65 => "danger"
        };
    }
}