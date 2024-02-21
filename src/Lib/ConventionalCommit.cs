namespace commitizen.NET.Lib;

public class ConventionalCommit
{
    public Header Header { get; set; } = new Header();
    public string? Body { get; set; }
    public string? Sha { get; set; }
    public List<ConventionalCommitNote> Footers { get; set; } = [];
    public List<ConventionalCommitIssue> Issues { get; set; } = [];
    public bool IsFeature => Header.Type == "feat";
    public bool IsFix => Header.Type == "fix";
    public bool IsBreakingChange => Footers.Any(note => "BREAKING CHANGE".Equals(note.Title));

    public required string Original { get; init; }
}

public class Footer : ConventionalCommitNote
{
}

public class Body
{
    public string[] Paragraphs { get; set; } = [];
}

public class Header
{
    public string Scope { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
}

public class ConventionalCommitNote
{
    public string Title { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}

public class ConventionalCommitIssue
{
    public string Token { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;
}
