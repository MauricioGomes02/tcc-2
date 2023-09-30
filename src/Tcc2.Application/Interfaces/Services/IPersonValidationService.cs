using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Validation;

namespace Tcc2.Application.Interfaces.Services;

public interface IPersonValidationService
{
    ValidationResult Validate(PersonInput person);
}
