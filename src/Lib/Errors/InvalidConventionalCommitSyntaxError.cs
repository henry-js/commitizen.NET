using FluentResults;

namespace commitizen.NET.Lib;

public class InvalidConventionalCommitSyntaxError : Error
{
    private readonly string correctSyntax = """
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
""";

    public InvalidConventionalCommitSyntaxError(string commitHeader)
        : base($"Invalid syntax: {commitHeader}.")
    {
        Metadata.Add(nameof(correctSyntax), correctSyntax);
    }
}
