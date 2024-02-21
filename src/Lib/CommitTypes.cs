namespace commitizen.NET.Lib;

public class DefaultSettings
{
    CommitType[] CommitTypes { get; set; } = default!;
}

public class CommitType
{
    public string Description { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Emoji { get; set; } = default!;
}