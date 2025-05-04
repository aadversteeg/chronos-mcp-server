using System;
using Ave.Extensions.Functional;
using Core.Application.Models;

namespace Core.Application.Services
{
    /// <summary>
    /// Interface for time-related operations used by the chronos tools.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// Gets the default timezone information.
        /// </summary>
        /// <returns>A Result containing either the default TimeZoneInfo on success or an Error on failure</returns>
        Result<TimeZoneInfo, Error> GetDefaultTimeZone();
        
        /// <summary>
        /// Gets the current date and time in the specified timezone or the default timezone if not specified.
        /// </summary>
        /// <param name="timezoneId">The ID of the timezone to get the current time for, or null to use default</param>
        /// <returns>A Result containing either a DateTimeWithTimeZoneId record on success or an Error on failure</returns>
        Result<DateTimeWithTimeZoneId, Error> GetCurrentTimeWithTimezone(TimeZoneId? timezoneId);
    }
}