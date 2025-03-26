using System.Text.Json;
using Cli;
using commitizen.Cli.Services;
using ConsoleAppFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace commitizen.Cli.Commands;

public class MyCommands([FromServices] ILogger<MyCommands> logger, IService service, IOptions<AppConfig> options, IConfiguration configuration)
{
    private readonly AppConfig config = options.Value;
    private readonly IConfiguration configuration = configuration;

    /// <summary>Root command test.</summary>
    /// <param name="msg">-m, Message to show.</param>
    [Command("")]
    public void Root(string msg)
    {
        logger.LogInformation($"Hello from logger");
        service.DoSomething();
        Console.WriteLine(msg);
    }

    /// <summary>Display message.</summary>
    /// <param name="msg">Message to show.</param>
    public void Echo(string msg) => Console.WriteLine(msg);

    /// <summary>Sum parameters.</summary>
    /// <param name="x">left value.</param>
    /// <param name="y">right value.</param>
    public void Sum(int x, int y) => Console.WriteLine(x + y);

    [Command("config")]
    public void Config()
    {
        var opts = options;
        logger.LogInformation("Displaying config");

        var text = JsonSerializer.Serialize(config, typeof(AppConfig), AppConfigContext.Default);

        Console.WriteLine(text);
    }
}