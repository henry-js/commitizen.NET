using FluentResults;

namespace commitizen.NET.Lib;

public class TypeDoesNotExistError : Error
{
    private string value;
    private Dictionary<string, CommitType> commitTypes;

    public TypeDoesNotExistError(string input, Dictionary<string, CommitType> commitTypes)
        : base($"type must be one of [{string.Join(", ", commitTypes.Keys)}]")
    {
        Metadata.Add(nameof(input), input);
        Metadata.Add(nameof(commitTypes), commitTypes);

    }
}