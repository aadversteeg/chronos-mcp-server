namespace Core.Application.Models
{
    /// <summary>
    /// Tool error codes for operational failures.
    /// </summary>
    public static class ToolErrorCodes
    {
        // TimeZone operational errors
        public const string TimeZoneNotFound = "TimeZoneNotFound";
        public const string TimeZoneConversionFailed = "TimeZoneConversionFailed";
    }
}
