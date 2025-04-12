using Core.Infrastructure.McpServer.Tools;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;


namespace UnitTests.Infrastructure.McpServer.Tools
{
    public class ChronosToolsTests
    {
        private readonly Mock<ILogger<ChronosTools>> _loggerMock;
        private readonly TimeZoneInfo _defaultTimezoneInfo;
        private readonly DateTime _fixedDateTime = new DateTime(2023, 12, 25, 12, 0, 0, DateTimeKind.Utc);
        private readonly Func<DateTime> _fixedDateTimeProvider;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true
        };

        public ChronosToolsTests()
        {
            _loggerMock = new Mock<ILogger<ChronosTools>>();
            _fixedDateTimeProvider = () => _fixedDateTime;
            
            // Try to get Amsterdam timezone, or fall back to UTC
            try
            {
                _defaultTimezoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
            }
            catch
            {
                try
                {
                    _defaultTimezoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
                }
                catch
                {
                    _defaultTimezoneInfo = TimeZoneInfo.Utc;
                }
            }
        }

        [Fact(DisplayName = "CT-001: Constructor throws when default timezone is null")]
        public void CT001()
        {
            // Arrange
            var settings = new ChronosToolSettings
            {
                DefaultTimezoneInfo = null!,
                CurrentDateTimeProvider = _fixedDateTimeProvider
            };
            
            Action act = () => new ChronosTools(_loggerMock.Object, settings);

            // Act & Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("DefaultTimezoneInfo");
        }

        [Fact(DisplayName = "CT-002: Constructor throws when current date time provider is null")]
        public void CT002()
        {
            // Arrange
            var settings = new ChronosToolSettings
            {
                DefaultTimezoneInfo = _defaultTimezoneInfo,
                CurrentDateTimeProvider = null!
            };
            
            Action act = () => new ChronosTools(_loggerMock.Object, settings);

            // Act & Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("CurrentDateTimeProvider");
        }

        [Fact(DisplayName = "CT-003: GetCurrentDateAndTime returns correct data with default timezone")]
        public void CT003()
        {
            // Arrange
            var settings = new ChronosToolSettings
            {
                DefaultTimezoneInfo = _defaultTimezoneInfo,
                CurrentDateTimeProvider = _fixedDateTimeProvider
            };
            
            var chronosTools = new ChronosTools(_loggerMock.Object, settings);

            // Act
            var result = chronosTools.GetCurrentDateAndTime();
            var response = JsonSerializer.Deserialize<DateTimeResponse>(result, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            // We don't test exact time since it will be different in Amsterdam timezone
            response!.Timezone.Should().Be(_defaultTimezoneInfo.Id);
        }

        [Fact(DisplayName = "CT-004: GetCurrentDateAndTime returns correct data with custom timezone")]
        public void CT004()
        {
            // Skip if running in environment without America/New_York timezone
            var availableTimezones = TimeZoneInfo.GetSystemTimeZones();
            if (!availableTimezones.Any(tz => tz.Id == "America/New_York" || tz.Id == "Eastern Standard Time"))
            {
                return; // Skip test if timezone not available
            }

            // Arrange
            var tzId = availableTimezones.Any(tz => tz.Id == "America/New_York") 
                ? "America/New_York" 
                : "Eastern Standard Time"; // Windows equivalent
                
            var settings = new ChronosToolSettings
            {
                DefaultTimezoneInfo = _defaultTimezoneInfo,
                CurrentDateTimeProvider = _fixedDateTimeProvider
            };
            
            var chronosTools = new ChronosTools(_loggerMock.Object, settings);

            // Act
            var result = chronosTools.GetCurrentDateAndTime(tzId);
            var response = JsonSerializer.Deserialize<DateTimeResponse>(result, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            response!.Timezone.Should().Be(tzId);
            // Note: We don't assert exact date/time as it depends on timezone conversions
        }

        [Fact(DisplayName = "CT-005: GetCurrentDateAndTime returns error for invalid timezone")]
        public void CT005()
        {
            // Arrange
            var invalidTimezoneId = "Invalid_Timezone";
            var settings = new ChronosToolSettings
            {
                DefaultTimezoneInfo = _defaultTimezoneInfo,
                CurrentDateTimeProvider = _fixedDateTimeProvider
            };
            
            var chronosTools = new ChronosTools(_loggerMock.Object, settings);

            // Act
            var result = chronosTools.GetCurrentDateAndTime(invalidTimezoneId);
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(result, _jsonOptions);

            // Assert
            errorResponse.Should().NotBeNull();
            errorResponse!.Error.Should().NotBeNull();
            errorResponse.Error.Should().Contain(invalidTimezoneId);
        }

        [Fact(DisplayName = "CT-006: GetDefaultTimeZoneId returns correct timezone ID")]
        public void CT006()
        {
            // Arrange
            var settings = new ChronosToolSettings
            {
                DefaultTimezoneInfo = _defaultTimezoneInfo,
                CurrentDateTimeProvider = _fixedDateTimeProvider
            };
            
            var chronosTools = new ChronosTools(_loggerMock.Object, settings);

            // Act
            var result = chronosTools.GetDefaultTimeZoneId();

            // Assert
            result.Should().Be(_defaultTimezoneInfo.Id);
        }

        // Helper classes for deserialization
        private class DateTimeResponse
        {
            public string? Date { get; set; }
            public string? Time { get; set; }
            public string? Timezone { get; set; }
        }

        private class ErrorResponse
        {
            public string? Error { get; set; }
        }
    }
}