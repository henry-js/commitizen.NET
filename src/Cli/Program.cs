using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using commitizen.NET.Cli;
using commitizen.NET.Lib;
using Community.Extensions.Spectre.Cli.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console.Cli;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.Sources.Clear();
builder.Logging.ClearProviders();
builder.Configuration
    .SetBasePath(new FileInfo(typeof(Program).Assembly.Location).DirectoryName)
    .AddJsonFile($"{new FileInfo(typeof(Program).Assembly.Location).DirectoryName}/rules.json", false);

builder.Services.Configure<Rules>(builder.Configuration.GetRequiredSection(Rules.Key));
var options = new Rules();
builder.Configuration.GetSection(Rules.Key).Bind(options);
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<Rules>>().Value);
builder.Services.AddSingleton<IConventionalCommitParser, ConventionalCommitParser>();

builder.Services.AddCommand<LintCommand>("lint", cmd =>
{
    cmd.WithDescription("A command that lints");
});

builder.UseSpectreConsole<LintCommand>(config =>
{

#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
    config.UseBasicExceptionHandler();
#endif
    config.SetApplicationName("czn");
});

var app = builder.Build();
await app.RunAsync();