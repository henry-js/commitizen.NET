using FluentValidation;

namespace commitizen.NET.Lib.ConventionalCommit;

public class BodyValidator : AbstractValidator<Body>
{
    public BodyValidator(Rules defaultRules)
    {
        if (defaultRules.BodyEmpty.State == State.on)
        {
            RuleFor(body => body)
                .NotEmpty();
        }
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