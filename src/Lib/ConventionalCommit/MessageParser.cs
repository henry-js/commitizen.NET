using commitizen.NET.Lib.Errors;
using static commitizen.NET.Lib.ConventionalCommit.DefaultPatterns;
using FluentResults;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace commitizen.NET.Lib.ConventionalCommit;

public partial class MessageParser : IMessageParser
{
    public MessageParser(IOptions<Rules> defaultRules)
    {
        Rules = defaultRules.Value;
    }

    private Rules Rules { get; }

    public Result<Message> Parse(string msg)
    {
        string[] msgParts = NewLinePattern.Split(msg, 2);
        var header = new Header(msgParts[0]);
        var body = msgParts.Length == 2 ? new Body(msgParts[1]) : Body.Empty();
        var message = new Message { Text = msg, Header = header, Body = body };

        FindIssues(message);
        var validator = new MessageValidator(Rules);

        var result = validator.Validate(message);

        if (!result.IsValid)
        {
            return Result.Fail(result.Errors.Select(x => new ConventionalCommitValidationError(x)));
        }

        return Result.Ok(message);
    }


    private void FindIssues(Message message)
    {
        var issuesMatch = IssuesPattern.Matches(message.Text);

        foreach (var issueMatch in issuesMatch.Cast<Match>())
        {
            message.Issues.Add(
                new ConventionalCommitIssue
                {
                    Token = issueMatch.Groups["issueToken"].Value,
                    Id = issueMatch.Groups["issueId"].Value,
                });
        }
    }
}

public interface IMessageParser
{
    public Result<Message> Parse(string msg);
}