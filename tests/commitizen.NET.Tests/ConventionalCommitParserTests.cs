﻿using commitizen.NET.Tests.Types;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using commitizen.NET.Lib;
using Xunit;
using FluentResults;

namespace commitizen.NET.Tests;

public class ConventionalCommitParserTests
{
    [Fact]
    public void ShouldParseTypeScopeAndSubjectFromSingleLineCommitMessage()
    {
        var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", "feat(scope): broadcast $destroy event on scope destruction");
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        conventionalCommit.Header.Type.Should().Be("feat");
        conventionalCommit.Header.Scope.Should().Be("scope");
        conventionalCommit.Header.Subject.Should().Be("broadcast $destroy event on scope destruction");
    }

    [Fact]
    public void ShouldUseFullHeaderAsSubjectIfNoTypeWasGiven()
    {
        var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", "broadcast $destroy event on scope destruction");
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        conventionalCommit.Header.Subject.Should().Be(testCommit.Message);
    }

    [Fact]
    public void ShouldUseFullHeaderAsSubjectIfNoTypeWasGivenButSubjectUsesColon()
    {
        var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", "broadcast $destroy event: on scope destruction");
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        conventionalCommit.Header.Subject.Should().Be(testCommit.Message);
    }

    [Fact]
    public void ShouldParseTypeScopeAndSubjectFromSingleLineCommitMessageIfSubjectUsesColon()
    {
        var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", "feat(scope): broadcast $destroy: event on scope destruction");
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        conventionalCommit.Header.Type.Should().Be("feat");
        conventionalCommit.Header.Scope.Should().Be("scope");
        conventionalCommit.Header.Subject.Should().Be("broadcast $destroy: event on scope destruction");
    }

    [Fact]
    public void ShouldExtractCommitNotes()
    {
        var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", "feat(scope): broadcast $destroy: event on scope destruction\nBREAKING CHANGE: this will break rc1 compatibility");
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        Assert.Single(conventionalCommit.Footers);

        var breakingChangeNote = conventionalCommit.Footers.Single();

        Assert.Equal("BREAKING CHANGE", breakingChangeNote.Title);
        Assert.Equal("this will break rc1 compatibility", breakingChangeNote.Text);
    }

    [Theory]
    [InlineData("feat(scope)!: broadcast $destroy: event on scope destruction")]
    public void ShouldSupportExclamationMarkToSignifyingBreakingChanges(string commitMessage)
    {
        var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", commitMessage);
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        conventionalCommit.Footers.Should().HaveCount(1, "single line commits can only have 1 message");
        conventionalCommit.Footers[0].Title.Should().Be("BREAKING CHANGE");
        conventionalCommit.Footers[0].Text.Should().Be(string.Empty);
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
        var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", commitMessage);
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        Assert.Equal(conventionalCommit.Issues.Count, expectedIssues.Length);

        foreach (var expectedIssue in expectedIssues)
        {
            var issue = conventionalCommit.Issues.SingleOrDefault(x => x.Id == expectedIssue);
            Assert.NotNull(issue);
            Assert.Equal(issue.Token, $"#{expectedIssue}");
        }
    }
    [Fact]
    public void BreakingChangeExclaimAndFooterShouldOnlyCreateOneFooter()
    {
        var commitMessage = """
feat!: Replace old button with new design

BREAKING CHANGE: old button is gone gone gone!!!!!!!
""";

        var testCommit = new TestCommit("", commitMessage);
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        conventionalCommit.Footers.Count.Should().Be(1);
    }

    [Fact]
    public void ShouldParseCommitMessageWithMultipleParagraphs()
    {
        var commitMessage = $"""
fix!: prevent racing of requests

Introduce a request id and a reference to latest request. Dismiss
incoming responses other than from latest request.

Remove timeouts which were used to mitigate the racing issue but are
obsolete now.

Reviewed-by: Z
456
Refs: #123
""";

        var testCommit = new TestCommit("", commitMessage);
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        conventionalCommit.Footers.Should().HaveCount(3);
    }

    [Theory]
    [InlineData("s(ui): resolve styling issues on the login page")]
    [InlineData("cd(docs): update project documentation")]

    public void ShouldFailValidationWhenHeaderTypeIsInvalid(string commitMessage)
    {
        var testCommit = new TestCommit("", commitMessage);

        Result<ConventionalCommit> conventionalCommit = ConventionalCommitParser.Validate(testCommit);

        conventionalCommit.Should().BeFailure();
    }

    [Theory]
    [InlineData("docs(): clarify installation instructions")]
    [InlineData("style(add class): format code using Prettier")]
    [InlineData("feat(api2): implement user profile endpoint")]
    public void ShouldFailValidationWhenHeaderScopeIsInvalid(string commitMessage)
    {
        var testCommit = new TestCommit("", commitMessage);

    }

    [Theory]
    [InlineData("docs(readme):")]
    [InlineData("style(css):format code using Prettier")]
    [InlineData("feat(api)implement user profile endpoint")]
    [InlineData("fix(tests) address failing unit tests")]
    public void ShouldFailValidationWhenHeaderDescriptionIsInvalid(string commitMessage)
    {
        var testCommit = new TestCommit("", commitMessage);

    }
}