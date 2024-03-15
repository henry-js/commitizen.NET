using System.Collections;
using Bogus;

public class CommitMessageGenerator : IEnumerable<object[]>
{
    private readonly IEnumerable<string> _types;
    private readonly int _count;

    public static IEnumerable<string> Generate(IEnumerable<string> types, int count = 10)
    {
        Randomizer.Seed = new Random(12345);
        var faker = new Faker("en");
        var messages = new List<string>();
        foreach (var i in Enumerable.Range(1, count))
        {
            string message = $"""
{faker.PickRandom(types)}: {faker.Lorem.Sentence(3, 2)}

{faker.Lorem.Sentence(7, 5)}

{faker.Lorem.Sentence(6, 3)}

Resolves #{faker.Random.Number(111, 9999)}
Entered-by: {faker.Person.UserName}
""";
            messages.Add(message);
        }
        return messages;

    }
    public CommitMessageGenerator()
    {
        _types = ["chore", "build", "ci", "docs", "feat", "fix", "perf", "refactor", "revert", "style", "test", "types"];
        _count = 10;
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        Randomizer.Seed = new Random(12345);
        var faker = new Faker("en");
        foreach (var i in Enumerable.Range(1, _count))
        {
            string message = $"""
{faker.PickRandom(_types)}: {faker.Lorem.Sentence(3, 2)}

{faker.Lorem.Sentence(7, 5)}

{faker.Lorem.Sentence(6, 3)}

Resolves #{faker.Random.Number(111, 9999)}
Entered-by: {faker.Person.UserName}
""";
            yield return [message];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}