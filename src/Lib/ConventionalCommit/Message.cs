namespace commitizen.NET.Lib.ConventionalCommit;

public class Message
{
    public required Header Header { get; set; }
    public required Body Body { get; set; }
    public List<ConventionalCommitIssue> Issues { get; set; } = [];
    public bool IsFeature => Header.Type == "feat";
    public bool IsFix => Header.Type == "fix";
    public bool IsBreakingChange => Body.FooterText.Contains('!') || Header.IsBreakingChange;
    public required string Text { get; init; }
}

public class Footer
{
    public List<FooterValue> Values { get; set; } = [];
}
public record FooterValue(string Token, string Separator, string Value);

public class Body
{

    public Body() { }
    public Body(string input)
    {
        Text = input;
        var footerMatch = DefaultPatterns.FooterPattern.Match(input);
        FooterText = input[footerMatch.Index..];
        var split = DefaultPatterns.FooterPattern.Split(input, 2);
        FooterText = footerMatch.Success ? input[footerMatch.Index..] : string.Empty;
        while (footerMatch.Success)
        {
            var token = footerMatch.Groups["token"].Value;
            var separator = footerMatch.Groups["separator"].Value;
            var value = footerMatch.Groups["value"].Value;
            Footers.Add(new FooterValue(token, separator, value));
            var current = footerMatch;
            Console.WriteLine($"Current = {current.Value}");
            footerMatch = footerMatch.NextMatch();
            Console.WriteLine($"Next = {footerMatch.Value}");
        }
    }
    public string Text { get; init; } = string.Empty;
    public bool HasFooter { get; set; } = false;
    public string FooterText { get; set; } = string.Empty;
    public List<FooterValue>? Footers = [];

    internal static Body Empty()
    {
        return new Body();
    }
}

public class Header
{
    public Header(string input)
    {
        Text = input;
        var match = DefaultPatterns.HeaderPattern.Match(input);

        if (match.Success)
        {
            Type = match.Groups["type"].Value;
            Scope = match.Groups["scope"].Value;
            Subject = match.Groups["subject"].Value;
            IsBreakingChange = match.Groups["breakingChangeMarker"].Success;
        }
        else
        {
            Subject = input;
        }
    }

    public string Scope { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public bool IsBreakingChange { get; internal set; }
    public string Text { get; init; }

    internal static Header Empty()
    {
        return new Header("");
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
