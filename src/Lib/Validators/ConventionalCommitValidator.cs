using FluentValidation;

namespace commitizen.NET.Lib.Validators;

public class ConventionalCommitValidator : AbstractValidator<ConventionalCommit>
{
    public ConventionalCommitValidator(Rules rules)
    {
        RuleFor(commit => commit.Header).SetValidator(new HeaderValidator(rules));
    }
}