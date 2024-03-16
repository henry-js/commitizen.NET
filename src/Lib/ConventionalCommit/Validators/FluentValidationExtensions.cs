using FluentValidation;

namespace commitizen.NET.Lib.ConventionalCommit;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<Header, string> TypeMustBeOfType<Header, TElement>(this IRuleBuilder<Header, string> ruleBuilder, IEnumerable<string> types)
    {
        return ruleBuilder.Must((rootObject, type, context) =>
        {
            context.MessageFormatter.AppendArgument("Types", $"[{string.Join(", ", types)}]");
            return types.Contains(type);
        })
        .WithMessage("{PropertyName} must be of {Types}")
        .WithSeverity(Severity.Error);
    }

    // public static IRuleBuilderOptions<
}