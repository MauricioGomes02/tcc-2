using Tcc2.Application.Interfaces.Services;
using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Validation;

namespace Tcc2.Application.Services;

public class PersonValidationService : IPersonValidationService
{
    private readonly IAddressValidationService _addressValidatorService;

    public PersonValidationService(IAddressValidationService addressValidatorService)
    {
        _addressValidatorService = addressValidatorService;
    }

    public ValidationResult Validate(PersonInput person)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrWhiteSpace(person.Name))
        {
            validationResult.Add(
                nameof(person.Name),
                nameof(PersonInput),
                "Cannot be empty or null");
        }

        var addressValidation = _addressValidatorService.Validate(person.Address);

        if (!addressValidation.IsValid)
        {
            validationResult.AddRange(addressValidation.InvalidFields);
        }

        return validationResult;
    }
}