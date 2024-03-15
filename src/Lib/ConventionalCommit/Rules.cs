namespace commitizen.NET.Lib.ConventionalCommit;

public class Rules
{
    public const string Key = "Rules";

    /// <summary>
    /// condition: <c>type</c> should be found in <c>Value</c>.
    /// </summary>
    public CommitlintRule<string[]> TypeEnum { get; set; } = default!;
    public CommitlintRule TypeNotEmpty { get; set; } = default!;
    public CommitlintRule<int> HeaderMaxLength { get; set; } = default!;
    public CommitlintRule<int> HeaderMinLength { get; set; } = default!;
    public CommitlintRule HeaderNoTrim { get; set; } = default!;
    public CommitlintRule BodyLeadingBlank { get; set; } = default!;
    public CommitlintRule BodyEmpty { get; set; } = default!;
    public CommitlintRule FooterLeadingBlank { get; set; } = default!;
}

public class CommitlintRule<T> : CommitlintRule
    where T : notnull//, new()
{
    public T Value { get; set; } = default!;
}

public class CommitlintRule
{
    public ErrorLevel Level { get; set; }
    public State State { get; set; }
}

public enum State
{
    off,
    on,
}

public enum ErrorLevel
{
    error = 0,
    warning = 1,
    disable = 2,
}
