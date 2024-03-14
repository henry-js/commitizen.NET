namespace commitizen.NET.Lib.ConventionalCommit;

public class Rules
{
    public const string Key = "Rules";

    /// <summary>
    /// condition: <c>type</c> should be found in <c>Value</c>.
    /// </summary>
    public TypeEnum TypeEnum { get; set; } = default!;
}

public class TypeEnum : CommitlintRule
{
    /// <summary>
    /// <value>List of allowed values</value
    /// </summary>
    public string[] Value { get; set; } = default!;
}

public class CommitlintRule
{
    public ErrorLevel Level { get; set; }
    public Applicable Applicable { get; set; }
}

public enum Applicable
{
    never,
    always,
}

public enum ErrorLevel
{
    error = 0,
    warning = 1,
    disable = 2,
}