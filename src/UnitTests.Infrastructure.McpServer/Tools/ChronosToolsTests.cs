using Core.Application.Services;
using Core.Infrastructure.McpServer.Tools;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Text.Json;
using Xunit;


namespace UnitTests.Infrastructure.McpServer.Tools
{
    public class ChronosToolsTests
    {
        private readonly Mock<ILogger<ChronosTools>> _loggerMock;
        private readonly Mock<ITimeService> _timeServiceMock;
        private readonly TimeZoneInfo _defaultTimezoneInfo;
        private readonly DateTime _fixedDateTime = new DateTime(2023, 12, 25, 12, 0, 0, DateTimeKind.Utc);
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true
        };

        public ChronosToolsTests()
        {
            _loggerMock = new Mock<ILogger<ChronosTools>>();
            _timeServiceMock = new Mock<ITimeService>();
            
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
            
            // Setup default behavior for ITimeService
            _timeServiceMock.Setup(s => s.GetCurrentTime()).Returns(_fixedDateTime);
            _timeServiceMock.Setup(s => s.GetDefaultTimeZone()).Returns(_defaultTimezoneInfo);
            _timeServiceMock.Setup(s => s.GetCurrentTimeInTimeZone(It.IsAny<string>()))
                .Returns((string tz) => TimeZoneInfo.ConvertTime(_fixedDateTime, TimeZoneInfo.FindSystemTimeZoneById(tz)));
            
            // Setup the new GetCurrentTimeWithTimezone method
            _timeServiceMock.Setup(s => s.GetCurrentTimeWithTimezone(It.IsAny<string>()))
                .Returns((string tz) => 
                {
                    if (string.IsNullOrEmpty(tz))
                    {
                        return (TimeZoneInfo.ConvertTime(_fixedDateTime, _defaultTimezoneInfo), _defaultTimezoneInfo.Id);
                    }
                    else
                    {
                        var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tz);
                        return (TimeZoneInfo.ConvertTime(_fixedDateTime, targetTimeZone), tz);
                    }
                });
        }

        [Fact(DisplayName = "CT-001: Constructor throws when logger is null")]
        public void CT001()
        {
            // Arrange
            ILogger<ChronosTools> logger = null!;
            
            // Act & Assert
            Action act = () => new ChronosTools(logger, _timeServiceMock.Object);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }

        [Fact(DisplayName = "CT-002: Constructor throws when time service is null")]
        public void CT002()
        {
            // Arrange
            ITimeService timeService = null!;
            
            // Act & Assert
            Action act = () => new ChronosTools(_loggerMock.Object, timeService);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("timeService");
        }

        [Fact(DisplayName = "CT-003: GetCurrentDateAndTime returns correct data with default timezone")]
        public void CT003()
        {
            // Arrange
            var chronosTools = new ChronosTools(_loggerMock.Object, _timeServiceMock.Object);
            
            // Setup specific expectations for this test
            var expectedDateTime = TimeZoneInfo.ConvertTime(_fixedDateTime, _defaultTimezoneInfo);
            
            // Act
            var result = chronosTools.GetCurrentDateAndTime();
            var response = JsonSerializer.Deserialize<DateTimeResponse>(result, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            response!.Timezone.Should().Be(_defaultTimezoneInfo.Id);
            
            // Use the new GetCurrentTimeWithTimezone method
            _timeServiceMock.Verify(s => s.GetCurrentTimeWithTimezone(It.IsAny<string>()), Times.Once);
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
                
            var chronosTools = new ChronosTools(_loggerMock.Object, _timeServiceMock.Object);

            // Act
            var result = chronosTools.GetCurrentDateAndTime(tzId);
            var response = JsonSerializer.Deserialize<DateTimeResponse>(result, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            response!.Timezone.Should().Be(tzId);
            
            // Use the new GetCurrentTimeWithTimezone method
            _timeServiceMock.Verify(s => s.GetCurrentTimeWithTimezone(tzId), Times.Once);
        }

        [Fact(DisplayName = "CT-005: GetCurrentDateAndTime returns error for invalid timezone")]
        public void CT005()
        {
            // Arrange
            var invalidTimezoneId = "Invalid_Timezone";
            
            _timeServiceMock.Setup(s => s.GetCurrentTimeWithTimezone(invalidTimezoneId))
                .Throws(new TimeZoneNotFoundException($"The time zone ID '{invalidTimezoneId}' was not found on the local computer."));
                
            var chronosTools = new ChronosTools(_loggerMock.Object, _timeServiceMock.Object);

            // Act
            var result = chronosTools.GetCurrentDateAndTime(invalidTimezoneId);
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(result, _jsonOptions);

            // Assert
            errorResponse.Should().NotBeNull();
            errorResponse!.Error.Should().NotBeNull();
            errorResponse.Error.Should().Contain(invalidTimezoneId);
            _timeServiceMock.Verify(s => s.GetCurrentTimeWithTimezone(invalidTimezoneId), Times.Once);
        }

        [Fact(DisplayName = "CT-006: GetDefaultTimeZoneId returns correct timezone ID")]
        public void CT006()
        {
            // Arrange
            var chronosTools = new ChronosTools(_loggerMock.Object, _timeServiceMock.Object);

            // Act
            var result = chronosTools.GetDefaultTimeZoneId();

            // Assert
            result.Should().Be(_defaultTimezoneInfo.Id);
            _timeServiceMock.Verify(s => s.GetDefaultTimeZone(), Times.AtLeastOnce);
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