using Ave.Extensions.Functional;
using Core.Application.Models;
using Core.Application.Services;
using Core.Application.Extensions;
using Core.Infrastructure.McpServer.Extensions;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Core.Infrastructure.McpServer.Tools
{
    /// <summary>
    /// Provides current date and time for different timezones.
    /// </summary>
    [McpServerToolType]
    public class ChronosTools
    {
        private readonly ILogger<ChronosTools> _logger;
        private readonly ITimeService _timeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChronosTools"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging events.</param>
        /// <param name="timeService">The time service for retrieving date and time information.</param>
        /// <exception cref="ArgumentNullException">Thrown if logger or timeService is null.</exception>
        public ChronosTools(ILogger<ChronosTools> logger, ITimeService timeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            
            var defaultTimezoneResult = _timeService.GetDefaultTimeZone();
            if (defaultTimezoneResult.IsSuccess)
            {
                _logger.LogInformation("ChronosTools initialized with timezone: {Timezone}", 
                    defaultTimezoneResult.Value.Id);
            }
            else
            {
                _logger.LogWarning("ChronosTools initialized but could not get default timezone: {ErrorMessage}",
                    defaultTimezoneResult.Error.Message);
            }
        }

        /// <summary>
        /// Gets the current date and time in the specified timezone or the default timezone.
        /// </summary>
        /// <param name="timezoneId">Optional timezone identifier (e.g., 'America/New_York'). If not specified, the default timezone id will be used.</param>
        /// <returns>JSON string containing date, time, and timezone information.</returns>
        /// <exception cref="McpException">Thrown when an error occurs during processing.</exception>
        [McpServerTool(Name = "get_current_date_and_time"), Description("Gets the current date and time in the specified timezone or the default timezone.")]
        public string GetCurrentDateAndTime(
            [Description("Optional: the timezone identifier (e.g., 'America/New_York', 'Eastern Standard Time'). If not specified, the default timezone id will be used.")] 
            string? timezoneId = null)
        {
            try
            {
                // Convert the string to TimeZoneId, handle null, and get current time
                return timezoneId
                    .Bind(TimeZoneId.Create) // Convert string to Result<TimeZoneId, Error>
                    .OnSuccessBind(_timeService.GetCurrentTimeWithTimezone) // Get current time with timezone
                    .ToToolResult(result => new { 
                        Date = result.CurrentDateTime.ToString("yyyy-MM-dd"),
                        Time = result.CurrentDateTime.ToString("HH:mm:ss"),
                        Timezone = result.UsedTimezoneId.Value
                    });
            }
            catch (McpException)
            {
                throw; // Re-throw McpException without wrapping
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current date and time");
                throw new McpException("Error getting current date and time", ex);
            }
        }

        /// <summary>
        /// Gets the default timezone identifier used when no timezone is specified.
        /// </summary>
        /// <returns>JSON string containing the default timezone identifier.</returns>
        /// <exception cref="McpException">Thrown when an error occurs during processing.</exception>
        [McpServerTool(Name = "get_default_timezone_id"), Description("Gets the default timezone identifier. Used to determine the current time when no timezone id is specified.")]
        public string GetDefaultTimeZoneId()
        {
            try
            {
                _logger.LogInformation("Getting default time zone information");

                // Get the default timezone and return its ID
                return _timeService.GetDefaultTimeZone()
                    .ToToolResult(timeZone => timeZone.Id);
            }
            catch (McpException)
            {
                throw; // Re-throw McpException without wrapping
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default time zone information");
                throw new McpException("Error getting default time zone information", ex);
            }
        }
    }
}