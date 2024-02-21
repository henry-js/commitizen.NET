using commitizen.NET.Cli;
using commitizen.NET.Lib;
using Community.Extensions.Spectre.Cli.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectre.Console.Cli;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("settings.json", false);
builder.Services.Configure<LintingSettings>(builder.Configuration.GetRequiredSection("LintingSettings"));
builder.Services.AddSingleton(resolver =>
{
    var options = resolver.GetRequiredService<IOptions<LintingSettings>>();
    return options.Value;
});

// Add a command and optionally configure it.
builder.Services.AddSingleton<IConventionalCommitParser, ConventionalCommitParser>();
builder.Services.AddCommand<LintCommand>("lint", cmd =>
{
    cmd.WithDescription("A command that lints");
});

//
// The standard call save for the commands will be pre-added & configured
//
builder.UseSpectreConsole<LintCommand>(config =>
{
    // All commands above are passed to config.AddCommand() by this point

#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
    config.SetApplicationName("czn");
    config.UseBasicExceptionHandler();
});

var app = builder.Build();
app.Run();