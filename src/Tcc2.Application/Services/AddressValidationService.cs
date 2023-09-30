using Tcc2.Application.Interfaces.Services;
using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Validation;

namespace Tcc2.Application.Services;

public class AddressValidationService : IAddressValidationService
{
    public ValidationResult Validate(AddressInput address)
    {
        var validationResult = new ValidationResult();

        var modelName = nameof(AddressInput);

        if (string.IsNullOrWhiteSpace(address.Country))
        {
            validationResult.Add(
                nameof(address.Country),
                modelName,
                "Cannot be empty or null");
        }

        if (string.IsNullOrWhiteSpace(address.State))
        {
            validationResult.Add(
                nameof(address.State),
                modelName,
                "Cannot be empty or null");
        }

        if (string.IsNullOrWhiteSpace(address.City))
        {
            validationResult.Add(
                nameof(address.City),
                modelName,
                "Cannot be empty or null");
        }

        if (string.IsNullOrWhiteSpace(address.Neighborhood))
        {
            validationResult.Add(
                nameof(address.Neighborhood),
                modelName,
                "Cannot be empty or null");
        }

        if (string.IsNullOrWhiteSpace(address.Street))
        {
            validationResult.Add(
                nameof(address.Street),
                modelName,
                "Cannot be empty or null");
        }

        if (address.Number == default || address.Number < 0)
        {
            validationResult.Add(
                nameof(address.Number),
                modelName,
                "Cannot be null or less than or equal to zero");
        }

        if (address.PostalCode is not null && address.PostalCode.Trim() == string.Empty)
        {
            validationResult.Add(
                nameof(address.State),
                modelName,
                "Cannot be empty");
        }

        return validationResult;
    }
}
