using LibGit2Sharp;

namespace Lib;

public class Commit : LibGit2Sharp.Commit, ICommit
{
    protected readonly string _sha;
    protected readonly string[] _messageLines;
    protected readonly string _message;

    public Commit(string sha, string message)
    {
        _sha = sha;
        _message = message;
        _messageLines = message.Split(Environment.NewLine,
                StringSplitOptions.None);
    }

    public override string Message => _message;
    public string[] MessageLines => _messageLines;

}

public interface ICommit
{

}