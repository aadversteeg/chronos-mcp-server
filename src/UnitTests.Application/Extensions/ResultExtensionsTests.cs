using Ave.Extensions.Functional;
using Core.Application.Models;
using Core.Application.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace UnitTests.Application.Extensions
{
    public class ResultExtensionsTests
    {
        [Fact(DisplayName = "RE-001: Unwrap returns the value for success result")]
        public void RE001()
        {
            // Arrange
            var expectedValue = "test data";
            var successResult = Result<string, Error>.Success(expectedValue);
            
            // Act
            var result = successResult.Unwrap();
            
            // Assert
            result.Should().Be(expectedValue);
        }

        [Fact(DisplayName = "RE-002: Unwrap throws InvalidOperationException with error message for failure result")]
        public void RE002()
        {
            // Arrange
            var errorMessage = "Test error message";
            var failureResult = Result<string, Error>.Failure(new ProtocolError(errorMessage, "ErrorCode"));
            
            // Act & Assert
            Action act = () => failureResult.Unwrap();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage(errorMessage);
        }

        [Fact(DisplayName = "RE-003: Unwrap with custom error message throws with that message")]
        public void RE003()
        {
            // Arrange
            var originalErrorMessage = "Original error message";
            var customErrorMessage = "Custom error message";
            var failureResult = Result<string, Error>.Failure(new ProtocolError(originalErrorMessage, "ErrorCode"));
            
            // Act & Assert
            Action act = () => failureResult.Unwrap(customErrorMessage);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage(customErrorMessage);
        }
        
        [Fact(DisplayName = "RE-004: Unwrap returns the value for success result with custom error message overload")]
        public void RE004()
        {
            // Arrange
            var expectedValue = "test data";
            var successResult = Result<string, Error>.Success(expectedValue);
            
            // Act
            var result = successResult.Unwrap("This error message should not be used");
            
            // Assert
            result.Should().Be(expectedValue);
        }
    }
}