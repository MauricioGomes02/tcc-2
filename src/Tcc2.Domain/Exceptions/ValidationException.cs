using Tcc2.Domain.Models.Validation;

namespace Tcc2.Domain.Exceptions;

public class ValidationException : Exception
{
    private readonly ValidationResult _validationResult;

    public ValidationException(string? message, ValidationResult validationResult) : base(message)
    {
        _validationResult = validationResult;
    }

    public ValidationException(string? message, ValidationResult validationResult, Exception? innerException) : base(message, innerException)
    {
        _validationResult = validationResult;
    }

    public ValidationResult ValidationResult => _validationResult;
}
