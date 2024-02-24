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

        if (!result.IsSuccess || commit.MessageLines.Length == 1)
            return result;

        ValidateRemaining(result);
        CheckForIssues(result);
        return result;
    }

    private void CheckForIssues(Result<ConventionalCommit> result)
    {
        var conventionalCommit = result.Value;
        var issuesMatch = IssuesPattern.Matches(conventionalCommit.Original);

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

        if (!match.Success) return Result.Fail(new InvalidConventionalCommitSyntaxError(header));

        var conventionalCommit = new ConventionalCommit()
        {
            Header = new()
            {
                Type = match.Groups["type"].Value,
                Scope = match.Groups["scope"].Value,
                Subject = match.Groups["subject"].Value,
            },
            Original = commit.Message
        };

        if (match.Groups["breakingChangeMarker"].Success)
        {
            conventionalCommit.Footers.Add(new ConventionalCommitNote
            {
                Title = "BREAKING CHANGE",
                Text = string.Empty
            });
        }

        ValidationResult vr = GetErrors(conventionalCommit);

        if (vr.Errors.Count > 0)
        {
            return Result.Fail(vr.Errors)
                .WithSuccesses(vr.Warnings)
                .ToResult(conventionalCommit);
        }

        return Result.Ok(conventionalCommit)
            .WithSuccesses(vr.Warnings);
    }

    private ValidationResult GetErrors(ConventionalCommit conCommit)
    {
        var validationResult = new ValidationResult();

        if (!DefaultSettings.Types.TryGetValue(conCommit.Header.Type, out var cti))
        {
            validationResult.Errors.Add(new TypeDoesNotExistError(conCommit.Header.Type, DefaultSettings.Types));
        }

        return validationResult;
    }

    // private string ValidateScope(string input, ValidationResult validationResult)
    // {
    //     var scope = ScopePattern.Match(input);

    //     if (scope.Success)
    //     {
    //         return input;
    //     }

    //     return input;
    // }

    // private string ValidateType(string value, ValidationResult validationResult)
    // {

    // }

    // private string ValidateSubject(string value, ValidationResult validationResult)
    // {
    //     return value;
    // }

    private void ValidateRemaining(Result<ConventionalCommit> result)
    {
        var remainingLines = result.Value.Original.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
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
                result.Value.Footers.Add(matchedFooter);
            }
        }
    }
}

public interface IConventionalCommitParser
{
    public Result<ConventionalCommit> Validate(Commit commit);
}