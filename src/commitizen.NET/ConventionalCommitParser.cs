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
        var conventionalCommit = new ConventionalCommit();

        var header = commit.MessageLines[0];

        ValidateHeader(conventionalCommit, header);

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
                // conventionalCommit.Header.Subject = header;
            }
        }
    }

    private static void ValidateRemaining(ConventionalCommit conventionalCommit, string[] remainingLines)
    {
        var lastEmptyLine = Array.LastIndexOf(remainingLines, string.Empty);

        // body is freeform
        var body = remainingLines[..lastEmptyLine];

        var footer = remainingLines[lastEmptyLine..];

        /*
        *   A footer’s token MUST use - in place of whitespace characters, e.g., Acked-by (this helps differentiate the footer section from a multi-paragraph body). An exception is made for BREAKING CHANGE, which MAY also be used as a token.
        *   A footer’s value MAY contain spaces and newlines, and parsing MUST terminate when the next valid footer token/separator pair is observed.
        */
        for (var i = 1; i < footer.Length; i++)
        {
            foreach (var noteKeyword in NoteKeywords)
            {
                var line = footer[i];
                if (line.StartsWith($"{noteKeyword}:"))
                {
                    conventionalCommit.Footers.Add(new Footer
                    {
                        Title = noteKeyword,
                        Text = line[$"{noteKeyword}:".Length..].TrimStart()
                    });

                    continue;
                }
            }

        }
        var footerJoined = string.Join(Environment.NewLine, footer);
        bool isNewLine = true;
        int tokenStart = 0;
        int tokenEnd = 0;
        string token;
        for (int i = 0; i < footerJoined.Length; i++)
        {
            var current = footerJoined[i];
            Console.WriteLine(current);
            if (footerJoined[i] == ':' && i != 0)
            {
                token = footerJoined[tokenStart..tokenEnd];
            }
            tokenEnd++;
        }
    }
}

internal class ParseResult<ConventionalCommit>
{
    public List<string> Errors { get; } = [];
    public bool IsSuccess => Errors.Count > 0;

}