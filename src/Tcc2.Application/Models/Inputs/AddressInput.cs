namespace Tcc2.Application.Models.Inputs;
public class AddressInput
{
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string Neighborhood { get; set; }
    public string Street { get; set; }
    public int Number { get; set; }
    public string? PostalCode { get; set; }
}
