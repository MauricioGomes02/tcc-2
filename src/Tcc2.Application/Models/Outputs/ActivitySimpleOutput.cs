using Tcc2.Domain.Entities;

namespace Tcc2.Application.Models.Outputs;

public class ActivitySimpleOutput : IEntity
{
    public long? Id { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string Description { get; set; }
    public ICollection<short> DaysOfWeek { get; set; }
}
