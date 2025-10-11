namespace Core.Application.Models
{
    /// <summary>
    /// Protocol error codes for validation failures.
    /// </summary>
    public static class ProtocolErrorCodes
    {
        // TimeZoneId validation errors
        public const string NoDefaultTimeZoneId = "NoDefaultTimeZoneId";
        public const string NullValueForTimeZoneIdValue = "NullValueForTimeZoneIdValue";
        public const string InvalidTimeZoneIdValue = "InvalidTimeZoneIdValue";
    }
}
