using Ave.Extensions.ErrorPaths;

namespace Core.Application.Models
{
    /// <summary>
    /// Chronos-specific error codes extending well-known error code hierarchies.
    /// </summary>
    public static class ChronosErrorCodes
    {
        /// <summary>
        /// Validation error codes for input validation failures.
        /// These errors result in McpException (invisible to LLM).
        /// </summary>
        public static class Validation
        {
            /// <summary>
            /// TimeZoneId value is null or empty (required field).
            /// </summary>
            public static readonly ErrorCode RequiredTimeZoneId = ErrorCodes.Validation.Required / "TimeZoneId";

            /// <summary>
            /// TimeZoneId value is not a recognized timezone identifier.
            /// </summary>
            public static readonly ErrorCode InvalidTimeZoneId = ErrorCodes.Validation.Invalid / "TimeZoneId";
        }

        /// <summary>
        /// Configuration error codes for missing or invalid configuration.
        /// These errors result in McpException (invisible to LLM).
        /// </summary>
        public static class Configuration
        {
            /// <summary>
            /// DefaultTimeZoneId configuration is not set.
            /// </summary>
            public static readonly ErrorCode DefaultTimeZoneId = ErrorCodes.Internal.Configuration / "DefaultTimeZoneId";
        }

        /// <summary>
        /// Operational error codes for runtime failures.
        /// These errors result in CallToolResult.IsError (visible to LLM).
        /// </summary>
        public static class Operational
        {
            /// <summary>
            /// A timezone was not found on the system.
            /// </summary>
            public static readonly ErrorCode TimeZoneNotFound = ErrorCodes.NotFound.Resource / "TimeZone";

            /// <summary>
            /// A timezone has corrupted or invalid data.
            /// </summary>
            public static readonly ErrorCode InvalidTimeZoneData = ErrorCodes.Internal.Unexpected / "TimeZoneData";

            /// <summary>
            /// Time conversion for a timezone failed.
            /// </summary>
            public static readonly ErrorCode TimeZoneConversionFailed = ErrorCodes.Internal.Unexpected / "TimeZoneConversion";
        }
    }
}
