using commitizen.NET.Lib.Validators;
using static commitizen.NET.Lib.DefaultPatterns;
using FluentResults;
using System.Text.RegularExpressions;

namespace commitizen.NET.Lib;

public class ConventionalCommitParser : IConventionalCommitParser
{
    public ConventionalCommitParser(LintingSettings defaultSettings)
    {
        DefaultSettings = defaultSettings;
    }
    private readonly string[] lineFeeds = ["\n", "\r\n", "\r"];

    private readonly string[] NoteKeywords = ["BREAKING CHANGE"];

    private LintingSettings DefaultSettings { get; }

    public Result<ConventionalCommit> Parse(string msg)
    {
        string[] msgLines = msg.Split(lineFeeds, StringSplitOptions.None);
        Result<Header> headerResult = ParseHeader(msgLines[0]);

        var commitResult = headerResult.ToResult(h =>
        new ConventionalCommit()
        {
            Header = h,
            Original = msg
        });

        if (commitResult.IsFailed)
            return commitResult;

        FindIssues(commitResult);

        // if (msgLines.Length == 1)
        return commitResult;
    }

    private Result<Body> ParseBody(string[] strings)
    {
        throw new NotImplementedException();
    }

    private void FindIssues(Result<ConventionalCommit> result)
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

    private Result<Header> ParseHeader(string header)
    {
        var match = HeaderPattern.Match(header);

        if (!match.Success) return Result.Fail(new InvalidSyntaxError(header));

        Header conventionalHeader = new()
        {
            Type = match.Groups["type"].Value,
            Scope = match.Groups["scope"].Value,
            Subject = match.Groups["subject"].Value,
            IsBreakingChange = match.Groups["breakingChangeMarker"].Success
        };

        var validator = new HeaderValidator(DefaultSettings);

        var results = validator.Validate(conventionalHeader);

        if (!results.IsValid)
            return Result.Fail(results.Errors.Select(x => x.ErrorMessage));
        return Result.Ok(conventionalHeader);
    }
}

public interface IConventionalCommitParser
{
    public Result<ConventionalCommit> Parse(string msg);
}