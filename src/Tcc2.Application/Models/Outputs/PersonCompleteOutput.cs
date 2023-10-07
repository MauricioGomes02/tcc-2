using Tcc2.Domain.Entities;

namespace Tcc2.Application.Models.Outputs;

public class PersonCompleteOutput : PersonSimpleOutput
{
    public AddressSimpleOutput Address { get; set; }
    public IEnumerable<ActivitySimpleOutput> Activities { get; set; }
}
