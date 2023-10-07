using Tcc2.Application.Interfaces.Services;
using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Validation;

namespace Tcc2.Application.Services;

public class ActivityValidationService : IActivityValidationService
{
    private readonly IAddressValidationService _addressValidationService;
    private readonly TimeSpan _maximumTime = new(0, 23, 59, 59, 999);

    public ActivityValidationService(IAddressValidationService addressValidationService)
    {
        _addressValidationService = addressValidationService;
    }

    public ValidationResult Validate(ActivityInput? activity)
    {
        var validationResult = new ValidationResult();
        var modelName = nameof(ActivityInput);

        if (activity is null)
        {
            validationResult.Add(
                nameof(activity),
                modelName,
                "Cannot be null");

            return validationResult;
        }

        if (activity.DaysOfWeek is null || !activity.DaysOfWeek.Any())
        {
            validationResult.Add(
                nameof(activity.DaysOfWeek),
                modelName,
                $"Cannot be empty or null");
        }

        if (activity.Start < TimeSpan.Zero || activity.Start > _maximumTime)
        {
            validationResult.Add(
                nameof(activity.Start),
                modelName,
                $"Cannot be less than {TimeSpan.Zero} or greater than {_maximumTime}");
        }

        if (activity.End < TimeSpan.Zero || activity.End > _maximumTime)
        {
            validationResult.Add(
                nameof(activity.End),
                modelName,
                $"Cannot be less than {TimeSpan.Zero} or greater than {_maximumTime}");
        }

        if (activity.Start >= activity.End)
        {
            validationResult.Add(
               nameof(activity.Start),
               modelName,
               $"Cannot be greater than or equal to {activity.End}");
        }

        if (string.IsNullOrWhiteSpace(activity.Description))
        {
            validationResult.Add(
               nameof(activity.Description),
               modelName,
               $"Cannot be empty or null");
        }

        var addressValidation = _addressValidationService.Validate(activity.Address);

        if (!addressValidation.IsValid)
        {
            validationResult.AddRange(addressValidation.InvalidFields);
        }

        return validationResult;
    }
}
