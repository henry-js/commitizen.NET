using commitizen.NET.Lib;
using FluentResults;
using Spectre.Console;

public static class ConsoleExtensions
{
    public static void WriteErrors(this IAnsiConsole console, Result<ConventionalCommit> result)
    {
        foreach (var error in result.Errors)
        {
            console.MarkupLine(error.Message);
        }
    }

    public static void WriteInput(this IAnsiConsole console, Result<ConventionalCommit> result)
    {
        throw new NotImplementedException();
    }
}