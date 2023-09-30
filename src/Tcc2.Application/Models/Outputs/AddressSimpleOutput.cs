namespace Tcc2.Application.Models.Outputs;

public class AddressSimpleOutput : IIdentity
{
    public long Id { get; set; }
    public string City { get; set; }
    public string State { get; set; }
}
