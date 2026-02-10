using Ave.Extensions.ErrorPaths;

namespace Core.Application.Models
{
    /// <summary>
    /// Factory class for creating Chronos-specific errors with metadata.
    /// </summary>
    public static class ChronosErrors
    {
        /// <summary>
        /// Error indicating the DefaultTimeZoneId is not configured.
        /// </summary>
        public static readonly Error NoDefaultTimeZoneId =
            new Error(ChronosErrorCodes.Configuration.DefaultTimeZoneId, "DefaultTimeZoneId is not set.");

        /// <summary>
        /// Error indicating a null or empty TimeZoneId value.
        /// </summary>
        public static readonly Error NullValueForTimeZoneIdValue =
            new Error(ChronosErrorCodes.Validation.RequiredTimeZoneId, "TimeZoneId value cannot be null or empty.");

        /// <summary>
        /// Creates an error for an invalid TimeZoneId value.
        /// </summary>
        /// <param name="value">The invalid timezone ID that was provided.</param>
        /// <returns>An Error with metadata containing the provided value.</returns>
        public static Error InvalidTimeZoneIdValue(string value) =>
            new Error(ChronosErrorCodes.Validation.InvalidTimeZoneId, $"Invalid TimeZoneId value: {value}.")
                .With("providedValue", value);

        /// <summary>
        /// Creates an error indicating a timezone was not found on the system.
        /// </summary>
        /// <param name="timezoneId">The timezone ID that was not found.</param>
        /// <returns>An Error with metadata containing the timezone ID.</returns>
        public static Error TimeZoneNotFound(string timezoneId) =>
            new Error(ChronosErrorCodes.Operational.TimeZoneNotFound, $"Timezone '{timezoneId}' was not found on this system.")
                .With("timezoneId", timezoneId);

        /// <summary>
        /// Creates an error indicating corrupted or invalid timezone data.
        /// </summary>
        /// <param name="timezoneId">The timezone ID with invalid data.</param>
        /// <param name="message">The error message from the system.</param>
        /// <returns>An Error with metadata containing the timezone ID and error message.</returns>
        public static Error InvalidTimeZoneData(string timezoneId, string message) =>
            new Error(ChronosErrorCodes.Operational.InvalidTimeZoneData, $"Timezone '{timezoneId}' has corrupted or invalid data: {message}")
                .With("timezoneId", timezoneId)
                .With("errorMessage", message);

        /// <summary>
        /// Creates an error indicating a time conversion failure.
        /// </summary>
        /// <param name="timezoneId">The timezone ID that failed conversion.</param>
        /// <param name="message">The error message from the system.</param>
        /// <returns>An Error with metadata containing the timezone ID and error message.</returns>
        public static Error TimeZoneConversionFailed(string timezoneId, string message) =>
            new Error(ChronosErrorCodes.Operational.TimeZoneConversionFailed, $"Failed to convert time for timezone '{timezoneId}': {message}")
                .With("timezoneId", timezoneId)
                .With("errorMessage", message);
    }
}
