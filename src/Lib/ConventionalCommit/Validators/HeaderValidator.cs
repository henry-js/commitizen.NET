using FluentValidation;

namespace commitizen.NET.Lib.ConventionalCommit;

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

public class CommitBodyValidator : AbstractValidator<Body>
{
    public CommitBodyValidator(Rules defaultRules)
    {
        RuleFor(body => body)
            .NotEmpty();
        if (defaultRules.BodyLeadingBlank.State == State.on)
        {
            RuleFor(body => body.Text.Split(Constants.lineFeeds, StringSplitOptions.None)[0])
                .Must(firstLine => string.Empty == firstLine)
                .WithSeverity(MapSeverity(defaultRules.BodyLeadingBlank.Level));
        }
    }

    private static Severity MapSeverity(ErrorLevel level) => level switch
    {
        ErrorLevel.error => Severity.Error,
        ErrorLevel.warning => Severity.Warning,
        ErrorLevel.disable => Severity.Info,
        _ => Severity.Info
    };
}