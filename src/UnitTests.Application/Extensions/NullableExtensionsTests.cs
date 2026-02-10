using Ave.Extensions.ErrorPaths;
using Ave.Extensions.Functional;
using Ave.Extensions.Functional.FluentAssertions;
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
                return Result<int, Error>.Failure(new Error(new ErrorCode("ParseError"), "Failed to parse"));
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
            var error = new Error(new ErrorCode("ParseError"), "Failed to parse");
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

        [Fact(DisplayName = "NE-003: ToMaybe with null string returns None")]
        public void NE003()
        {
            // Arrange
            string? source = null;

            // Act
            var result = source.ToMaybe();

            // Assert
            result.HasNoValue.Should().BeTrue();
        }

        [Fact(DisplayName = "NE-004: ToMaybe with empty string returns None")]
        public void NE004()
        {
            // Arrange
            string source = "";

            // Act
            var result = source.ToMaybe();

            // Assert
            result.HasNoValue.Should().BeTrue();
        }

        [Fact(DisplayName = "NE-005: ToMaybe with whitespace string returns None")]
        public void NE005()
        {
            // Arrange
            string source = "   ";

            // Act
            var result = source.ToMaybe();

            // Assert
            result.HasNoValue.Should().BeTrue();
        }

        [Fact(DisplayName = "NE-006: ToMaybe with valid string returns Some")]
        public void NE006()
        {
            // Arrange
            string source = "test value";

            // Act
            var result = source.ToMaybe();

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be("test value");
        }

        [Fact(DisplayName = "NE-007: ToMaybe with non-whitespace string returns Some")]
        public void NE007()
        {
            // Arrange
            string source = "UTC";

            // Act
            var result = source.ToMaybe();

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be("UTC");
        }

        [Fact(DisplayName = "NE-008: ToMaybe preserves the original string value")]
        public void NE008()
        {
            // Arrange
            string source = "Europe/Amsterdam";

            // Act
            var result = source.ToMaybe();

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be("Europe/Amsterdam");
        }
    }
}
