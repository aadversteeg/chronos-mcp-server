using System;
using Microsoft.Extensions.Logging;

namespace Core.Application.Services
{
    /// <summary>
    /// Implementation of ITimeService that provides time-related operations.
    /// </summary>
    public class TimeService : ITimeService
    {
        private readonly ILogger<TimeService> _logger;
        private readonly TimeZoneInfo _defaultTimeZone;
        private readonly Func<DateTime> _currentDateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="defaultTimeZone">The default timezone to use</param>
        /// <param name="currentDateTimeProvider">Provider function for the current date and time</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
        public TimeService(
            ILogger<TimeService> logger,
            TimeZoneInfo defaultTimeZone,
            Func<DateTime> currentDateTimeProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _defaultTimeZone = defaultTimeZone ?? throw new ArgumentNullException(nameof(defaultTimeZone));
            _currentDateTimeProvider = currentDateTimeProvider ?? throw new ArgumentNullException(nameof(currentDateTimeProvider));
            
            _logger.LogInformation("TimeService initialized with default timezone: {Timezone}", _defaultTimeZone.Id);
        }

        /// <inheritdoc />
        public DateTime GetCurrentTime()
        {
            return _currentDateTimeProvider();
        }

        /// <inheritdoc />
        public TimeZoneInfo GetDefaultTimeZone()
        {
            _logger.LogInformation("Getting default timezone: {Timezone}", _defaultTimeZone.Id);
            return _defaultTimeZone;
        }

        /// <inheritdoc />
        public DateTime GetCurrentTimeInTimeZone(string timezoneId)
        {
            try
            {
                var targetTimeZone = string.IsNullOrEmpty(timezoneId)
                    ? _defaultTimeZone
                    : TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

                _logger.LogInformation("Converting current time to timezone: {Timezone}", targetTimeZone.Id);
                var currentDateTime = _currentDateTimeProvider();
                return TimeZoneInfo.ConvertTime(currentDateTime, targetTimeZone);
            }
            catch (TimeZoneNotFoundException ex)
            {
                _logger.LogError(ex, "Timezone not found: {TimezoneId}", timezoneId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting time to timezone: {TimezoneId}", timezoneId);
                throw;
            }
        }
        
        /// <inheritdoc />
        public (DateTime CurrentDateTime, string UsedTimezoneId) GetCurrentTimeWithTimezone(string? timezoneId)
        {
            try
            {
                DateTime currentDateTime;
                string usedTimezoneId;
                
                if (string.IsNullOrEmpty(timezoneId))
                {
                    // Use default timezone
                    currentDateTime = TimeZoneInfo.ConvertTime(
                        _currentDateTimeProvider(),
                        _defaultTimeZone);
                    usedTimezoneId = _defaultTimeZone.Id;
                    _logger.LogInformation("Using default timezone: {Timezone}", usedTimezoneId);
                }
                else
                {
                    // Use specified timezone
                    var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
                    currentDateTime = TimeZoneInfo.ConvertTime(
                        _currentDateTimeProvider(),
                        targetTimeZone);
                    usedTimezoneId = timezoneId;
                    _logger.LogInformation("Using specified timezone: {Timezone}", usedTimezoneId);
                }

                return (currentDateTime, usedTimezoneId);
            }
            catch (TimeZoneNotFoundException ex)
            {
                _logger.LogError(ex, "Timezone not found: {TimezoneId}", timezoneId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting time with timezone: {TimezoneId}", timezoneId);
                throw;
            }
        }
    }
}