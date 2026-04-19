namespace DellinDictionary.Helpers;

public static class ScheduleHelper
{
    private static readonly TimeZoneInfo MoscowTimeZone =
        TimeZoneInfo.TryFindSystemTimeZoneById("Europe/Moscow", out var tz)
            ? tz
            : TimeZoneInfo.CreateCustomTimeZone("MSK", TimeSpan.FromHours(3), "MSK", "MSK");

    public static TimeSpan GetDelayUntilNextRun(TimeSpan runTimeMsk)
    {
        var nowUtc = DateTime.UtcNow;
        var nowMsk = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, MoscowTimeZone);

        var todayRun = nowMsk.Date.Add(runTimeMsk);
        var nextRunMsk = nowMsk < todayRun ? todayRun : todayRun.AddDays(1);

        var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(nextRunMsk, DateTimeKind.Unspecified),
            MoscowTimeZone);

        var delay = nextRunUtc - nowUtc;
        return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
    }
}
