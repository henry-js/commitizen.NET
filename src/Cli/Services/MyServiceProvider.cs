using commitizen.Cli.Commands;
using commitizen.Cli.Services;
using Jab;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raiqub.JabModules.MicrosoftExtensionsOptions;
using MSLogger = Microsoft.Extensions.Logging.LoggerFactory;

namespace Cli.Services;

[ServiceProvider]
[Singleton<ILoggerFactory>(Instance = nameof(LoggerFactory))]
[Singleton(typeof(ILogger<>), Factory = nameof(CreateLogger))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
[Import(typeof(IOptionsModule))]
[Transient<IConfigureOptions<AppConfig>>(Factory = nameof(ConfigureMyConfig))]
[Singleton<MyCommands>]
[Singleton<IConfiguration>(Factory = nameof(CreateConfiguration))]

internal partial class MyServiceProvider
{
    private IConfiguration CreateConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("./config.json", false)
            .Build();
        return configuration;
    }
    public ILoggerFactory LoggerFactory
        => MSLogger.Create(builder => builder.AddConsole()
    );
    private ILogger<T> CreateLogger<T>()
        => LoggerFactory.CreateLogger<T>();

    private static IConfigureOptions<AppConfig> ConfigureMyConfig(IConfiguration configuration)
        => IOptionsModule.Configure<AppConfig>(config => configuration.Bind("Config", config));
}
