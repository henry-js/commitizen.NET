using FluentValidation;

namespace commitizen.NET.Lib.ConventionalCommit;

public class MessageValidator : AbstractValidator<Message>
{
    public MessageValidator(Rules rules)
    {
        RuleFor(message => message.Header).SetValidator(new HeaderValidator(rules));
        RuleFor(message => message.Body).SetValidator(new BodyValidator(rules));
    }
}