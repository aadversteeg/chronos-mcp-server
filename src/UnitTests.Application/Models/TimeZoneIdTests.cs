using System;
using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.FluentAssertions;
using Core.Application.Models;
using FluentAssertions;
using Ave.Extensions.Functional.FluentAssertions;
using Xunit;

namespace UnitTests.Application.Models
{
    public class TimeZoneIdTests
    {
        [Fact(DisplayName = "TZI-001: Create with valid timezone ID returns Success")]
        public void TZI001()
        {
            // Arrange
            string validTimezoneId = "UTC";

            // Try to get a valid timezone ID on the system
            try
            {
                var availableTimezones = TimeZoneInfo.GetSystemTimeZones();
                if (availableTimezones.Count > 0)
                {
                    validTimezoneId = availableTimezones[0].Id;
                }
            }
            catch
            {
                // Fall back to UTC if there's an issue
            }

            // Act
            var result = TimeZoneId.Create(validTimezoneId);

            // Assert
            result.Should().Succeed();
            result.Value.Value.Should().Be(validTimezoneId);
        }

        [Fact(DisplayName = "TZI-002: Create with null or empty timezone ID returns Failure")]
        public void TZI002()
        {
            // Arrange
            string? nullTimezoneId = null;
            string emptyTimezoneId = string.Empty;
            string whitespaceTimezoneId = "   ";

            // Act
            var resultNull = TimeZoneId.Create(nullTimezoneId!);
            var resultEmpty = TimeZoneId.Create(emptyTimezoneId);
            var resultWhitespace = TimeZoneId.Create(whitespaceTimezoneId);

            // Assert
            resultNull.Should().Fail();
            resultNull.Error.Should()
                .HaveCode(ChronosErrorCodes.Validation.RequiredTimeZoneId)
                .And.HaveMessage("TimeZoneId value cannot be null or empty.")
                .And.MatchCode(ErrorCodes.Validation._);

            resultEmpty.Should().Fail();
            resultEmpty.Error.Should()
                .HaveCode(ChronosErrorCodes.Validation.RequiredTimeZoneId)
                .And.HaveMessage("TimeZoneId value cannot be null or empty.")
                .And.MatchCode(ErrorCodes.Validation._);

            resultWhitespace.Should().Fail();
            resultWhitespace.Error.Should()
                .HaveCode(ChronosErrorCodes.Validation.RequiredTimeZoneId)
                .And.HaveMessage("TimeZoneId value cannot be null or empty.")
                .And.MatchCode(ErrorCodes.Validation._);
        }

        [Fact(DisplayName = "TZI-003: Create with invalid timezone ID returns Failure")]
        public void TZI003()
        {
            // Arrange
            string invalidTimezoneId = "Invalid_Timezone_ID_That_Does_Not_Exist";

            // Act
            var result = TimeZoneId.Create(invalidTimezoneId);

            // Assert
            result.Should().Fail();
            result.Error.Should()
                .HaveCode(ChronosErrorCodes.Validation.InvalidTimeZoneId)
                .And.HaveMessage($"Invalid TimeZoneId value: {invalidTimezoneId}.")
                .And.MatchCode(ErrorCodes.Validation._)
                .And.HaveMetadataEntry("providedValue", invalidTimezoneId);
        }

        [Fact(DisplayName = "TZI-004: TimeZoneId Value property returns the timezone ID")]
        public void TZI004()
        {
            // Arrange
            string validTimezoneId = "UTC";

            // Try to get a valid timezone ID on the system
            try
            {
                var availableTimezones = TimeZoneInfo.GetSystemTimeZones();
                if (availableTimezones.Count > 0)
                {
                    validTimezoneId = availableTimezones[0].Id;
                }
            }
            catch
            {
                // Fall back to UTC if there's an issue
            }

            // Act
            var result = TimeZoneId.Create(validTimezoneId);

            // Assert
            result.Should().Succeed();
            result.Value.Value.Should().Be(validTimezoneId);
        }
    }
}
