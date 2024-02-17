using System.Text.RegularExpressions;
using LibGit2Sharp;

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

        var commitMessageLines = commit.Message.Split(
                Environment.NewLine,
                StringSplitOptions.None
            )
            .Select(line => line.Trim())
            .ToList();

        var header = commitMessageLines.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(header))
        {
            return conventionalCommit;
        }

        var match = HeaderPattern.Match(header);
        if (match.Success)
        {
            conventionalCommit.Scope = match.Groups["scope"].Value;
            conventionalCommit.Type = match.Groups["type"].Value;
            conventionalCommit.Description = match.Groups["subject"].Value;

            if (match.Groups["breakingChangeMarker"].Success)
            {
                conventionalCommit.Notes.Add(new ConventionalCommitNote
                {
                    Title = "BREAKING CHANGE",
                    Text = string.Empty
                });
            }

            var issuesMatch = IssuesPattern.Matches(conventionalCommit.Description);
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
            conventionalCommit.Description = header;
        }

        for (var i = 1; i < commitMessageLines.Count; i++)
        {
            foreach (var noteKeyword in NoteKeywords)
            {
                var line = commitMessageLines[i];
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