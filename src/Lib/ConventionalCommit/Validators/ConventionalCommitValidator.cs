using FluentValidation;

namespace commitizen.NET.Lib.ConventionalCommit;

public class ConventionalCommitValidator : AbstractValidator<Message>
{
    public ConventionalCommitValidator(Rules rules)
    {

        RuleFor(message => message.Header).SetValidator(new HeaderValidator(rules));
    }
}