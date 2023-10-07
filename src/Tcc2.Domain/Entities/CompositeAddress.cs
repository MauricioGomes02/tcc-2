namespace Tcc2.Domain.Entities.ValueObjects;
public class CompositeAddress : Address, IEntity
{
    public CompositeAddress()
    {

    }

    public CompositeAddress(
        string country,
        string state,
        string city,
        string neighborhood,
        string street,
        int number,
        string postalCode,
        GeographicCoordinate? geographicCoordinate = null,
        long? personId = null,
        Person? person = null,
        long? id = null) : base(country, state, city, neighborhood, street, number, postalCode)
    {
        GeographicCoordinate = geographicCoordinate;
        PersonId = personId;
        Person = person;
        Id = id;
    }

    public GeographicCoordinate? GeographicCoordinate { get; private set; }
    public long? PersonId { get; private set; }
    public long? Id { get; private set; }
    public Person? Person { get; private set; }

    internal void AddGeographicCoordinate(GeographicCoordinate geographicCoordinate)
    {
        GeographicCoordinate = geographicCoordinate;
    }

    public override string ToString()
    {
        return $"{Street}, {Number} - {Neighborhood}, {City} - {State}, {Country}";
    }
}
