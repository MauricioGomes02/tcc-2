using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Exceptions;
using Tcc2.Domain.Models.Validation;

namespace Tcc2.Domain.Entities;

public class Activity : IEntity
{
    private readonly TimeSpan _maximumTime = new(0, 23, 59, 59, 999);

    public Activity()
    {

    }

    public Activity(
        Address address,
        TimeSpan start,
        TimeSpan end,
        IEnumerable<short> daysOfWeek,
        string description,
        long? personId = null,
        Person? person = null,
        long? id = null)
    {
        Validate(address, start, end, daysOfWeek, description);

        Address = address;
        ActivityDay = daysOfWeek.Select(x => new ActivityDay(this, x)).ToList();
        Start = start;
        End = end;
        PersonId = personId;
        Person = person;
        Description = description;
        Id = id;
    }

    public Address Address { get; private set; }
    public ICollection<ActivityDay> ActivityDay { get; private set; }
    public TimeSpan Start { get; private set; }
    public TimeSpan End { get; private set; }
    public string Description { get; private set; }
    public long? PersonId { get; private set; }
    public Person? Person { get; private set; }
    public long? Id { get; private set; }

    internal void UpdatePersonId(long personId)
    {
        PersonId = personId;
    }

    private void Validate(
        Address address,
        TimeSpan start,
        TimeSpan end,
        IEnumerable<short> daysOfWeek,
        string description)
    {
        var validationResult = GetValidationResult(address, start, end, daysOfWeek, description);

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }
    }

    private ValidationResult GetValidationResult(
        Address address,
        TimeSpan start,
        TimeSpan end,
        IEnumerable<short> daysOfWeek,
        string description)
    {
        var validationResult = new ValidationResult();
        var modelName = nameof(Activity);

        if (start < TimeSpan.Zero || start > _maximumTime)
        {
            validationResult.Add(
                nameof(Start),
                modelName,
                $"Cannot be less than {TimeSpan.Zero} or greater than {_maximumTime}");
        }
        if (end < TimeSpan.Zero || end > _maximumTime)
        {
            validationResult.Add(
                nameof(End),
                modelName,
                $"Cannot be less than {TimeSpan.Zero} or greater than {_maximumTime}");
        }

        if (start >= end)
        {
            validationResult.Add(
                nameof(Start),
                modelName,
                $"Cannot be greater than or equal to {End}");
        }

        if (daysOfWeek is null || !daysOfWeek.Any())
        {
            validationResult.Add(
                nameof(ActivityDay),
                modelName,
                $"Cannot be empty or null");
        }
        else if (daysOfWeek.Count() > 7)
        {
            validationResult.Add(
                nameof(ActivityDay),
                modelName,
                $"A week has a maximum of 7 days");
        }
        else if (daysOfWeek.GroupBy(x => x).Any(x => x.Count() > 1))
        {
            validationResult.Add(
                nameof(ActivityDay),
                modelName,
                $"The days of the week cannot be repeated");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            validationResult.Add(
               nameof(Description),
               modelName,
               $"Cannot be empty or null");
        }

        if (address is null)
        {
            validationResult.Add(
                nameof(Address),
                modelName,
                "Cannot be null");
        }

        var addressValidation = address?.GetValidationResult();

        if (addressValidation is not null && !addressValidation.IsValid)
        {
            validationResult.AddRange(addressValidation.InvalidFields);
        }

        return validationResult;
    }

    public void Validate() => Validate(
        Address,
        Start,
        End,
        ActivityDay?.Select(x => x.DayId) ?? Enumerable.Empty<short>(),
        Description);
}
