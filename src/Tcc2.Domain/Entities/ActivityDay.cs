namespace Tcc2.Domain.Entities;

public class ActivityDay
{
    public ActivityDay()
    {
        
    }

    public ActivityDay(Activity activity, Day day, long? activityId = null, short? dayId = null)
    {
        Activity = activity;
        Day = day;
        ActivityId = activityId;
        DayId = dayId;
    }

    public ActivityDay(short dayId)
    {
        Day = new Day(dayId);
        DayId = dayId;
    }

    public Activity? Activity { get; private set; }
    public Day Day { get; private set; }
    public long? ActivityId { get; private set; }
    public short? DayId { get; private set; }

    internal void UpdateDay(Day day)
    {
        Day = day;
    }
}
