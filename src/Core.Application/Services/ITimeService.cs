using System;
using Core.Application.Models;

namespace Core.Application.Services
{
    /// <summary>
    /// Interface for time-related operations used by the chronos tools.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// Gets the current date and time.
        /// </summary>
        /// <returns>The current date and time</returns>
        DateTime GetCurrentTime();
        
        /// <summary>
        /// Gets the default timezone information.
        /// </summary>
        /// <returns>The default timezone information</returns>
        TimeZoneInfo GetDefaultTimeZone();
        
        /// <summary>
        /// Gets the current date and time in the specified timezone.
        /// </summary>
        /// <param name="timezoneId">The ID of the timezone to get the current time for</param>
        /// <returns>The current date and time in the specified timezone</returns>
        /// <exception cref="TimeZoneNotFoundException">Thrown when the timezone ID is not found</exception>
        DateTime GetCurrentTimeInTimeZone(string timezoneId);
        
        /// <summary>
        /// Gets the current date and time in the specified timezone or the default timezone if not specified.
        /// </summary>
        /// <param name="timezoneId">The ID of the timezone to get the current time for, or null/empty to use default</param>
        /// <returns>A record containing the current date and time in the timezone and the timezone ID used</returns>
        /// <exception cref="TimeZoneNotFoundException">Thrown when the specified timezone ID is not found</exception>
        DateTimeWithTimeZoneId GetCurrentTimeWithTimezone(string? timezoneId);
    }
}