using commitizen.NET.Lib;
using FluentValidation;

namespace commitizen.NET.Lib.Validators;

public class HeaderValidator : AbstractValidator<Header>
{
    public HeaderValidator(LintingSettings lintingSettings)
    {
        RuleFor(header => header.Type)
            .NotEmpty()
            .WithMessage("❌ type may not be empty");
        RuleFor(header => header.Subject)
            .NotEmpty()
            .WithMessage("❌ description may not be empty");

        RuleFor(header => header.Type)
            .TypeMustBeOfType<Header, string>(lintingSettings.Types);
    }

}
public static class FluentValidationExtensions
{

    public static IRuleBuilderOptions<Header, string> TypeMustBeOfType<Header, TElement>(this IRuleBuilder<Header, string> ruleBuilder, Dictionary<string, CommitTypeInfo> types)
    {
        return ruleBuilder.Must((rootObject, type, context) =>
        {
            context.MessageFormatter.AppendArgument("Types", $"[{string.Join(", ", types.Keys)}]");
            return types.ContainsKey(type);
        })
        .WithMessage("{PropertyName} must be of {Types}");
    }
}