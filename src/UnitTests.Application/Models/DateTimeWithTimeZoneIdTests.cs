using System;
using Core.Application.Models;
using FluentAssertions;
using Xunit;

namespace UnitTests.Application.Models
{
    public class DateTimeWithTimeZoneIdTests
    {
        [Fact(DisplayName = "DTWTZ-001: Constructor initializes properties correctly")]
        public void DTWTZ001()
        {
            // Arrange
            var currentDateTime = new DateTime(2023, 12, 25, 12, 0, 0);
            var timezoneIdString = "UTC";
            var timezoneIdResult = TimeZoneId.Create(timezoneIdString);
            timezoneIdResult.IsSuccess.Should().BeTrue();
            var timezoneId = timezoneIdResult.Value;

            // Act
            var dateTimeWithTimeZoneId = new DateTimeWithTimeZoneId(currentDateTime, timezoneId);

            // Assert
            dateTimeWithTimeZoneId.CurrentDateTime.Should().Be(currentDateTime);
            dateTimeWithTimeZoneId.UsedTimezoneId.Should().Be(timezoneId);
            dateTimeWithTimeZoneId.UsedTimezoneId.Value.Should().Be(timezoneIdString);
        }

        [Fact(DisplayName = "DTWTZ-002: Records with same values are equal")]
        public void DTWTZ002()
        {
            // Arrange
            var currentDateTime = new DateTime(2023, 12, 25, 12, 0, 0);
            var timezoneIdString = "UTC";
            var timezoneIdResult = TimeZoneId.Create(timezoneIdString);
            timezoneIdResult.IsSuccess.Should().BeTrue();
            var timezoneId = timezoneIdResult.Value;

            // Act
            var record1 = new DateTimeWithTimeZoneId(currentDateTime, timezoneId);
            var record2 = new DateTimeWithTimeZoneId(currentDateTime, timezoneId);

            // Assert
            record1.Should().Be(record2);
            record1.GetHashCode().Should().Be(record2.GetHashCode());
        }

        [Fact(DisplayName = "DTWTZ-003: Records with different values are not equal")]
        public void DTWTZ003()
        {
            // Arrange
            var currentDateTime1 = new DateTime(2023, 12, 25, 12, 0, 0);
            var currentDateTime2 = new DateTime(2023, 12, 25, 13, 0, 0);
            
            // Get two different timezone IDs (UTC and another timezone like GMT)
            var timezoneId1Result = TimeZoneId.Create("UTC");
            timezoneId1Result.IsSuccess.Should().BeTrue();
            var timezoneId1 = timezoneId1Result.Value;
            
            // Try to get another timezone - try with GMT or fall back to using the same timezone
            // but with a different instance of TimeZoneId to check value equality
            var timezoneId2Result = TimeZoneId.Create("GMT");
            if (timezoneId2Result.IsSuccess)
            {
                var timezoneId2 = timezoneId2Result.Value;
                
                // Act
                var record1 = new DateTimeWithTimeZoneId(currentDateTime1, timezoneId1);
                var record2 = new DateTimeWithTimeZoneId(currentDateTime2, timezoneId1);
                var record3 = new DateTimeWithTimeZoneId(currentDateTime1, timezoneId2);

                // Assert
                record1.Should().NotBe(record2); // Different DateTime
                record1.Should().NotBe(record3); // Different TimeZoneId
            }
            else
            {
                // If GMT is not available, use the same timezone ID but with a different DateTime
                // Act
                var record1 = new DateTimeWithTimeZoneId(currentDateTime1, timezoneId1);
                var record2 = new DateTimeWithTimeZoneId(currentDateTime2, timezoneId1);

                // Assert
                record1.Should().NotBe(record2); // Different DateTime
            }
        }

        [Fact(DisplayName = "DTWTZ-004: Records support value deconstruction")]
        public void DTWTZ004()
        {
            // Arrange
            var currentDateTime = new DateTime(2023, 12, 25, 12, 0, 0);
            var timezoneIdString = "UTC";
            var timezoneIdResult = TimeZoneId.Create(timezoneIdString);
            timezoneIdResult.IsSuccess.Should().BeTrue();
            var timezoneId = timezoneIdResult.Value;
            
            var record = new DateTimeWithTimeZoneId(currentDateTime, timezoneId);

            // Act
            var (extractedDateTime, extractedTimezoneId) = record;

            // Assert
            extractedDateTime.Should().Be(currentDateTime);
            extractedTimezoneId.Should().Be(timezoneId);
            extractedTimezoneId.Value.Should().Be(timezoneIdString);
        }

        [Fact(DisplayName = "DTWTZ-005: ToString provides meaningful representation")]
        public void DTWTZ005()
        {
            // Arrange
            var currentDateTime = new DateTime(2023, 12, 25, 12, 0, 0);
            var timezoneIdString = "UTC";
            var timezoneIdResult = TimeZoneId.Create(timezoneIdString);
            timezoneIdResult.IsSuccess.Should().BeTrue();
            var timezoneId = timezoneIdResult.Value;
            
            var record = new DateTimeWithTimeZoneId(currentDateTime, timezoneId);

            // Act
            var stringRepresentation = record.ToString();

            // Assert
            stringRepresentation.Should().Contain(currentDateTime.ToString());
            stringRepresentation.Should().Contain(timezoneId.ToString());
        }
    }
}