using Microsoft.Extensions.Configuration;

namespace TrxSlackBot.Configuration;

public class TrxSlackBotConfigService
{
    private static readonly IConfigurationRoot ConfigurationRoot;
    public static string? ConfigPath { get; set; }

    static TrxSlackBotConfigService() => ConfigurationRoot = InitializeConfiguration();

    public static TrxSlackBotConfig GetTrxSlackBotConfig()
    {
        const string configName = nameof(TrxSlackBotConfig);
        return ConfigurationRoot.GetSection(configName).Get<TrxSlackBotConfig>();
    }

    public static void SetConfigPath()
    {
        ConfigPath = Program.ConfigFile;
    }

    private static IConfigurationRoot InitializeConfiguration()
    {
        SetConfigPath();
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile(Program.ConfigFile);
        builder.AddEnvironmentVariables();
        return builder.Build();
    }
}