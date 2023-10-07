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

    public override string ToString()
    {
        return $"{Street}, {Number} - {Neighborhood}, {City} - {State}, {Country}";
    }
}
