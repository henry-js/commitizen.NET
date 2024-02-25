using commitizen.NET.Lib;
using FluentResults;
using Spectre.Console;

public static class ConsoleExtensions
{
    public static void WriteErrors(this IAnsiConsole console, Result<ConventionalCommit> result)
    {
        foreach (var error in result.Errors)
        {
            console.WriteLine(error.Message);
        }
    }

    public static void WriteInput(this IAnsiConsole console, string input)
    {
        var markup = Emoji.Replace($":inbox_tray: {input}");
        console.MarkupLine(markup);
    }
}