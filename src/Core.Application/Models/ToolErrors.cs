using System.Collections.Generic;

namespace Core.Application.Models
{
    /// <summary>
    /// Application layer tool error factories (operational failures).
    /// </summary>
    public static class ToolErrors
    {
        public static ToolError TimeZoneNotFound(string timezoneId) =>
            new($"Timezone '{timezoneId}' was not found on this system.",
                ToolErrorCodes.TimeZoneNotFound,
                new Dictionary<string, object> { ["timezoneId"] = timezoneId });

        public static ToolError TimeZoneConversionFailed(string timezoneId, string message) =>
            new($"Failed to convert time for timezone '{timezoneId}': {message}",
                ToolErrorCodes.TimeZoneConversionFailed,
                new Dictionary<string, object>
                {
                    ["timezoneId"] = timezoneId,
                    ["errorMessage"] = message
                });
    }
}
