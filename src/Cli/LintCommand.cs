using commitizen.NET.Lib;
using Spectre.Console.Cli;

namespace commitizen.NET.Cli;

internal class LintCommand(IConventionalCommitParser parser) : Command<LintCommand.Settings>
{
    private readonly IConventionalCommitParser parser = parser;

    public override int Execute(CommandContext context, Settings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.CommitMessage)) throw new ArgumentNullException(nameof(settings.CommitMessage), "Null or empty value passed to cli");

        var commit = new Commit("", settings.CommitMessage);

        var result = parser.Validate(commit);
        return 01;
    }
    internal class Settings : CommandSettings
    {
        [CommandArgument(0, "[Name]")]
        public string? CommitMessage { get; set; }
    }
}
