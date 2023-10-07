using Tcc2.Domain.Entities.ValueObjects;

namespace Tcc2.Domain.Entities;

public class Person : IAggregateRoot
{
    public Person()
    {

    }

    public Person(string name, CompositeAddress address, ICollection<Activity>? activities = null, long? id = null)
    {
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
}
