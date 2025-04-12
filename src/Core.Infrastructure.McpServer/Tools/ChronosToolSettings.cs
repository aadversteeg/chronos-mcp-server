namespace Core.Infrastructure.McpServer.Tools
{
    public class ChronosToolSettings
    {
        public TimeZoneInfo DefaultTimezoneInfo { get; set; } = TimeZoneInfo.Utc;
        public Func<DateTime> CurrentDateTimeProvider { get; set; } = () => DateTime.UtcNow;
    }
}
