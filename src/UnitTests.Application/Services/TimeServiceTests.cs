using System;
using System.Linq;
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
        private readonly TimeZoneInfo _defaultTimezoneInfo;
        private readonly DateTime _fixedDateTime = new DateTime(2023, 12, 25, 12, 0, 0, DateTimeKind.Utc);
        private readonly Func<DateTime> _fixedDateTimeProvider;

        public TimeServiceTests()
        {
            _loggerMock = new Mock<ILogger<TimeService>>();
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

        [Fact(DisplayName = "TS-001: Constructor throws when logger is null")]
        public void TS001()
        {
            // Arrange
            ILogger<TimeService> logger = null!;
            
            // Act
            Action act = () => new TimeService(logger, _defaultTimezoneInfo, _fixedDateTimeProvider);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }

        [Fact(DisplayName = "TS-002: Constructor throws when default timezone is null")]
        public void TS002()
        {
            // Arrange
            TimeZoneInfo defaultTimeZone = null!;
            
            // Act
            Action act = () => new TimeService(_loggerMock.Object, defaultTimeZone, _fixedDateTimeProvider);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("defaultTimeZone");
        }

        [Fact(DisplayName = "TS-003: Constructor throws when current date time provider is null")]
        public void TS003()
        {
            // Arrange
            Func<DateTime> currentDateTimeProvider = null!;
            
            // Act
            Action act = () => new TimeService(_loggerMock.Object, _defaultTimezoneInfo, currentDateTimeProvider);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("currentDateTimeProvider");
        }

        [Fact(DisplayName = "TS-004: GetCurrentTime returns fixed date time from provider")]
        public void TS004()
        {
            // Arrange
            var timeService = new TimeService(
                _loggerMock.Object,
                _defaultTimezoneInfo,
                _fixedDateTimeProvider);

            // Act
            var result = timeService.GetCurrentTime();

            // Assert
            result.Should().Be(_fixedDateTime);
        }

        [Fact(DisplayName = "TS-005: GetDefaultTimeZone returns the default timezone")]
        public void TS005()
        {
            // Arrange
            var timeService = new TimeService(
                _loggerMock.Object,
                _defaultTimezoneInfo, 
                _fixedDateTimeProvider);

            // Act
            var result = timeService.GetDefaultTimeZone();

            // Assert
            result.Should().Be(_defaultTimezoneInfo);
        }

        [Fact(DisplayName = "TS-006: GetCurrentTimeInTimeZone converts to specified timezone")]
        public void TS006()
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
                _defaultTimezoneInfo, 
                _fixedDateTimeProvider);

            var expectedTimezone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            var expectedDateTime = TimeZoneInfo.ConvertTime(_fixedDateTime, expectedTimezone);

            // Act
            var result = timeService.GetCurrentTimeInTimeZone(tzId);

            // Assert
            result.Should().Be(expectedDateTime);
        }

        [Fact(DisplayName = "TS-007: GetCurrentTimeInTimeZone uses default timezone when null or empty")]
        public void TS007()
        {
            // Arrange
            var timeService = new TimeService(
                _loggerMock.Object,
                _defaultTimezoneInfo, 
                _fixedDateTimeProvider);

            var expectedDateTime = TimeZoneInfo.ConvertTime(_fixedDateTime, _defaultTimezoneInfo);

            // Act
            var resultWithNull = timeService.GetCurrentTimeInTimeZone(null!);
            var resultWithEmpty = timeService.GetCurrentTimeInTimeZone(string.Empty);

            // Assert
            resultWithNull.Should().Be(expectedDateTime);
            resultWithEmpty.Should().Be(expectedDateTime);
        }

        [Fact(DisplayName = "TS-008: GetCurrentTimeInTimeZone throws for invalid timezone")]
        public void TS008()
        {
            // Arrange
            var invalidTimezoneId = "Invalid_Timezone";
            var timeService = new TimeService(
                _loggerMock.Object,
                _defaultTimezoneInfo, 
                _fixedDateTimeProvider);

            // Act
            Action act = () => timeService.GetCurrentTimeInTimeZone(invalidTimezoneId);

            // Assert
            act.Should().Throw<TimeZoneNotFoundException>();
        }
        
        [Fact(DisplayName = "TS-009: GetCurrentTimeWithTimezone returns correct time and timezone with default timezone")]
        public void TS009()
        {
            // Arrange
            var timeService = new TimeService(
                _loggerMock.Object,
                _defaultTimezoneInfo, 
                _fixedDateTimeProvider);

            var expectedDateTime = TimeZoneInfo.ConvertTime(_fixedDateTime, _defaultTimezoneInfo);

            // Act
            var (resultDateTime, resultTimezoneId) = timeService.GetCurrentTimeWithTimezone(null);
            var (resultDateTime2, resultTimezoneId2) = timeService.GetCurrentTimeWithTimezone(string.Empty);

            // Assert
            resultDateTime.Should().Be(expectedDateTime);
            resultTimezoneId.Should().Be(_defaultTimezoneInfo.Id);
            
            resultDateTime2.Should().Be(expectedDateTime);
            resultTimezoneId2.Should().Be(_defaultTimezoneInfo.Id);
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
                _defaultTimezoneInfo, 
                _fixedDateTimeProvider);

            var expectedTimezone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            var expectedDateTime = TimeZoneInfo.ConvertTime(_fixedDateTime, expectedTimezone);

            // Act
            var (resultDateTime, resultTimezoneId) = timeService.GetCurrentTimeWithTimezone(tzId);

            // Assert
            resultDateTime.Should().Be(expectedDateTime);
            resultTimezoneId.Should().Be(tzId);
        }
        
        [Fact(DisplayName = "TS-011: GetCurrentTimeWithTimezone throws for invalid timezone")]
        public void TS011()
        {
            // Arrange
            var invalidTimezoneId = "Invalid_Timezone";
            var timeService = new TimeService(
                _loggerMock.Object,
                _defaultTimezoneInfo, 
                _fixedDateTimeProvider);

            // Act
            Action act = () => timeService.GetCurrentTimeWithTimezone(invalidTimezoneId);

            // Assert
            act.Should().Throw<TimeZoneNotFoundException>();
        }
    }
}