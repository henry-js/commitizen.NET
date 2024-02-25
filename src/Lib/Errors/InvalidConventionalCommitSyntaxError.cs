using FluentResults;

namespace commitizen.NET.Lib;

public class InvalidSyntaxError : Error
{
    private readonly string correctSyntax = """
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
""";

    public InvalidSyntaxError(string commitHeader)
        : base($"Invalid syntax: {commitHeader}.")
    {
        Metadata.Add(nameof(correctSyntax), correctSyntax);
    }
}
