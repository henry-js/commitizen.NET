using System.Text.RegularExpressions;
using commitizen.NET.Lib;
using static commitizen.NET.Lib.DefaultPatterns;

namespace commitizen.NET;

public static class ConventionalCommitParser
{
    private static readonly string[] NoteKeywords = ["BREAKING CHANGE"];

    public static List<ConventionalCommit> Parse(List<Commit> commits)
    {
        return commits.ConvertAll(Parse);
    }

    public static ConventionalCommit Parse(Commit commit)
    {
        var conventionalCommit = new ConventionalCommit();

        var header = commit.MessageLines[0];

        ValidateHeader(conventionalCommit, header);

        if (commit.MessageLines.Length < 2) return conventionalCommit;

        ValidateRemaining(conventionalCommit, commit.MessageLines[1..]);

        return conventionalCommit;

        static void ValidateHeader(ConventionalCommit conventionalCommit, string header)
        {
            var match = HeaderPattern.Match(header);
            if (match.Success)
            {
                conventionalCommit.Header = new Header()
                {
                    Scope = match.Groups["scope"].Value,
                    Type = match.Groups["type"].Value,
                    Subject = match.Groups["subject"].Value,
                };

                if (match.Groups["breakingChangeMarker"].Success)
                {
                    conventionalCommit.Footers.Add(new ConventionalCommitNote
                    {
                        Title = "BREAKING CHANGE",
                        Text = string.Empty
                    });
                }

                var issuesMatch = IssuesPattern.Matches(conventionalCommit.Header.Subject);
                foreach (var issueMatch in issuesMatch.Cast<Match>())
                {
                    conventionalCommit.Issues.Add(
                        new ConventionalCommitIssue
                        {
                            Token = issueMatch.Groups["issueToken"].Value,
                            Id = issueMatch.Groups["issueId"].Value,
                        });
                }
            }
            else
            {
                // conventionalCommit.Header.Subject = header;
            }
        }
    }

    public static ParseResult<ConventionalCommit> Validate(Commit commit)
    {
        throw new NotImplementedException();
    }

    private static void ValidateRemaining(ConventionalCommit conventionalCommit, string[] remainingLines)
    {

        if (remainingLines.Length < 1) return;
        // body is freeform
        var bodyParagraphs = remainingLines[..^1];


        var footerLines = remainingLines[^1];
        var footerString = string.Join(Environment.NewLine, footerLines);
        var footerMatches = FooterPattern.Matches(footerString);
        if (footerMatches.Count > 0)
        {
            for (int i = 0; i < footerMatches.Count; i++)
            {
                var curMatch = footerMatches[i];
                var valueStartIndex = curMatch.Groups["value"].Index;
                var valueEndIndex = footerMatches.Count > i + 1 ? footerMatches[i + 1].Index : footerString.Length;
                Console.WriteLine(curMatch.Value);
                var matchedFooter = new Footer()
                {
                    Title = curMatch.Groups["token"].Value,
                    Text = footerString[valueStartIndex..valueEndIndex].Trim(),
                };
                conventionalCommit.Footers.Add(matchedFooter);
            }
        }
    }

}
public class ParseResult<ConventionalCommit>
{
    public List<string> Errors { get; } = [];
    public bool IsSuccess => Errors.Count > 0;

}

public class HeaderValidationResult
{
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}