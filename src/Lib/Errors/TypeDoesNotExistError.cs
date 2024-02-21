using FluentResults;

namespace commitizen.NET.Lib;

public class TypeDoesNotExistError : Error
{
    public TypeDoesNotExistError(string input, Dictionary<string, CommitType> commitTypes)
        : base($"type must be one of [{string.Join(", ", commitTypes.Keys)}]")
    {
        Metadata.Add(nameof(input), input);
        Metadata.Add(nameof(commitTypes), commitTypes);

    }
}