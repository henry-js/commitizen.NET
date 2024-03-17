using commitizen.NET.Lib;
using commitizen.NET.Lib.ConventionalCommit;
using FluentAssertions;
using FluentResults;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace commitizen.NET.Tests;

public class ConventionalCommitParserTests
{

    private readonly IOptions<Rules> defaultRules = default!;

    public ConventionalCommitParserTests()
    {
        defaultRules = Options.Create(TestConfigHelper.GetIConfigurationRoot()
            .GetRequiredSection(Rules.Key).Get<Rules>() ?? throw new ArgumentNullException());
    }

    [Fact]
    public void ShouldParseTypeScopeAndSubjectFromSingleLineCommitMessage()
    {

        var parser = new MessageParser(defaultRules);
        var msg = "feat(scope): broadcast $destroy event on scope destruction";
        var result = parser.Parse(msg);
        var conventionalCommit = result.Value;

        conventionalCommit.Header.Type.Should().Be("feat");
        conventionalCommit.Header.Scope.Should().Be("scope");
        conventionalCommit.Header.Subject.Should().Be("broadcast $destroy event on scope destruction");
    }

    [Fact]
    public void ShouldParseTypeScopeAndSubjectFromSingleLineCommitMessageIfSubjectUsesColon()
    {
        var parser = new MessageParser(defaultRules);

        var message = "feat(scope): broadcast $destroy: event on scope destruction";
        // var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", message);
        var result = parser.Parse(message);
        var conventionalCommit = result.Value;

        conventionalCommit.Header.Type.Should().Be("feat");
        conventionalCommit.Header.Scope.Should().Be("scope");
        conventionalCommit.Header.Subject.Should().Be("broadcast $destroy: event on scope destruction");
    }

    [Theory]
    [InlineData("s(ui): resolve styling issues on the login page")]
    [InlineData("cd(docs): update project documentation")]

    public void ShouldFailValidationWhenHeaderTypeIsInvalid(string commitMessage)
    {
        // var testCommit = new TestCommit("", commitMessage);
        var parser = new MessageParser(defaultRules);

        Result<Message> conventionalCommit = parser.Parse(commitMessage);

        conventionalCommit.Should().BeFailure();
    }

    [Theory]
    [InlineData("feat(scope)!: broadcast $destroy: event on scope destruction")]
    public void ShouldSupportExclamationMarkToSignifyingBreakingChanges(string commitMessage)
    {
        var parser = new MessageParser(defaultRules);

        var result = parser.Parse(commitMessage);
        var conventionalCommit = result.Value;

        conventionalCommit.IsBreakingChange.Should().BeTrue();
    }

    // TODO: Update to support issue syntax from other platforms i.e. Jira ('VE-####')
    [Theory]
    [InlineData("fix: subject text #64", new[] { "64" })]
    [InlineData("fix: subject #64 text", new[] { "64" })]
    [InlineData("fix: #64 subject text", new[] { "64" })]
    [InlineData("fix: subject text. #64 #65", new[] { "64", "65" })]
    [InlineData("fix: subject text. (#64) (#65)", new[] { "64", "65" })]
    [InlineData("fix: subject text. #64#65", new[] { "64", "65" })]
    [InlineData("fix: #64 subject #65 text. (#66)", new[] { "64", "65", "66" })]
    public void ShouldExtractCommitIssues(string commitMessage, string[] expectedIssues)
    {
        var parser = new MessageParser(defaultRules);

        var result = parser.Parse(commitMessage);
        var conventionalCommit = result.Value;

        conventionalCommit.Issues.Count.Should().Be(expectedIssues.Length);

        foreach (var expectedIssue in expectedIssues)
        {
            var issue = conventionalCommit.Issues.SingleOrDefault(x => x.Id == expectedIssue);
            issue.Should().NotBeNull();
            issue?.Token.Should().Be($"#{expectedIssue}");
        }
    }

    // [Theory]
    // [InlineData("docs(): clarify installation instructions")]
    // [InlineData("style(add class): format code using Prettier")]
    // [InlineData("feat(api2): implement user profile endpoint")]
    // public void ShouldFailValidationWhenHeaderScopeIsInvalid(string commitMessage)
    // {
    //         var testCommit = new TestCommit("", commitMessage);
    //         var parser = new ConventionalCommitParser(defaultSettings);

    //         var conventionalCommit = parser.Validate(testCommit);
    // }

    [Theory]
    [InlineData("docs(readme):")]
    [InlineData("style(css):format code using Prettier")]
    [InlineData("feat(api)implement user profile endpoint")]
    [InlineData("fix(tests) address failing unit tests")]
    public void ShouldFailValidationWhenHeaderDescriptionIsInvalid(string commitMessage)
    {
        var parser = new MessageParser(defaultRules);

        var result = parser.Parse(commitMessage);

        result.Should().BeFailure();
    }

    [Theory]
    [MemberData(nameof(CommitMessageData.CommitMessageWithBodyAndFooter), MemberType = typeof(CommitMessageData))]
    public void ShouldParseMultiLineCommitMessages(string commitMessage)
    {
        var parser = new MessageParser(defaultRules);

        Result<Message> result = parser.Parse(commitMessage);

        result.IsSuccess.Should().BeTrue();
    }
}
