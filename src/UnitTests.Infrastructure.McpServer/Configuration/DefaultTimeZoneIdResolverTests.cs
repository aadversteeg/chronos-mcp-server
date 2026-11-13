using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.FluentAssertions;
using Core.Application.Models;
using Core.Infrastructure.McpServer.Configuration;
using FluentAssertions;

namespace UnitTests.Infrastructure.McpServer.Configuration
{
    public class DefaultTimeZoneIdResolverTests
    {
        [Fact(DisplayName = "DTR-001: Resolve returns Success(None) for null input")]
        public void DTR001()
        {
            // Arrange
            string? configuredTimeZoneId = null;

            // Act
            var result = DefaultTimeZoneIdResolver.Resolve(configuredTimeZoneId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.HasNoValue.Should().BeTrue();
        }

        [Fact(DisplayName = "DTR-002: Resolve returns Success(None) for whitespace input")]
        public void DTR002()
        {
            // Arrange
            const string configuredTimeZoneId = "   ";

            // Act
            var result = DefaultTimeZoneIdResolver.Resolve(configuredTimeZoneId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.HasNoValue.Should().BeTrue();
        }

        [Fact(DisplayName = "DTR-003: Resolve returns Success(Some) for valid timezone")]
        public void DTR003()
        {
            // Arrange
            const string configuredTimeZoneId = "UTC";

            // Act
            var result = DefaultTimeZoneIdResolver.Resolve(configuredTimeZoneId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.HasValue.Should().BeTrue();
            result.Value.Value.Value.Should().Be(configuredTimeZoneId);
        }

        [Fact(DisplayName = "DTR-004: Resolve returns Failure for invalid configured timezone")]
        public void DTR004()
        {
            // Arrange
            const string invalidTimeZoneId = "Invalid_Timezone_ID_That_Does_Not_Exist";

            // Act
            var result = DefaultTimeZoneIdResolver.Resolve(invalidTimeZoneId);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should()
                .HaveCode(ChronosErrorCodes.Validation.InvalidTimeZoneId)
                .And.HaveMessage($"Invalid TimeZoneId value: {invalidTimeZoneId}.")
                .And.MatchCode(ErrorCodes.Validation._)
                .And.HaveMetadataEntry("providedValue", invalidTimeZoneId);
        }
    }
}
