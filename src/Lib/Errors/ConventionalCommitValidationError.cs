using FluentResults;
using FluentValidation.Results;

namespace commitizen.NET.Lib.Errors;

public class ConventionalCommitValidationError : Error
{
    public ConventionalCommitValidationError(ValidationFailure failure) : base(failure.ErrorMessage)
    {
        Metadata.Add("Severity", failure.Severity);
    }
}