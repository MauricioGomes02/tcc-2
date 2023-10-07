namespace Tcc2.Application.Models.Inputs;

public class ActivityInput
{
    public AddressInput Address { get; set; }
    public ICollection<short> DaysOfWeek { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public string Description { get; set; }
}
