using static commitizen.NET.Lib.ConventionalCommit.DefaultPatterns;
using FluentResults;
using System.Text.RegularExpressions;
using commitizen.NET.Lib.Errors;
using Microsoft.Extensions.Options;

namespace commitizen.NET.Lib.ConventionalCommit;

public class MessageParser : IMessageParser
{
    public MessageParser(IOptions<Rules> defaultSettings)
    {
        DefaultRules = defaultSettings.Value;
    }
    private readonly string[] lineFeeds = ["\n", "\r\n", "\r"];

    private readonly string[] NoteKeywords = ["BREAKING CHANGE"];

    private Rules DefaultRules { get; }

    public Result<Message> Parse(string msg)
    {
        string[] msgLines = msg.Split(lineFeeds, StringSplitOptions.None);
        Result<Header> headerResult = ParseHeader(msgLines[0]);
        var commitResult = headerResult.ToResult(h =>
        new Message()
        {
            Header = h,
            Original = msg
        });

        var msgRemaining = string.Join(Environment.NewLine, msgLines[1..]);
        Result<Footer> footerResult;
        if (HasFooter(msgRemaining, out int startIndex))
        {
            footerResult = ParseFooter(msgRemaining);
        }
        else
        {
            footerResult = Result.Fail<Footer>("");
        }

        Result<Body> bodyResult;

        if (startIndex < 1)
        {
            bodyResult = ParseBody(msgRemaining[0..startIndex]);
        }
        var matches = FooterPattern.Matches(msgRemaining);
        var body = matches.FirstOrDefault() is null ? msgRemaining : msgRemaining[0..matches.First().Index];

        if (commitResult.IsFailed)
            return commitResult;

        FindIssues(commitResult);

        // if (msgLines.Length == 1)
        return commitResult;
    }

    private Result<Footer> ParseFooter(string input)
    {
        var matches = FooterPattern.Matches(input);
        return Result.Fail<Footer>("");
    }

    private bool HasFooter(string msgLines, out int startIndex)
    {
        var matches = FooterPattern.Matches(msgLines);
        if (!matches.Any())
        {
            startIndex = -1;
            return false;
        }
        startIndex = matches.First().Index;
        return true;
    }

    private Result<Body> ParseBody(string input)
    {
        throw new NotImplementedException();
    }

    private void FindIssues(Result<Message> result)
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

        var validator = new HeaderValidator(DefaultRules);

        var results = validator.Validate(conventionalHeader);

        if (!results.IsValid)
            return Result.Fail(results.Errors.Select(x => new ConventionalCommitValidationError(x)));
        return Result.Ok(conventionalHeader);
    }
}

public interface IMessageParser
{
    public Result<Message> Parse(string msg);
}