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

            try
            {
                var timeZoneInfo = TimeZoneInfo
                    .FindSystemTimeZoneById(_defaultTimeZoneId.Value);

                return Result<TimeZoneInfo, Error>.Success(timeZoneInfo);
            }
            catch (TimeZoneNotFoundException)
            {
                _logger.LogError("Default timezone '{TimezoneId}' was not found", _defaultTimeZoneId.Value);
                return Result<TimeZoneInfo, Error>.Failure(ToolErrors.TimeZoneNotFound(_defaultTimeZoneId.Value));
            }
            catch (InvalidTimeZoneException ex)
            {
                _logger.LogError(ex, "Default timezone '{TimezoneId}' has corrupted data", _defaultTimeZoneId.Value);
                return Result<TimeZoneInfo, Error>.Failure(
                    ToolErrors.InvalidTimeZoneData(_defaultTimeZoneId.Value, ex.Message));
            }
        }

        /// <inheritdoc />
        public Result<DateTimeWithTimeZoneId, Error> GetCurrentTimeWithTimezone(TimeZoneId? maybeTimeZoneId)
        {
            return maybeTimeZoneId
                .Ensure(_defaultTimeZoneId, ProtocolErrors.NoDefaultTimeZoneId)
                .OnSuccessBind(timeZoneId =>
                {
                    try
                    {
                        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId.Value);
                        var currentDateTime = TimeZoneInfo.ConvertTime(
                            _currentDateTimeProvider(),
                            timeZone);

                        return Result<DateTimeWithTimeZoneId, Error>.Success(
                            new DateTimeWithTimeZoneId(currentDateTime, timeZoneId));
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        _logger.LogError("Timezone '{TimezoneId}' was not found", timeZoneId.Value);
                        return Result<DateTimeWithTimeZoneId, Error>.Failure(
                            ToolErrors.TimeZoneNotFound(timeZoneId.Value));
                    }
                    catch (InvalidTimeZoneException ex)
                    {
                        _logger.LogError(ex, "Timezone '{TimezoneId}' has corrupted data", timeZoneId.Value);
                        return Result<DateTimeWithTimeZoneId, Error>.Failure(
                            ToolErrors.InvalidTimeZoneData(timeZoneId.Value, ex.Message));
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.LogError(ex, "Failed to convert time for timezone '{TimezoneId}'", timeZoneId.Value);
                        return Result<DateTimeWithTimeZoneId, Error>.Failure(
                            ToolErrors.TimeZoneConversionFailed(timeZoneId.Value, ex.Message));
                    }
                });
        }
    }
}