using System.Text.RegularExpressions;

namespace commitizen.NET.Lib;

public static partial class DefaultPatterns
{
    public static readonly Regex HeaderPattern = HeaderRegex();
    public static readonly Regex IssuesPattern = IssuesRegex();
    public static readonly Regex FooterPattern = FooterRegex();

    [GeneratedRegex(@"^(?<type>[\w\s]*)(?:\((?<scope>.*)\))?(?<breakingChangeMarker>!)?: (?<subject>.*)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex HeaderRegex();
    [GeneratedRegex(@"(?<issueToken>#(?<issueId>\d+))", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex IssuesRegex();
    [GeneratedRegex(@"^(?<token>[\w\-]+|BREAKING CHANGE)(?<seperator>: | #)(?<value>.*?(?=$|^([\w\-]+|BREAKING CHANGE)(: | #)))", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex FooterRegex();
}