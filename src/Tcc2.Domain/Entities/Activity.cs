using Tcc2.Domain.Entities.ValueObjects;

namespace Tcc2.Domain.Entities;

public class Activity : IEntity
{
    public Activity()
    {

    }

    public Activity(
        Address address,
        TimeSpan start,
        TimeSpan end,
        string description,
        long? personId = null,
        Person? person = null,
        long? id = null)
    {
        Address = address;
        ActivityDay = new List<ActivityDay>();
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

    public void AddActivityDay(ActivityDay activityDay)
    {
        ActivityDay.Add(activityDay);
    }
}
