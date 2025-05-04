using Ave.Extensions.Functional;
using Core.Application.Models;
using Core.Infrastructure.McpServer.Extensions;
using FluentAssertions;
using ModelContextProtocol;
using System.Text.Json;

namespace UnitTests.Infrastructure.McpServer.Extensions
{
    public class ResultExtensionsTests
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        [Fact(DisplayName = "RE-001: ToToolResult serializes success result to JSON")]
        public void RE001()
        {
            // Arrange
            var successResult = Result<string, Error>.Success("test data");
            
            // Act
            var result = successResult.ToToolResult(data => new { Value = data });
            
            // Assert
            result.Should().NotBeNull();
            var deserialized = JsonSerializer.Deserialize<TestResponse>(result, _jsonOptions);
            deserialized.Should().NotBeNull();
            deserialized!.Value.Should().Be("test data");
        }

        [Fact(DisplayName = "RE-002: ToToolResult maps value before serialization")]
        public void RE002()
        {
            // Arrange
            var successResult = Result<int, Error>.Success(42);
            
            // Act
            var result = successResult.ToToolResult(num => new { DoubledValue = num * 2 });
            
            // Assert
            result.Should().NotBeNull();
            var deserialized = JsonSerializer.Deserialize<DoubleValueResponse>(result, _jsonOptions);
            deserialized.Should().NotBeNull();
            deserialized!.DoubledValue.Should().Be(84);
        }

        [Fact(DisplayName = "RE-003: ToToolResult throws McpException on failure")]
        public void RE003()
        {
            // Arrange
            var errorMessage = "Test error message";
            var failureResult = Result<string, Error>.Failure(new Error(errorMessage, "ErrorCode"));
            
            // Act & Assert
            Action act = () => failureResult.ToToolResult(data => new { Value = data });
            act.Should().Throw<McpException>()
                .WithMessage(errorMessage);
        }

        // Helper classes for deserialization
        private class TestResponse
        {
            public string? Value { get; set; }
        }

        private class DoubleValueResponse
        {
            public int DoubledValue { get; set; }
        }
    }
}