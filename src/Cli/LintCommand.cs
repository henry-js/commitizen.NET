using commitizen.NET.Lib;
using FluentResults;
using Spectre.Console;
using Spectre.Console.Cli;

namespace commitizen.NET.Cli;

internal class LintCommand(IConventionalCommitParser parser, IAnsiConsole console) : Command<LintCommand.Settings>
{
    private readonly IConventionalCommitParser parser = parser;
    private readonly IAnsiConsole console = console;

    public override int Execute(CommandContext context, Settings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.CommitMessage)) throw new ArgumentNullException(nameof(settings.CommitMessage), "Null or empty value passed to cli");

        var commit = new Commit("", settings.CommitMessage);

        var result = parser.Validate(commit);

        if (result.IsSuccess)
        {

        }
        else
        {
            console.WriteInput(result);
            console.WriteErrors(result);
        }
        return 01;
    }

    internal class Settings : CommandSettings
    {
        [CommandArgument(0, "[message]")]
        public string? CommitMessage { get; set; }
    }
}
