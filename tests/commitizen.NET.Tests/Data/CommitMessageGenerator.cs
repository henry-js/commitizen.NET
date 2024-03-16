using System.Collections;
using Bogus;

public class CommitMessageGenerator : IEnumerable<object[]>
{
    private static readonly IEnumerable<string> _types = ["chore", "build", "ci", "docs", "feat", "fix", "perf", "refactor", "revert", "style", "test", "types"];
    private readonly int _count;
    private static Faker _faker;

    static CommitMessageGenerator()
    {
        Randomizer.Seed = new Random(12345);
        _faker = new Faker("en");
    }

    public static string GenerateMessageWithBodyAndFooter()
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
        return message;
    }

    public IEnumerator<object[]> GetEnumerator()
    {

        foreach (var i in Enumerable.Range(1, count: 10))
        {
            string message = GenerateMessageWithBodyAndFooter();
            yield return [message];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}