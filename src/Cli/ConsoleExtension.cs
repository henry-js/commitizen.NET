using commitizen.NET.Lib;
using FluentResults;
using Spectre.Console;

public static class ConsoleExtensions
{
    public static void WriteErrors(this IAnsiConsole console, Result<ConventionalCommit> result)
    {
        int errors = 0;
        int warnings = 0;

        var grid = new Grid().AddColumns(2);

        foreach (var error in result.Errors)
        {
            string emoji = "";
            var severity = error.Metadata["Severity"].ToString();
            if (severity == "Error")
            {
                emoji = ":cross_mark:";
                errors++;
            }
            else if (severity == "Warning")
            {
                emoji = ":warning:";
                warnings++;
            }
            grid.AddRow(Emoji.Replace(emoji), error.Message.EscapeMarkup());
        }
        string summaryEmoji = errors > 0 ? ":cross_mark:" : ":warning:";
        string summary = $"found {errors} errors, {warnings} warnings";

        grid.AddRow(Emoji.Replace(summaryEmoji), summary);

        console.Write(grid);
    }

    public static void WriteInput(this IAnsiConsole console, string input)
    {
        var markup = Emoji.Replace($":inbox_tray: input: {input}");
        console.MarkupLine(markup);
    }
}