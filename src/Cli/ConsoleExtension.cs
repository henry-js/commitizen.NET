using commitizen.NET.Lib.ConventionalCommit;
using FluentResults;
using Spectre.Console;

public static class ConsoleExtensions
{
    public static void WriteErrors(this IAnsiConsole console, Result<Message> result)
    {
        int errors = 0;
        int warnings = 0;
        const string errorEmoji = ":cross_mark:";
        const string warnEmoji = ":white_exclamation_mark:";
        var grid = new Grid().AddColumns(2);
        foreach (var error in result.Errors)
        {
            var severity = error.Metadata["Severity"].ToString();
            switch (severity)
            {
                case "Error":
                    errors++;
                    grid.AddRow(Emoji.Replace(errorEmoji), error.Message.EscapeMarkup());
                    break;
                case "Warning":
                    warnings++;
                    grid.AddRow(Emoji.Replace(warnEmoji), error.Message.EscapeMarkup());
                    break;
            }
        }
        string summaryEmoji = errors > 0 ? errorEmoji : warnEmoji;
        string summary = $"found [red]{errors} errors[/], [yellow]{warnings} warnings[/]";

        grid.AddRow(Emoji.Replace(summaryEmoji), summary);

        console.Write(grid);
        console.Write(new Rule());
        console.WriteLine("Correct format:\n");
        console.WriteLine(Constants.CorrectFormat);
    }

    public static void WriteInput(this IAnsiConsole console, string input)
    {
        var markup = Emoji.Replace($":inbox_tray: input => '{input}'");
        console.MarkupLine(markup);
    }
}