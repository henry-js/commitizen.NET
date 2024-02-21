using FluentResults;

namespace commitizen.NET.Lib;

public record ValidationResult
{
    public List<Error> Errors { get; } = [];
    public List<Warning> Warnings { get; } = [];

}