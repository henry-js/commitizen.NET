using System.Text.RegularExpressions;

namespace commitizen.NET.Lib;

public static partial class DefaultPatterns
{
    [GeneratedRegex(@"^(?<type>[\w]*)(?:\((?<scope>.*)\))?(?<breakingChangeMarker>!)?: (?<subject>.*)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex HeaderRegex();
    public static readonly Regex HeaderPattern = HeaderRegex();

    [GeneratedRegex(@"(?<issueToken>#(?<issueId>\d+))", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex IssuesRegex();
    public static readonly Regex IssuesPattern = IssuesRegex();

    [GeneratedRegex(@"^(?<token>[\w\-]+|BREAKING CHANGE)(?<seperator>: | #)(?<value>.*?(?=$|^([\w\-]+|BREAKING CHANGE)(: | #)))", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex FooterRegex();
    public static readonly Regex FooterPattern = FooterRegex();

    [GeneratedRegex(@"\(?^\w{2,9}$\)?", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]

    private static partial Regex ScopeRegex();
    public static readonly Regex ScopePattern = ScopeRegex();

    [GeneratedRegex(@"(?<issueToken>#(?<issueId>\d+))", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]

    private static partial Regex TypeRegex();
    public static readonly Regex TypePattern = TypeRegex();
    [GeneratedRegex(@"(?<issueToken>#(?<issueId>\d+))", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]

    private static partial Regex SubjectRegex();
    public static readonly Regex SubjectPattern = SubjectRegex();

}