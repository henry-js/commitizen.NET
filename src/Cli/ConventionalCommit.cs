namespace commitizen.NET;

public class ConventionalCommit
{
    public Header Header { get; set; } = new Header();
    public string Body { get; set; }
    public string? Sha { get; set; }
    public List<ConventionalCommitNote> Footers { get; set; } = [];
    public List<ConventionalCommitIssue> Issues { get; set; } = [];
    public bool IsFeature => Header.Type == "feat";
    public bool IsFix => Header.Type == "fix";
    public bool IsBreakingChange => Footers.Any(note => "BREAKING CHANGE".Equals(note.Title));
}

public class Footer : ConventionalCommitNote
{
}

public class Body
{
    public Body()
    {
    }
    public string[] Paragraphs { get; set; }
}

public class Header
{
    public string Scope { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
}

public class ConventionalCommitNote
{
    public string Title { get; set; }

    public string Text { get; set; }
}

public class ConventionalCommitIssue
{
    public string Token { get; set; }

    public string Id { get; set; }
}
