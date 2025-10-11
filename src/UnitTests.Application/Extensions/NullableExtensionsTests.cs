using Ave.Extensions.Functional;
using Ave.Extensions.Functional.FluentAssertions;
using Core.Application.Models;
using Core.Application.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace UnitTests.Application.Extensions
{
    public class NullableExtensionsTests
    {
        [Fact(DisplayName = "NE-001: Bind works with custom transformation functions")]
        public void NE001()
        {
            // Arrange
            string? input = "42";
            Func<string, Result<int, Error>> parseFunc = str => 
            {
                if (int.TryParse(str, out int value))
                {
                    return Result<int, Error>.Success(value);
                }
                return Result<int, Error>.Failure(new ProtocolError("Failed to parse", "ParseError"));
            };
            
            // Act
            var result = input.Bind(parseFunc);
            
            // Assert
            result.Should().SucceedWith(42);
        }

        [Fact(DisplayName = "NE-002: Bind propagates error with custom functions")]
        public void NE002()
        {
            // Arrange
            string? input = "not a number";
            var error = new ProtocolError("Failed to parse", "ParseError");
            Func<string, Result<int, Error>> parseFunc = str => 
            {
                if (int.TryParse(str, out int value))
                {
                    return Result<int, Error>.Success(value);
                }
                return Result<int, Error>.Failure(error);
            };
            
            // Act
            var result = input.Bind(parseFunc);
            
            // Assert
            result.Should().FailWith(error);
        }

        [Fact(DisplayName = "NE-003: Ensure with null reference type and non-null default returns success with default")]
        public void NE003()
        {
            // Arrange
            string? nullString = null;
            string defaultValue = "default";
            var error = new ProtocolError("Test error", "TestError");
            
            // Act
            var result = nullString.Ensure(defaultValue, error);
            
            // Assert
            result.Should().SucceedWith("default");
        }
        
        [Fact(DisplayName = "NE-004: Ensure with null reference type and null default returns failure")]
        public void NE004()
        {
            // Arrange
            string? nullString = null;
            string? defaultValue = null;
            var error = new ProtocolError("Test error", "TestError");
            
            // Act
            var result = nullString.Ensure(defaultValue, error);
            
            // Assert
            result.Should().FailWith(error);
        }

        [Fact(DisplayName = "NE-005: Ensure with non-null reference type returns success with source value")]
        public void NE005()
        {
            // Arrange
            string nonNullString = "test";
            string defaultValue = "default";
            var error = new ProtocolError("Test error", "TestError");
            
            // Act
            var result = nonNullString.Ensure(defaultValue, error);
            
            // Assert
            result.Should().SucceedWith("test");
        }
        
        [Fact(DisplayName = "NE-006: Ensure with null value type and default returns success with default")]
        public void NE006()
        {
            // Arrange
            int? nullInt = null;
            int defaultValue = 42;
            var error = new ProtocolError("Test error", "TestError");
            
            // Act
            var result = nullInt.Ensure(defaultValue, error);
            
            // Assert
            result.Should().SucceedWith(42);
        }
        
        [Fact(DisplayName = "NE-007: Ensure with non-null value type returns success with source value")]
        public void NE007()
        {
            // Arrange
            int? nonNullInt = 10;
            int defaultValue = 42;
            var error = new ProtocolError("Test error", "TestError");
            
            // Act
            var result = nonNullInt.Ensure(defaultValue, error);
            
            // Assert
            result.Should().SucceedWith(10);
        }
        
        [Fact(DisplayName = "NE-008: Ensure with null reference type and no default returns failure")]
        public void NE008()
        {
            // Arrange
            string? source = null;
            var error = new ProtocolError("Test error", "TestError");

            // Act
            var result = source.Ensure(error);

            // Assert
            result.Should().FailWith(error);
        }

        [Fact(DisplayName = "NE-009: Ensure with non-null reference type and no default returns success with source value")]
        public void NE009()
        {
            // Arrange
            string source = "test data";
            var error = new ProtocolError("Test error", "TestError");

            // Act
            var result = source.Ensure(error);

            // Assert
            result.IsSuccess.Should().BeTrue("Non-null reference should result in success");
            result.Value.Should().Be(source);
        }

        [Fact(DisplayName = "NE-010: Ensure with null value type and no default returns failure")]
        public void NE010()
        {
            // Arrange
            int? source = null;
            var error = new ProtocolError("Test error", "TestError");

            // Act
            var result = source.Ensure(error);

            // Assert
            result.IsFailure.Should().BeTrue("Null value type should result in failure");
            result.Error.Should().Be(error);
        }

        [Fact(DisplayName = "NE-011: Ensure with non-null value type and no default returns success with source value")]
        public void NE011()
        {
            // Arrange
            int? source = 42;
            var error = new ProtocolError("Test error", "TestError");

            // Act
            var result = source.Ensure(error);

            // Assert
            result.IsSuccess.Should().BeTrue("Non-null value type should result in success");
            result.Value.Should().Be(42);
        }
        
        [Fact(DisplayName = "NE-012: Ensure with TimeZoneId works correctly for null value")]
        public void NE012()
        {
            // Arrange
            TimeZoneId? source = null;
            
            // Act
            var result = source.Ensure(ProtocolErrors.NoDefaultTimeZoneId);

            // Assert
            result.IsFailure.Should().BeTrue("Null TimeZoneId should result in failure");
            result.Error.Should().Be(ProtocolErrors.NoDefaultTimeZoneId);
        }

        [Fact(DisplayName = "NE-013: Ensure with TimeZoneId works correctly for non-null value")]
        public void NE013()
        {
            // Arrange
            // Try to create a valid TimeZoneId
            var createResult = TimeZoneId.Create("UTC");
            createResult.IsSuccess.Should().BeTrue("UTC should be a valid timezone");
            
            TimeZoneId source = createResult.Value;
            
            // Act
            var result = source.Ensure(ProtocolErrors.NoDefaultTimeZoneId);

            // Assert
            result.IsSuccess.Should().BeTrue("Non-null TimeZoneId should result in success");
            result.Value.Should().Be(source);
        }
    }
}