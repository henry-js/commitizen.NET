using System.Text.RegularExpressions;
using FluentResults;
using static commitizen.NET.Lib.DefaultPatterns;

namespace commitizen.NET.Lib;

public class ConventionalCommitParser : IConventionalCommitParser
{
    public ConventionalCommitParser(LintingSettings defaultSettings)
    {
        DefaultSettings = defaultSettings;
    }

    private readonly string[] NoteKeywords = ["BREAKING CHANGE"];

    private LintingSettings DefaultSettings { get; }

    public Result<ConventionalCommit> Validate(Commit commit)
    {
        Result<ConventionalCommit> result = ValidateHeaderResult(commit);
        CheckForIssues(result);
        return result;
    }

    private void CheckForIssues(Result<ConventionalCommit> result)
    {
        var conventionalCommit = result.Value;
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

    private Result<ConventionalCommit> ValidateHeaderResult(Commit commit)
    {
        var header = commit.MessageLines[0];

        var match = HeaderPattern.Match(header);
        if (!match.Success)
        {
            return Result.Fail(new InvalidConventionalCommitSyntaxError(header));
        }
        var vr = new ValidationResult();

        var conventionalCommit = new ConventionalCommit()
        {
            Header = new()
            {
                Type = ValidateType(match.Groups["type"].Value, vr),
                Scope = ValidateScope(match.Groups["scope"].Value, vr),
                Subject = match.Groups["subject"].Value,
            }
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

        if (vr.Errors.Count > 0)
        {
            return Result.Fail(vr.Errors)
                .WithSuccesses(vr.Warnings)
                .ToResult(conventionalCommit);
        }

        return Result.Ok(conventionalCommit)
            .WithSuccesses(vr.Warnings);
    }

    private string ValidateScope(string input, ValidationResult validationResult)
    {
        var scope = ScopePattern.Match(input);

        if (scope.Success)
        {
            return input;
        }

        return input;
    }

    private string ValidateType(string value, ValidationResult validationResult)
    {
        if (!DefaultSettings.Types.ContainsKey(value))
        {
            validationResult.Errors.Add(new TypeDoesNotExistError(value, DefaultSettings.Types));
            return "";
        }
        return value;
    }

    private string ValidateSubject(string value, ValidationResult validationResult)
    {
        return "";
    }

    private void ValidateRemaining(ConventionalCommit conventionalCommit, string[] remainingLines)
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

public interface IConventionalCommitParser
{
    public Result<ConventionalCommit> Validate(Commit commit);
}