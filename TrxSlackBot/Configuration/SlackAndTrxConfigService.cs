using Microsoft.Extensions.Configuration;

namespace TrxSlackBot.Configuration;

public sealed class SlackAndTrxConfigService
{
    private static readonly IConfigurationRoot ConfigurationRoot;

    static SlackAndTrxConfigService() => ConfigurationRoot = InitializeConfiguration();

    public static SlackAndTrxConfig GetSlackAndTrxConfig()
    {
        var configName = typeof(SlackAndTrxConfig).Name;
        return ConfigurationRoot.GetSection(configName).Get<SlackAndTrxConfig>();
    }

    private static IConfigurationRoot InitializeConfiguration()
    {
        var builder = new ConfigurationBuilder();
        var configFileName = "slackAndTrxConfig.json";
        var configPath = Path.Combine(Environment.CurrentDirectory, configFileName);
        builder.AddJsonFile(configPath);
        builder.AddEnvironmentVariables();
        return builder.Build();
    }
}