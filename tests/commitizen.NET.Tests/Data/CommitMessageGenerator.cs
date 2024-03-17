using System.Collections;
using Bogus;

public class CommitMessageData
{
    private static readonly IEnumerable<string> _types = ["chore", "build", "ci", "docs", "feat", "fix", "perf", "refactor", "revert", "style", "test", "types"];
    private static readonly Faker _faker;

    static CommitMessageData()
    {
        Randomizer.Seed = new Random(12345);
        _faker = new Faker("en");
    }

    public static TheoryData<string> CommitMessageWithBodyAndFooter()
    {
        var theoryData = new TheoryData<string>();
        foreach (var _ in Enumerable.Range(1, 10))
        {
            string message = $"""
{_faker.PickRandom(_types)}: {_faker.Hacker.Phrase()}

- {_faker.Hacker.Noun()} this.

- {_faker.Hacker.Verb()} that.

- {_faker.Hacker.IngVerb()} the widget.

- {_faker.Hacker.Phrase()}

Resolves #{_faker.Random.Number(111, 9999)}
Entered-by: {_faker.Person.UserName}
""";
            theoryData.Add(message);
        }

        return theoryData;
    }
}
