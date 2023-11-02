using Tcc2.Domain.Exceptions;
using Tcc2.Domain.Models.Validation;

namespace Tcc2.Domain.Entities.ValueObjects;

public class Address
{
    public Address()
    {

    }

    public Address(
        string country,
        string state,
        string city,
        string neighborhood,
        string street,
        int number,
        string postalCode)
    {
        Validate(country, state, city, neighborhood, street, number, postalCode);

        Country = country;
        State = state;
        City = city;
        Neighborhood = neighborhood;
        Street = street;
        Number = number;
        PostalCode = postalCode;
    }

    public string Country { get; private set; }
    public string State { get; private set; }
    public string City { get; private set; }
    public string Neighborhood { get; private set; }
    public string Street { get; private set; }
    public int Number { get; private set; }
    public string PostalCode { get; private set; }

    private void Validate(
        string country,
        string state,
        string city,
        string neighborhood,
        string street,
        int number,
        string postalCode)
    {
        var validationResult = GetValidationResult(country, state, city, neighborhood, street, number, postalCode);

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }
    }

    private ValidationResult GetValidationResult(
        string country,
        string state,
        string city,
        string neighborhood,
        string street,
        int number,
        string postalCode)
    {
        var validationResult = new ValidationResult();
        var modelName = nameof(CompositeAddress);

        if (string.IsNullOrWhiteSpace(country))
        {
            validationResult.Add(
                nameof(Country),
                modelName,
                "Cannot be empty or null");
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            validationResult.Add(
                nameof(State),
                modelName,
                "Cannot be empty or null");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            validationResult.Add(
                nameof(City),
                modelName,
                "Cannot be empty or null");
        }

        if (string.IsNullOrWhiteSpace(neighborhood))
        {
            validationResult.Add(
                nameof(Neighborhood),
                modelName,
                "Cannot be empty or null");
        }

        if (string.IsNullOrWhiteSpace(street))
        {
            validationResult.Add(
            nameof(Street),
            modelName,
                "Cannot be empty or null");
        }
        if (number == default || number < 0)
        {
            validationResult.Add(
                nameof(Number),
                modelName,
                "Cannot be null or less than or equal to zero");
        }

        if (string.IsNullOrWhiteSpace(postalCode))
        {
            validationResult.Add(
                nameof(PostalCode),
                modelName,
                "Cannot be empty or null");
        }

        return validationResult;
    }

    internal ValidationResult GetValidationResult()
    {
        return GetValidationResult(Country, State, City, Neighborhood, Street, Number, PostalCode);
    }

    public void Validate()
    {
        Validate(Country, State, City, Neighborhood, Street, Number, PostalCode);
    }
}
