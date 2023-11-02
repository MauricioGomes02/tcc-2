using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Exceptions;
using Tcc2.Domain.Models.Validation;

namespace Tcc2.Domain.Entities;

public class Person : IAggregateRoot
{
    public Person()
    {

    }

    public Person(
        string name,
        CompositeAddress address,
        ICollection<Activity>? activities = null,
        long? id = null)
    {
        Validate(name, address);

        Name = name;
        Address = address;
        Activities = activities ?? new List<Activity>();
        Id = id;
    }

    public string Name { get; private set; }
    public CompositeAddress Address { get; private set; }
    public ICollection<Activity> Activities { get; private set; }
    public long? Id { get; private set; }

    public void UpdateAddress(CompositeAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);
        Address = address;
    }

    private void Validate(string name, CompositeAddress address)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrWhiteSpace(name))
        {
            validationResult.Add(
                nameof(Name),
                nameof(Person),
                "Cannot be empty or null");
        }

        if (address is null)
        {
            validationResult.Add(
                nameof(Address),
                nameof(Person),
                "Cannot be null");
        }

        var addressValidation = address?.GetValidationResult();

        if (addressValidation is not null && !addressValidation.IsValid)
        {
            validationResult.AddRange(addressValidation.InvalidFields);
        }

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }
    }

    public void Validate() => Validate(Name, Address);
}
