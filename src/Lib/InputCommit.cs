namespace commitizen.NET.Lib;

public class Commit //: LibGit2Sharp.Commit, ICommit
{
    protected readonly string _sha;
    protected readonly string[] _messageLines;
    protected readonly string _message;
    private static readonly string[] lineFeed = [
        $"{Environment.NewLine}{Environment.NewLine}"
        ];
    public Commit(string sha, string message)
    {
        _sha = sha;
        _message = message;
        _messageLines = message.Split(lineFeed,
                StringSplitOptions.None);
    }

    public string Message => _message;
    public string[] MessageLines => _messageLines;

}

public interface ICommit
{

}