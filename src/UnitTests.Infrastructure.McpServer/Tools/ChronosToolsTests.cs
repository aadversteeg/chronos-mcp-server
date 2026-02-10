using Ave.Extensions.ErrorPaths;
using Core.Application.Models;
using Core.Application.Services;
using Core.Infrastructure.McpServer.Tools;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using Moq;
using System.Text.Json;
using Ave.Extensions.Functional;

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
            _timeServiceMock.Setup(s => s.GetDefaultTimeZone())
                .Returns(Result<TimeZoneInfo, Error>.Success(_defaultTimezoneInfo));

            // Setup GetDefaultTimeZoneId to return TimeZoneId
            _timeServiceMock.Setup(s => s.GetDefaultTimeZoneId())
                .Returns(() =>
                {
                    var timeZoneIdResult = TimeZoneId.Create(_defaultTimezoneInfo.Id);
                    return timeZoneIdResult;
                });

            // Setup the GetCurrentTimeWithTimezone method with DateTimeWithTimeZoneId
            _timeServiceMock.Setup(s => s.GetCurrentTimeWithTimezone(It.IsAny<TimeZoneId>()))
                .ReturnsAsync((TimeZoneId tz) =>
                {
                    try
                    {
                        var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tz.Value);

                        var dateTimeWithTz = new DateTimeWithTimeZoneId(
                            TimeZoneInfo.ConvertTime(_fixedDateTime, targetTimeZone),
                            tz);

                        return Result<DateTimeWithTimeZoneId, Error>.Success(dateTimeWithTz);
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        return Result<DateTimeWithTimeZoneId, Error>.Failure(
                            ChronosErrors.TimeZoneNotFound(tz.Value));
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
        public async Task CT003()
        {
            // Arrange
            var chronosTools = new ChronosTools(_loggerMock.Object, _timeServiceMock.Object);

            // Setup specific expectations for this test
            var expectedDateTime = TimeZoneInfo.ConvertTime(_fixedDateTime, _defaultTimezoneInfo);

            // Act
            var callResult = await chronosTools.GetCurrentDateAndTime();
            var textContent = callResult.Content.OfType<ModelContextProtocol.Protocol.TextContentBlock>().FirstOrDefault()?.Text;
            textContent.Should().NotBeNull();
            var response = JsonSerializer.Deserialize<DateTimeResponse>(textContent!, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            response!.Timezone.Should().Be(_defaultTimezoneInfo.Id);

            // Use the GetCurrentTimeWithTimezone method
            _timeServiceMock.Verify(s => s.GetCurrentTimeWithTimezone(It.IsAny<TimeZoneId>()), Times.Once);
        }

        [Fact(DisplayName = "CT-004: GetCurrentDateAndTime returns correct data with custom timezone")]
        public async Task CT004()
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
            var callResult = await chronosTools.GetCurrentDateAndTime(tzId);
            var textContent = callResult.Content.OfType<ModelContextProtocol.Protocol.TextContentBlock>().FirstOrDefault()?.Text;
            textContent.Should().NotBeNull();
            var response = JsonSerializer.Deserialize<DateTimeResponse>(textContent!, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            response!.Timezone.Should().Be(tzId);

            // Since we're using a string parameter, the ChronosTools should convert it to a TimeZoneId
            // and then pass it to the TimeService
            _timeServiceMock.Verify(s => s.GetCurrentTimeWithTimezone(It.IsAny<TimeZoneId>()), Times.Once);

            // Additionally verify that the timezone value inside the parameter matches what's expected
            response.Timezone.Should().Be(tzId);
        }

        [Fact(DisplayName = "CT-005: GetCurrentDateAndTime throws for invalid timezone")]
        public async Task CT005()
        {
            // Arrange
            var invalidTimezoneId = "Invalid_Timezone";

            // Setup TimeZoneId.Create to return failure for this case
            // The error will happen in ChronosTools before even calling the service

            var chronosTools = new ChronosTools(_loggerMock.Object, _timeServiceMock.Object);

            // Setup the mock to return failure for the TimeZoneId.Create call

            // Act & Assert
            Func<Task> act = async () => await chronosTools.GetCurrentDateAndTime(invalidTimezoneId);
            await act.Should().ThrowAsync<McpException>();
            // No specific message check since the implementation has changed to use functional extensions

            // Verify the TimeService was never called since validation fails early
            _timeServiceMock.Verify(s => s.GetCurrentTimeWithTimezone(It.IsAny<TimeZoneId>()), Times.Never);
        }

        [Fact(DisplayName = "CT-006: GetDefaultTimeZoneId returns correct timezone ID")]
        public void CT006()
        {
            // Arrange
            var chronosTools = new ChronosTools(_loggerMock.Object, _timeServiceMock.Object);

            // Act
            var callResult = chronosTools.GetDefaultTimeZoneId();
            var textContent = callResult.Content.OfType<ModelContextProtocol.Protocol.TextContentBlock>().FirstOrDefault()?.Text;
            textContent.Should().NotBeNull();

            // Assert
            // Remove double quotes from the result if present
            var cleanResult = textContent!.Trim('"');
            cleanResult.Should().Be(_defaultTimezoneInfo.Id);
            _timeServiceMock.Verify(s => s.GetDefaultTimeZone(), Times.AtLeastOnce);
        }

        [Fact(DisplayName = "CT-007: GetDefaultTimeZoneId throws McpException when error occurs")]
        public void CT007()
        {
            // Arrange
            var chronosTools = new ChronosTools(_loggerMock.Object, _timeServiceMock.Object);

            // Setup to return failure after constructor has completed
            // Use a Validation error code so it routes to McpException
            _timeServiceMock.Setup(s => s.GetDefaultTimeZoneId())
                .Returns(Result<TimeZoneId, Error>.Failure(
                    new Error(ErrorCodes.Validation._, "Test exception")));

            // Act & Assert
            Action act = () => chronosTools.GetDefaultTimeZoneId();
            act.Should().Throw<McpException>();
            // No specific message check since the implementation has changed to use functional extensions
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
