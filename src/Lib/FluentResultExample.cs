using FluentResults;

namespace commitizen.NET.Lib;

public class FluentResultExample : Error
{
    protected FluentResultExample()
    {

    }
}

public class Warning : Success
{
    protected Warning()
    {
    }

    public Warning(string message) : base(message)
    {
    }
}