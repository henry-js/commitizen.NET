using FluentValidation;

namespace commitizen.NET.Lib.Validators;

public class ConventionalCommitValidator : AbstractValidator<ConventionalCommit>
{
    public ConventionalCommitValidator(LintingSettings settings)
    {
        RuleFor(commit => commit.Header).SetValidator(new HeaderValidator(settings));
    }
}