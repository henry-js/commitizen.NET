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
        var result = parser.Parse(settings.CommitMessage);

        console.WriteInput(settings.CommitMessage);
        if (result.IsSuccess)
        {
            Console.WriteLine(settings.CommitMessage);
            return 0;
        }
        else
        {
            console.WriteErrors(result);
            return 01;
        }
    }

    internal class Settings : CommandSettings
    {
        [CommandArgument(0, "[message]")]
        public string CommitMessage { get; set; } = null!;

        public override ValidationResult Validate()
        {
            return string.IsNullOrWhiteSpace(CommitMessage)
                ? ValidationResult.Error("input cannot be null or empty")
                : ValidationResult.Success();
        }
    }
}
