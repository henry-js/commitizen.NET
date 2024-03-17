using System.Data;
using FluentValidation;

namespace commitizen.NET.Lib.ConventionalCommit;

public class HeaderValidator : AbstractValidator<Header>
{
    public HeaderValidator(Rules rules)
    {
        RuleFor(header => header.Type)
            .NotEmpty()
            .When(_ => rules.TypeNotEmpty.IsActive)
            .WithSeverity(Severity.Error)
            .WithMessage("type may not be empty");
        RuleFor(header => header.Type)
            .TypeMustBeOfType<Header, string>(rules.TypeEnum.Value)
            .When(_ => rules.TypeEnum.IsActive);
        RuleFor(header => header.Subject)
            .NotEmpty()
            .WithSeverity(Severity.Error)
            .WithMessage("description may not be empty");
        RuleFor(header => header.Text)
            .MaximumLength(rules.HeaderMaxLength.Value)
            .When(_ => rules.HeaderMaxLength.IsActive, ApplyConditionTo.CurrentValidator)
            .MinimumLength(rules.HeaderMinLength.Value)
            .When(_ => rules.HeaderMinLength.IsActive, ApplyConditionTo.CurrentValidator);
    }
}
