using Microsoft.Extensions.Configuration;

namespace TrxSlackBot.Configuration;

public sealed class SlackAndTrxConfigService
{
    private static readonly IConfigurationRoot ConfigurationRoot;
    public static string? ConfigPath { get; set; }

    static SlackAndTrxConfigService() => ConfigurationRoot = InitializeConfiguration();

    public static SlackAndTrxConfig GetSlackAndTrxConfig()
    {
        const string configName = nameof(SlackAndTrxConfig);
        return ConfigurationRoot.GetSection(configName).Get<SlackAndTrxConfig>();
    }

    public static void SetConfigPath()
    {
        ConfigPath = Program.ConfigFile;
    }

    private static IConfigurationRoot InitializeConfiguration()
    {
        SetConfigPath();
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile(ConfigPath);
        builder.AddEnvironmentVariables();
        return builder.Build();
    }
}