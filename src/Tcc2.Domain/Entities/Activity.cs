using Tcc2.Domain.Entities.ValueObjects;

namespace Tcc2.Domain.Entities;

public class Activity : IEntity
{
    public Activity(
        Address address,
        DateOnly date,
        TimeSpan start,
        TimeSpan end,
        string description,
        long? id = null)
    {
        Address = address;
        Date = date;
        Start = start;
        End = end;
        Description = description;
        Id = id;
    }

    public Address Address { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeSpan Start { get; private set; }
    public TimeSpan End { get; private set; }
    public string Description { get; private set; }
    public long? Id { get; private set; }
}
