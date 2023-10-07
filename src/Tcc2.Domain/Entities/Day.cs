namespace Tcc2.Domain.Entities;
public class Day
{
    public Day()
    {

    }

    public Day(short id, ICollection<ActivityDay>? activityDay = null)
    {
        Id = id;
        ActivityDay = activityDay;
    }

    public ICollection<ActivityDay>? ActivityDay { get; private set; }
    public short Id { get; private set; }
    public DayOfWeek? GetDayOfWeek()
    {
        return (DayOfWeek)Id;
    }
}
