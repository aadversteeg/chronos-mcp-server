namespace Core.Application.Models
{
    public class Errors
    {
        public static readonly Error NoDefaultTimeZoneId = 
            new ("DefaultTimeZoneId is not set.","NoDefaultTimeZoneId");

        public static readonly Error NullValueForTimeZoneIdValue =
            new("TimeZoneId value cannot be null or empty.", "NullValueForTimeZoneIdValue");

        public static Error InvalidTimeZoneIdValue(string value) =>
            new($"Invalid TimeZoneId value: {value}.", "InvalidTimeZoneIdValue");
    }
}
