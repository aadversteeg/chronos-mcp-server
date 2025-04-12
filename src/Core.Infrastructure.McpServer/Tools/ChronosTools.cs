using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
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
        private readonly TimeZoneInfo _defaultTimezoneInfo;
        private readonly Func<DateTime> _currentDateTimeProvider;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public ChronosTools(ILogger<ChronosTools> logger, ChronosToolSettings toolsSettings)
        {
            _logger = logger;

            if(toolsSettings.DefaultTimezoneInfo == null)
            {
                throw new ArgumentNullException(nameof(ChronosToolSettings.DefaultTimezoneInfo), "Default timezone info cannot be null.");
            }
            _defaultTimezoneInfo = toolsSettings.DefaultTimezoneInfo;
            _logger.LogInformation("ChronosTools initialized with timezone: {Timezone}", _defaultTimezoneInfo.Id);

            if (toolsSettings.CurrentDateTimeProvider == null)
            {
                throw new ArgumentNullException(nameof(ChronosToolSettings.CurrentDateTimeProvider), "Current date time provider cannot be null.");
            }
            _currentDateTimeProvider = toolsSettings.CurrentDateTimeProvider;
        }


        [McpServerTool(Name = "get_current_date_and_time"), Description("Gets the current date and time in the specified timezone or the default timezone.")]
        public string GetCurrentDateAndTime(
            [Description("Optional: the timezone identifier (e.g., 'America/New_York', 'Eastern Standard Time'). If not specified, the default timezone id will be used.")] 
            string? timezoneId = null)
        {
            try
            {
                TimeZoneInfo timezoneInfo = _defaultTimezoneInfo;
                if (timezoneId != null)
                {
                    timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
                }
                _logger.LogInformation("Getting current time for timezone: {Timezone}", timezoneInfo.Id);

                // Get the current date and time
                var currentDateTime = _currentDateTimeProvider();

                // Get the current time in the specified timezone
                var currentDateTimeInTimezone = TimeZoneInfo.ConvertTime(currentDateTime, timezoneInfo);

                // Create structured response
                var response = new
                {
                    Date = currentDateTimeInTimezone.ToString("yyyy-MM-dd"),
                    Time = currentDateTimeInTimezone.ToString("HH:mm:ss"),
                    Timezone = timezoneInfo.Id,
                };

                return JsonSerializer.Serialize(response, JsonOptions);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Error = ex.Message }, JsonOptions);
            }
        }


        [McpServerTool(Name = "get_default_timezone_id"), Description("Gets the default timezone identifier. Used to determine the current time when no timezone id is specified.")]
        public string GetDefaultTimeZoneId()
        {
            try
            {
                _logger.LogInformation("Getting default time zone information");
                return _defaultTimezoneInfo.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default time zone information");
                return JsonSerializer.Serialize(new { Error = ex.Message }, JsonOptions);
            }
        }
    }
}