using Tcc2.Domain.Entities.ValueObjects;

namespace Tcc2.Domain.Entities;

public class Person : IAggregateRoot
{
    public Person()
    {

    }

    public Person(string name, Address address, long? id = null)
    {
        Name = name;
        Address = address;
        Id = id;
    }

    public string Name { get; private set; }
    public Address Address { get; private set; }
    public long? Id { get; private set; }

    public void UpdateAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);
        Address = address;
    }
}
