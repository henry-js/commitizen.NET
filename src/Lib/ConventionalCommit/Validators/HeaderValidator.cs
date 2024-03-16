using FluentValidation;
using FluentValidation.Results;

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

public class HeaderResult
{
    public required ValidationResult Validation { get; init; }
    public required Header Value { get; init; }
    // public HeaderResult(string errorMessage) : base(errorMessage)
    // {

    // }
    // protected HeaderResult(ValidationResult validationResult) : base(validationResult)
    // {
    // }

    // public static HeaderResult FromValidation(ValidationResult validationResult)
    // {
    //     // HeaderResult result = new HeaderResult(validationResult);
    //     // return result;
    // }
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
                .WithSeverity(Enum.Parse<Severity>(defaultRules.BodyLeadingBlank.Level.ToString()));
        }
    }
}