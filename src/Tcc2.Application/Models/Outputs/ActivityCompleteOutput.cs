namespace Tcc2.Application.Models.Outputs;
public class ActivityCompleteOutput : ActivitySimpleOutput
{
    public string Country { get; set; }
    public string Street { get; set; }
    public int Number { get; set; }
    public string PostalCode { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
}
