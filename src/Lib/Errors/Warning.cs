using FluentResults;

namespace commitizen.NET.Lib.Errors;

public class Warning : Success
{
    protected Warning()
    {
    }

    public Warning(string message) : base(message)
    {
    }
}