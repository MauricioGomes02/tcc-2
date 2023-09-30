namespace Tcc2.Application.Models.Outputs;

public class AddressCompleteOutput : IIdentity
{
    public long Id { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string Neighborhood { get; set; }
    public string Street { get; set; }
    public int Number { get; set; }
    public string? PostalCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
