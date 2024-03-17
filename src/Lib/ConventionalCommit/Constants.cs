namespace commitizen.NET.Lib.ConventionalCommit;

public static class Constants
{
    public const string CorrectFormat = """
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
""";
    public static readonly string[] lineFeeds = ["\n", "\r\n", "\r"];

}
public static class RegexConstants
{
    public const string token = "token";
    public const string separator = "separator";
    public const string value = "value";
}