namespace commitizen.NET.Lib.ConventionalCommit;

public class Message
{
    public required Header Header { get; set; }
    public Body? Body { get; set; }
    public string? Sha { get; set; }
    public List<ConventionalCommitIssue> Issues { get; set; } = [];
    public bool IsFeature => Header.Type == "feat";
    public bool IsFix => Header.Type == "fix";
    // public bool IsBreakingChange => Footer.Values.Any(note => note.StartsWith("BREAKING CHANGE") || Header.IsBreakingChange);
    public required string Original { get; init; }
}

public class Footer
{
    public List<FooterValue> Values { get; set; } = [];
}
public record FooterValue(string Token, string Separator, string Value);

public class Body
{
    public Body(string rawText)
    {
        Text = rawText;
    }
    public Body() { }
    public string Text { get; init; }
    public bool HasFooter { get; set; } = false;
    public string? FooterText { get; set; }
    public List<FooterValue>? Footers = [];
}

public class Header
{
    public string Scope { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public bool IsBreakingChange { get; internal set; }
    public required string Raw { get; init; }

    internal static Header Empty()
    {
        return new Header() { Raw = "" };
    }
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
