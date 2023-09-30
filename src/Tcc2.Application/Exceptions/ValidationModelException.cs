using Tcc2.Application.Models.Validation;

namespace Tcc2.Application.Exceptions;

public class ValidationModelException : Exception
{
    private readonly ValidationResult _validationResult;

    public ValidationModelException(string? message, ValidationResult validationResult) : base(message)
    {
        _validationResult = validationResult;
    }

    public ValidationModelException(string? message, ValidationResult validationResult, Exception? innerException) : base(message, innerException)
    {
        _validationResult = validationResult;
    }

    public ValidationResult ValidationResult => _validationResult;
}
