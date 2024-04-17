namespace Travaloud.Application.Common.Utils;

public class DateTimeUtils
{
    public static DateTime CalculateEndDate(DateTime startDate, decimal? dayDuration, decimal? nightDuration, decimal? hourDuration)
    {
        double totalDurationInDays = 0;

        if (dayDuration.HasValue)
        {
            totalDurationInDays += (double) dayDuration;
        }
        else if (nightDuration.HasValue)
        {
            totalDurationInDays += (double) nightDuration / 2.0;
        }

        var wholeDays = (int) Math.Floor(totalDurationInDays);
        var fractionalDayHours = (totalDurationInDays - wholeDays) * 24;

        startDate = startDate.AddDays(wholeDays).AddHours(fractionalDayHours);

        if (hourDuration.HasValue)
        {
            startDate = startDate.AddHours((double) hourDuration);
        }

        return startDate;
    }

}