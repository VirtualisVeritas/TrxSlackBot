using TrxSlackBot.TrxBot;

namespace TrxSlackBot;

public static class Program
{
    public static string? ConfigFile { get; set; }
        
    private static async Task Main(string[] args)
    {
        try
        {
            ConfigFile = args.Length != 0 ? args[0] : Path.Combine(Environment.CurrentDirectory, "trxSlackBotConfig.json");
            await SlackCommunication.SendSlackMessage();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}