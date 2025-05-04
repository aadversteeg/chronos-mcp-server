using Core.Application.Services;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public ChronosTools(ILogger<ChronosTools> logger, ITimeService timeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            
            _logger.LogInformation("ChronosTools initialized with timezone: {Timezone}", 
                _timeService.GetDefaultTimeZone().Id);
        }

        [McpServerTool(Name = "get_current_date_and_time"), Description("Gets the current date and time in the specified timezone or the default timezone.")]
        public string GetCurrentDateAndTime(
            [Description("Optional: the timezone identifier (e.g., 'America/New_York', 'Eastern Standard Time'). If not specified, the default timezone id will be used.")] 
            string? timezoneId = null)
        {
            try
            {
                var (currentDateTimeInTimezone, timezoneIdToUse) = _timeService.GetCurrentTimeWithTimezone(timezoneId);
                
                _logger.LogInformation("Returning current time for timezone: {Timezone}", timezoneIdToUse);

                // Create structured response
                var response = new
                {
                    Date = currentDateTimeInTimezone.ToString("yyyy-MM-dd"),
                    Time = currentDateTimeInTimezone.ToString("HH:mm:ss"),
                    Timezone = timezoneIdToUse,
                };

                return JsonSerializer.Serialize(response, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current date and time");
                return JsonSerializer.Serialize(new { Error = ex.Message }, JsonOptions);
            }
        }

        [McpServerTool(Name = "get_default_timezone_id"), Description("Gets the default timezone identifier. Used to determine the current time when no timezone id is specified.")]
        public string GetDefaultTimeZoneId()
        {
            try
            {
                _logger.LogInformation("Getting default time zone information");
                return _timeService.GetDefaultTimeZone().Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default time zone information");
                return JsonSerializer.Serialize(new { Error = ex.Message }, JsonOptions);
            }
        }
    }
}