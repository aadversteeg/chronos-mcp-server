using System.Collections.Generic;

namespace Core.Application.Models
{
    /// <summary>
    /// Application layer protocol error factories (validation failures).
    /// </summary>
    public static class ProtocolErrors
    {
        public static readonly ProtocolError NoDefaultTimeZoneId =
            new("DefaultTimeZoneId is not set.", ProtocolErrorCodes.NoDefaultTimeZoneId);

        public static readonly ProtocolError NullValueForTimeZoneIdValue =
            new("TimeZoneId value cannot be null or empty.", ProtocolErrorCodes.NullValueForTimeZoneIdValue);

        public static ProtocolError InvalidTimeZoneIdValue(string value) =>
            new($"Invalid TimeZoneId value: {value}.",
                ProtocolErrorCodes.InvalidTimeZoneIdValue,
                new Dictionary<string, object> { ["providedValue"] = value });
    }
}
