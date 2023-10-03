using Tcc2.Domain.Entities;

namespace Tcc2.Application.Models.Outputs;

public class PersonSimpleOutput : IEntity
{
    public long? Id { get; set; }
    public string Name { get; set; }
}
