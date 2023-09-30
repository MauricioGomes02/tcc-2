using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Validation;

namespace Tcc2.Application.Interfaces.Services;

public interface IAddressValidationService
{
    ValidationResult Validate(AddressInput address);
}
