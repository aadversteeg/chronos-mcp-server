using System;
using System.Linq;
using Core.Application.Models;
using Core.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTests.Application.Services
{
    public class TimeServiceTests
    {
        private readonly Mock<ILogger<TimeService>> _loggerMock;
        private readonly TimeZoneId _defaultTimeZoneId;
        private readonly DateTime _fixedDateTime = new DateTime(2023, 12, 25, 12, 0, 0, DateTimeKind.Utc);
        private readonly Func<DateTime> _fixedDateTimeProvider;

        public TimeServiceTests()
        {
            _loggerMock = new Mock<ILogger<TimeService>>();
            _fixedDateTimeProvider = () => _fixedDateTime;
            
            // Try to get Amsterdam timezone, or fall back to UTC
            string tzId;
            try
            {
                tzId = "Europe/Amsterdam";
                TimeZoneInfo.FindSystemTimeZoneById(tzId); // Check if timezone exists
            }
            catch
            {
                try
                {
                    tzId = "W. Europe Standard Time";
                    TimeZoneInfo.FindSystemTimeZoneById(tzId); // Check if timezone exists
                }
                catch
                {
                    tzId = "UTC";
                }
            }
            
            var defaultTimeZoneIdResult = TimeZoneId.Create(tzId);
            defaultTimeZoneIdResult.IsSuccess.Should().BeTrue();
            _defaultTimeZoneId = defaultTimeZoneIdResult.Value;
        }

        [Fact(DisplayName = "TS-001: Constructor throws when logger is null")]
        public void TS001()
        {
            // Arrange
            ILogger<TimeService> logger = null!;
            
            // Act
            Action act = () => new TimeService(logger, _defaultTimeZoneId, _fixedDateTimeProvider);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }

        [Fact(DisplayName = "TS-003: Constructor throws when current date time provider is null")]
        public void TS003()
        {
            // Arrange
            Func<DateTime> currentDateTimeProvider = null!;
            
            // Act
            Action act = () => new TimeService(_loggerMock.Object, _defaultTimeZoneId, currentDateTimeProvider);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("currentDateTimeProvider");
        }

        [Fact(DisplayName = "TS-005: GetDefaultTimeZone returns the default timezone")]
        public void TS005()
        {
            // Arrange
            var timeService = new TimeService(
                _loggerMock.Object,
                _defaultTimeZoneId, 
                _fixedDateTimeProvider);

            // Act
            var result = timeService.GetDefaultTimeZone();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(_defaultTimeZoneId.Value);
        }
        
        [Fact(DisplayName = "TS-006: GetDefaultTimeZone returns error when default timezone is null")]
        public void TS006()
        {
            // Arrange
            TimeZoneId? nullTimeZoneId = null;
            var timeService = new TimeService(
                _loggerMock.Object,
                nullTimeZoneId, 
                _fixedDateTimeProvider);

            // Act
            var result = timeService.GetDefaultTimeZone();

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("NoDefaultTimeZoneId");
            result.Error.Message.Should().Be("DefaultTimeZoneId is not set.");
        }
        
        [Fact(DisplayName = "TS-009: GetCurrentTimeWithTimezone returns correct time and timezone with default timezone")]
        public void TS009()
        {
            // Arrange
            var timeService = new TimeService(
                _loggerMock.Object,
                _defaultTimeZoneId, 
                _fixedDateTimeProvider);

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_defaultTimeZoneId.Value);
            var expectedDateTime = TimeZoneInfo.ConvertTime(_fixedDateTime, timeZoneInfo);

            // Act
            var result = timeService.GetCurrentTimeWithTimezone(null);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.CurrentDateTime.Should().Be(expectedDateTime);
            result.Value.UsedTimezoneId.Value.Should().Be(_defaultTimeZoneId.Value);
        }
        
        [Fact(DisplayName = "TS-010: GetCurrentTimeWithTimezone returns correct time and timezone with specified timezone")]
        public void TS010()
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
                
            var timeService = new TimeService(
                _loggerMock.Object,
                _defaultTimeZoneId, 
                _fixedDateTimeProvider);

            var expectedTimezone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            var expectedDateTime = TimeZoneInfo.ConvertTime(_fixedDateTime, expectedTimezone);
            
            var timezoneIdResult = TimeZoneId.Create(tzId);
            timezoneIdResult.IsSuccess.Should().BeTrue();

            // Act
            var result = timeService.GetCurrentTimeWithTimezone(timezoneIdResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.CurrentDateTime.Should().Be(expectedDateTime);
            result.Value.UsedTimezoneId.Value.Should().Be(tzId);
        }
        
        [Fact(DisplayName = "TS-011: GetCurrentTimeWithTimezone returns error when no timezone provided and default is null")]
        public void TS011()
        {
            // Arrange
            TimeZoneId? nullTimeZoneId = null;
            var timeService = new TimeService(
                _loggerMock.Object,
                nullTimeZoneId, 
                _fixedDateTimeProvider);

            // Act
            var result = timeService.GetCurrentTimeWithTimezone(null);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("NoDefaultTimeZoneId");
            result.Error.Message.Should().Be("DefaultTimeZoneId is not set.");
        }
    }
}