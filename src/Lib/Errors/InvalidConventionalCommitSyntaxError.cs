using FluentResults;

namespace commitizen.NET.Lib;

public class InvalidSyntaxError : Error
{
    public InvalidSyntaxError(string commitHeader)
        : base($"Invalid syntax: {commitHeader}.")
    {
        Metadata.Add("Severity", "Error");
    }
}
