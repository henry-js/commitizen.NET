using Lib;

namespace commitizen.NET.Tests.Types;

public class TestCommit : Commit
{
    private readonly string _sha;
    private readonly string _message;

    public TestCommit(string sha, string message) : base(sha, message)
    {
    }
    // public static TestCommit Create(string sha, string message)
    // {
    //     var tc = new TestCommit()
    // }

    public string MessageLines { get => _message; }

    public override string Sha { get => _sha; }
}
