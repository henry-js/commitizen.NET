using commitlint.NET.Tests.Types;
using FluentAssertions;
using Xunit;

namespace commitlint.NET.Tests;

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

        conventionalCommit.Subject.Should().Be(testCommit.MessageLines);
    }

    [Fact]
    public void ShouldUseFullHeaderAsSubjectIfNoTypeWasGivenButSubjectUsesColon()
    {
        var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", "broadcast $destroy event: on scope destruction");
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        conventionalCommit.Subject.Should().Be(testCommit.MessageLines);
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

        Assert.Single(conventionalCommit.Notes);

        var breakingChangeNote = conventionalCommit.Notes.Single();

        Assert.Equal("BREAKING CHANGE", breakingChangeNote.Title);
        Assert.Equal("this will break rc1 compatibility", breakingChangeNote.Text);
    }

    [Theory]
    [InlineData("feat!: broadcast $destroy: event on scope destruction")]
    [InlineData("feat(scope)!: broadcast $destroy: event on scope destruction")]
    public void ShouldSupportExclamationMarkToSignifyingBreakingChanges(string commitMessage)
    {
        var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", commitMessage);
        var conventionalCommit = ConventionalCommitParser.Parse(testCommit);

        conventionalCommit.Notes.Should().HaveCount(1, "single line commits can only have 1 message");
        conventionalCommit.Notes[0].Title.Should().Be("BREAKING CHANGE");
        conventionalCommit.Notes[0].Text.Should().Be(string.Empty);
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
}
