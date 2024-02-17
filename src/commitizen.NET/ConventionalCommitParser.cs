using System.Text.RegularExpressions;
using Lib;

namespace commitizen.NET;

public static class ConventionalCommitParser
{
    private static readonly string[] NoteKeywords = ["BREAKING CHANGE"];

    private static readonly Regex HeaderPattern = new("^(?<type>\\w*)(?:\\((?<scope>.*)\\))?(?<breakingChangeMarker>!)?: (?<subject>.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly Regex IssuesPattern = new("(?<issueToken>#(?<issueId>\\d+))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
    // private static readonly string[] lineFeed = [
    //     "\r\n",
    //     "\r",
    //     "\n"
    //     ];

    public static List<ConventionalCommit> Parse(List<Commit> commits)
    {
        return commits.ConvertAll(Parse);
    }

    public static ConventionalCommit Parse(Commit commit)
    {
        var conventionalCommit = new ConventionalCommit
        {
            Sha = commit.Sha
        };



        var header = commit.MessageLines[0];

        if (string.IsNullOrWhiteSpace(header))
        {
            return conventionalCommit;
        }

        var match = HeaderPattern.Match(header);
        if (match.Success)
        {
            conventionalCommit.Header.Scope = match.Groups["scope"].Value;
            conventionalCommit.Header.Type = match.Groups["type"].Value;
            conventionalCommit.Header.Subject = match.Groups["subject"].Value;

            if (match.Groups["breakingChangeMarker"].Success)
            {
                conventionalCommit.Notes.Add(new ConventionalCommitNote
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
            conventionalCommit.Header.Subject = header;
        }

        for (var i = 1; i < commit.MessageLines.Length; i++)
        {
            foreach (var noteKeyword in NoteKeywords)
            {
                var line = commit.MessageLines[i];
                if (line.StartsWith($"{noteKeyword}:"))
                {
                    conventionalCommit.Notes.Add(new ConventionalCommitNote
                    {
                        Title = noteKeyword,
                        Text = line[$"{noteKeyword}:".Length..].TrimStart()
                    });
                }
            }
        }

        return conventionalCommit;
    }

}

internal class ParseResult<ConventionalCommit>
{
    public List<string> Errors { get; } = [];
    public bool IsSuccess => Errors.Count > 0;

}