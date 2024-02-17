namespace commitizen.NET;

public class ConventionalCommit : IConventionalCommitHeader
{
    public string? Sha { get; set; }

    public string Scope { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<ConventionalCommitNote> Notes { get; set; } = [];

    public List<ConventionalCommitIssue> Issues { get; set; } = [];

    public bool IsFeature => Type == "feat";
    public bool IsFix => Type == "fix";
    public bool IsBreakingChange => Notes.Any(note => "BREAKING CHANGE".Equals(note.Title));
}

public interface IConventionalCommitHeader
{
    public string Type { get; }
    public string Scope { get; }
    public string Description { get; }
}

public class ConventionalCommitNote// : IConventionalCommitHeader
{
    public string Title { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    // public string Type { get; set; } = string.Empty;

    // public string Scope { get; set; } = string.Empty;

    // public string Description { get; set; } = string.Empty;
}

public class ConventionalCommitIssue
{
    public string Token { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;
}

public class ConventionalCommitType
{
    public required string Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;

    public override string ToString() => $"{Emoji}{Type}";
}