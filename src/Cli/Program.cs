using commitizen.NET.Cli;
using commitizen.NET.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("settings.json", false);
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
builder.Services.Configure<LintingSettings>(builder.Configuration.GetRequiredSection("LintingSettings"));
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
builder.Services.AddSingleton(resolver =>
{
    var options = resolver.GetRequiredService<IOptions<LintingSettings>>();
    return options.Value;
});

// Add a command and optionally configure it.
builder.Services.AddSingleton<IConventionalCommitParser, ConventionalCommitParser>();

var registrar = new TypeRegistrar(builder);
var app = new CommandApp();
app.Configure(config =>
{
#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
    config.SetApplicationName("czn");
    config.AddCommand<LintCommand>("lint");
});

try
{
    return app.Run(args);

}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.Default);
    return -99;
}

// builder.Services.AddCommand<LintCommand>("lint", cmd =>
// {
//     cmd.WithDescription("A command that lints");
// });

//
// The standard call save for the commands will be pre-added & configured
//
// builder.UseSpectreConsole<LintCommand>(config =>
// {
//     // All commands above are passed to config.AddCommand() by this point

// #if DEBUG
//     config.PropagateExceptions();
//     config.ValidateExamples();
// #endif
//     config.SetApplicationName("czn");
//     config.UseBasicExceptionHandler();
// });

// var app = builder.Build();
// await app.RunAsync();