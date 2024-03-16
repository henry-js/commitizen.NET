using static commitizen.NET.Lib.ConventionalCommit.DefaultPatterns;
using FluentResults;
using System.Text.RegularExpressions;
using commitizen.NET.Lib.Errors;
using Microsoft.Extensions.Options;
using FluentValidation.Results;

namespace commitizen.NET.Lib.ConventionalCommit;

public class MessageParser : IMessageParser
{
    public MessageParser(IOptions<Rules> defaultSettings)
    {
        Rules = defaultSettings.Value;
    }
    private readonly string[] lineFeeds = ["\n", "\r\n", "\r"];

    private readonly string[] NoteKeywords = ["BREAKING CHANGE"];

    private Rules Rules { get; }

    public Result<Message> Parse(string msg)
    {
        var nlRegex = new Regex(@"\r?\n|\r");
        string[] msgParts = nlRegex.Split(msg, 2);
        string header = msgParts[0];
        var headerResult = ParseHeader(header);
        if (!headerResult.Validation.IsValid)
        {
            return Result.Fail(headerResult.Validation.Errors.Select(x => new ConventionalCommitValidationError(x)));
        }
        if (msgParts.Length == 1)
        {
            var commit = new Message() { Header = headerResult.Value, Original = msg };
            return Result.Ok(commit);
        }

        var bodyResult = ParseBody(msgParts[^1]);

        if (bodyResult.result.IsValid)
        {
            var commit = new Message() { Header = headerResult.Value, Original = msg, Body = bodyResult.body };
            return Result.Ok(commit);
        }
        return Result.Ok();
    }

    private Footer? ParseFooter(string input)
    {
        var matches = FooterPattern.Matches(input);
        if (matches.Count() == 0)
            return null;

        var footer = new Footer();
        for (int i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var token = match.Groups[RegexConstants.token];
            var separator = match.Groups[RegexConstants.separator];
            var valueStartIndex = separator.Index + separator.Value.Length;
            var value = i + 1 == matches.Count ? input[valueStartIndex..] : input[valueStartIndex..matches[i + 1].Groups[RegexConstants.token].Index];
            footer.Values.Add(new(token.Value, separator.Value, value));
        }

        return footer;
    }

    private bool HasFooter(string msg, out int startIndex)
    {
        var matches = FooterPattern.Matches(msg);
        if (matches.Count == 0)
        {
            startIndex = -1;
            return false;
        }
        startIndex = matches.First().Index;
        return true;
    }

    private (ValidationResult result, Body body) ParseBody(string msgRemaining)
    {
        Body body = new();
        string? footerText;
        string bodyText;
        if (HasFooter(msgRemaining, out int startIndex))
        {
            footerText = msgRemaining[startIndex..];
            bodyText = msgRemaining[..startIndex];
            var footer = ParseFooter(msgRemaining);
            body = new Body { Text = bodyText, Footers = footer?.Values, FooterText = footerText };
        }
        else
        {
            body = new Body { Text = msgRemaining };
        }

        var validator = new CommitBodyValidator(Rules);

        var vr = validator.Validate(body);

        return (vr, body);
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

    private HeaderResult ParseHeader(string header)
    {
        var match = HeaderPattern.Match(header);

        if (!match.Success) return new HeaderResult() { Validation = new ValidationResult(), Value = Header.Empty() };

        Header conventionalHeader = new()
        {
            Raw = header,
            Type = match.Groups["type"].Value,
            Scope = match.Groups["scope"].Value,
            Subject = match.Groups["subject"].Value,
            IsBreakingChange = match.Groups["breakingChangeMarker"].Success
        };

        var validator = new HeaderValidator(Rules);
        var results = validator.Validate(conventionalHeader);
        return new HeaderResult() { Validation = results, Value = conventionalHeader };
    }
}

public interface IMessageParser
{
    public Result<Message> Parse(string msg);
}