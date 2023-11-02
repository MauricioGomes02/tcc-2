namespace Tcc2.Domain.Entities;

public class ActivityDay
{
    public ActivityDay()
    {
        
    }

    public ActivityDay(Activity activity, short dayId, long? activityId = null)
    {
        Activity = activity;
        ActivityId = activityId;
        DayId = dayId;
    }

    public Activity? Activity { get; private set; }
    public Day? Day { get; private set; }
    public long? ActivityId { get; private set; }
    public short DayId { get; private set; }
}
