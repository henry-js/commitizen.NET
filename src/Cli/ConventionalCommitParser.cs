using System.Text.RegularExpressions;
using Lib;

namespace commitizen.NET;

public static partial class ConventionalCommitParser
{
    private static readonly string[] NoteKeywords = ["BREAKING CHANGE"];

    private static readonly Regex HeaderPattern = new("^(?<type>\\w*)(?:\\((?<scope>.*)\\))?(?<breakingChangeMarker>!)?: (?<subject>.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly Regex IssuesPattern = new("(?<issueToken>#(?<issueId>\\d+))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
    private static readonly Regex Whitespace = WhitespacePattern();
    private static readonly Regex IsFooterToken = IsWordPattern();
    private static readonly Regex IsFooter = IsFooterPattern();

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

        // body is freeform
        var bodyParagraphs = remainingLines[..^1];
        conventionalCommit.Body = ParseBody(bodyParagraphs);


        var lastParagraph = remainingLines.Last();
        conventionalCommit.Footers = ParseLastParagraph(lastParagraph);

        /*
        *   A footer’s token MUST use - in place of whitespace characters, e.g., Acked-by (this helps differentiate the footer section from a multi-paragraph body). An exception is made for BREAKING CHANGE, which MAY also be used as a token.
        *   A footer’s value MAY contain spaces and newlines, and parsing MUST terminate when the next valid footer token/separator pair is observed.
        */
        // for (var i = 1; i < footer.Length; i++)
        // {
        //     foreach (var noteKeyword in NoteKeywords)
        //     {
        //         var line = footer[i];
        //         if (line.StartsWith($"{noteKeyword}:"))
        //         {
        //             conventionalCommit.Footers.Add(new Footer
        //             {
        //                 Title = noteKeyword,
        //                 Text = line[$"{noteKeyword}:".Length..].TrimStart()
        //             });

        //             continue;
        //         }
        //     }

        // }
        var footerJoined = string.Join(Environment.NewLine, lastParagraph);
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

    private static List<Footer> ParseLastParagraph(string text)
    {
        var lines = text.Split(Environment.NewLine, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++)
        {
            var ft = IsFooterToken.Match(lines[i]);
            if (ft.Success)
            {

            }
        }
        var tokens = Whitespace.Split(text);
        var footerToken = IsFooterToken.Match(tokens[0]);
        if (footerToken.Success)
        {
            footerToken.GetType();
        }
        else
        {
            // TODO: Handle invalid footer
        }
        for (int i = 0; i < tokens.Length; i++)
        {
            if (i == 0 && tokens[i] == ":")
            {

            }
        }
        return [];
    }

    private static Body? ParseBody(string[] texts)
    {
        var body = new Body();
        foreach (var text in texts)
        {
            var tokens = Whitespace.Split(text);
            foreach (var item in tokens)
            {
                Console.WriteLine(item);
            }
        }
        return body;
    }

    [GeneratedRegex(@"\s")]
    private static partial Regex WhitespacePattern();

    [GeneratedRegex(@"(?<token>[\w-]+)(?<separator>[: ][ #])(?<value>.+)(?=[\w-]+)")]
    private static partial Regex IsWordPattern();

    [GeneratedRegex(@"(?<token>[\w-]+)(?<separator>[: ][ #])(?<value>.+)")]
    private static partial Regex IsFooterPattern();
}

internal class ParseResult<ConventionalCommit>
{
    public List<string> Errors { get; } = [];
    public bool IsSuccess => Errors.Count > 0;

}