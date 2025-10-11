using System;
using Microsoft.Extensions.Logging;
using Core.Application.Models;
using Ave.Extensions.Functional;
using Core.Application.Extensions;

namespace Core.Application.Services
{
    /// <summary>
    /// Implementation of ITimeService that provides time-related operations.
    /// </summary>
    public class TimeService : ITimeService
    {
        private readonly ILogger<TimeService> _logger;
        private readonly TimeZoneId? _defaultTimeZoneId;
        private readonly Func<DateTime> _currentDateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="defaultTimeZoneId">The default timezone id to use</param>
        /// <param name="currentDateTimeProvider">Provider function for the current date and time</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
        public TimeService(
            ILogger<TimeService> logger,
            TimeZoneId? defaultTimeZoneId,
            Func<DateTime> currentDateTimeProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _defaultTimeZoneId = defaultTimeZoneId;
            _currentDateTimeProvider = currentDateTimeProvider ?? throw new ArgumentNullException(nameof(currentDateTimeProvider));
        }

        /// <inheritdoc />
        public Result<TimeZoneInfo, Error> GetDefaultTimeZone()
        {
            if (_defaultTimeZoneId == null)
            {
                return Result<TimeZoneInfo, Error>.Failure(ProtocolErrors.NoDefaultTimeZoneId);
            }

            return GetTimeZoneInfoById(_defaultTimeZoneId.Value);
        }

        /// <inheritdoc />
        public Result<DateTimeWithTimeZoneId, Error> GetCurrentTimeWithTimezone(TimeZoneId? maybeTimeZoneId)
        {
            return maybeTimeZoneId
                .Ensure(_defaultTimeZoneId, ProtocolErrors.NoDefaultTimeZoneId)
                .OnSuccessBind(timeZoneId =>
                    GetTimeZoneInfoById(timeZoneId.Value)
                        .OnSuccessBind(ConvertTimeToTimeZone));
        }

        /// <summary>
        /// Gets TimeZoneInfo by ID with proper error handling.
        /// </summary>
        private Result<TimeZoneInfo, Error> GetTimeZoneInfoById(string timezoneId)
        {
            try
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
                return Result<TimeZoneInfo, Error>.Success(timeZoneInfo);
            }
            catch (TimeZoneNotFoundException)
            {
                _logger.LogError("Timezone '{TimezoneId}' was not found", timezoneId);
                return Result<TimeZoneInfo, Error>.Failure(ToolErrors.TimeZoneNotFound(timezoneId));
            }
            catch (InvalidTimeZoneException ex)
            {
                _logger.LogError(ex, "Timezone '{TimezoneId}' has corrupted data", timezoneId);
                return Result<TimeZoneInfo, Error>.Failure(
                    ToolErrors.InvalidTimeZoneData(timezoneId, ex.Message));
            }
        }

        /// <summary>
        /// Converts current time to the specified timezone with proper error handling.
        /// </summary>
        private Result<DateTimeWithTimeZoneId, Error> ConvertTimeToTimeZone(TimeZoneInfo timeZone)
        {
            var currentDateTime = _currentDateTimeProvider();
            return ConvertDateTimeToTimeZone(currentDateTime, timeZone);
        }

        /// <summary>
        /// Converts a DateTime to the specified timezone with proper error handling.
        /// </summary>
        private Result<DateTimeWithTimeZoneId, Error> ConvertDateTimeToTimeZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            try
            {
                var convertedDateTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);

                // TimeZoneInfo.Id is guaranteed to be valid since we successfully loaded it
                var timeZoneId = new TimeZoneId(timeZone.Id);

                return Result<DateTimeWithTimeZoneId, Error>.Success(
                    new DateTimeWithTimeZoneId(convertedDateTime, timeZoneId));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Failed to convert time for timezone '{TimezoneId}'", timeZone.Id);
                return Result<DateTimeWithTimeZoneId, Error>.Failure(
                    ToolErrors.TimeZoneConversionFailed(timeZone.Id, ex.Message));
            }
        }
    }
}