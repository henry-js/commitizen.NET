using commitizen.NET.Lib;

namespace commitizen.NET.Tests.Types;

public class TestCommit(string sha, string message)
    : Commit(sha, message)
{ }
