using commitizen.NET.Lib;
using FluentValidation;

namespace commitizen.NET.Lib.Validators;

public class HeaderValidator : AbstractValidator<Header>
{
    public HeaderValidator(Rules defaultRules)
    {
        RuleFor(header => header.Type)
            .NotEmpty()
            .WithSeverity(Severity.Error)
            .WithMessage("type may not be empty");
        RuleFor(header => header.Subject)
            .NotEmpty()
            .WithSeverity(Severity.Error)
            .WithMessage("description may not be empty");

        RuleFor(header => header.Type)
            .TypeMustBeOfType<Header, string>(defaultRules.TypeEnum.Value);
    }

}
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
}