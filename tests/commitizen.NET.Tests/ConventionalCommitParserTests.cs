using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using commitizen.NET.Lib;
using FluentResults;
using Microsoft.Extensions.Configuration;

namespace commitizen.NET.Tests;

public class ConventionalCommitParserTests
{

        private readonly Rules defaultRules = default!;

        public ConventionalCommitParserTests()
        {
                defaultRules = TestConfigHelper.GetIConfigurationRoot()
                    .GetRequiredSection(Rules.Key).Get<Rules>() ?? throw new ArgumentNullException();
        }

        [Fact]
        public void ShouldParseTypeScopeAndSubjectFromSingleLineCommitMessage()
        {
                var parser = new ConventionalCommitParser(defaultRules);
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
                var parser = new ConventionalCommitParser(defaultRules);

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
                var parser = new ConventionalCommitParser(defaultRules);

                Result<ConventionalCommit> conventionalCommit = parser.Parse(commitMessage);

                conventionalCommit.Should().BeFailure();
        }

        //         [Fact]
        //         public void ShouldExtractCommitNotes()
        //         {
        //                 var parser = new ConventionalCommitParser(defaultSettings);
        //                 string msg = """
        // feat(scope): broadcast $destroy: event on scope destruction

        // BREAKING CHANGE: this will break rc1 compatibility
        // """;
        //                 // var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", msg);

        //                 var result = parser.Validate(msg);
        //                 var conventionalCommit = result.Value;

        //                 Assert.Single(conventionalCommit.Footers);

        //                 var breakingChangeNote = conventionalCommit.Footers.Single();

        //                 Assert.Equal("BREAKING CHANGE", breakingChangeNote.Title);
        //                 Assert.Equal("this will break rc1 compatibility", breakingChangeNote.Text);
        //         }

        [Theory]
        [InlineData("feat(scope)!: broadcast $destroy: event on scope destruction")]
        public void ShouldSupportExclamationMarkToSignifyingBreakingChanges(string commitMessage)
        {
                var parser = new ConventionalCommitParser(defaultRules);

                // var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", commitMessage);
                var result = parser.Parse(commitMessage);
                var conventionalCommit = result.Value;

                result.Value.IsBreakingChange.Should().BeTrue();

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
                var parser = new ConventionalCommitParser(defaultRules);

                // var testCommit = new TestCommit("c360d6a307909c6e571b29d4a329fd786c5d4543", commitMessage);
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
        // //         [Fact]
        // //         public void BreakingChangeExclaimAndFooterShouldOnlyCreateOneFooter()
        // //         {
        // //                 var commitMessage = """
        // // feat!: Replace old button with new design

        // // BREAKING CHANGE: old button is gone gone gone!!!!!!!
        // // """;

        // //                 var parser = new ConventionalCommitParser(defaultSettings);
        // //                 var testCommit = new TestCommit("", commitMessage);

        // //                 var result = parser.Validate(testCommit);
        // //                 var conventionalCommit = result.Value;

        // //                 //Assert
        // //                 conventionalCommit.Footers.Count.Should().Be(1);
        // //         }



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
                // var testCommit = new TestCommit("", commitMessage);
                var parser = new ConventionalCommitParser(defaultRules);

                var result = parser.Parse(msg: commitMessage);

                result.Should().BeFailure();
        }
}