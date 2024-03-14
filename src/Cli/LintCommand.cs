using System.ComponentModel;
using commitizen.NET.Lib;
using commitizen.NET.Lib.ConventionalCommit;
using FluentResults;
using Spectre.Console;
using Spectre.Console.Cli;

namespace commitizen.NET.Cli;

internal class LintCommand(IMessageParser parser, IAnsiConsole console) : Command<LintCommand.Settings>
{
    private readonly IMessageParser parser = parser;
    private readonly IAnsiConsole console = console;

    public override int Execute(CommandContext context, Settings settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(settings.CommitMessage);

        if (settings.AsFile)
        {
            if (!File.Exists(settings.CommitMessage)) throw new FileNotFoundException("File does not exist", settings.CommitMessage);
            else settings.CommitMessage = File.ReadAllLines(settings.CommitMessage)[0];
        }
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
        public string CommitMessage { get; set; } = string.Empty;

        [CommandOption("-f|--file")]
        [DefaultValue(false)]
        public bool AsFile { get; set; }

        // public override ValidationResult Validate()
        // {
        //     return string.IsNullOrWhiteSpace(CommitMessage)
        //         ? ValidationResult.Error("input cannot be null or empty")
        //         : ValidationResult.Success();
        // }
    }
}
