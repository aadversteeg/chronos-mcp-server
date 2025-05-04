using Core.Application;
using Core.Application.Models;
using Core.Application.Services;
using Core.Application.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace UnitTests.Application
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact(DisplayName = "SCE-002: AddCoreApplicationServices registers ITimeService")]
        public void SCE002()
        {
            // Arrange
            var services = new ServiceCollection();
            var defaultTimeZoneId = TimeZoneId.Create("UTC").Unwrap();

            // Act
            services.AddApplicationServices(defaultTimeZoneId);
            var serviceProvider = services.BuildServiceProvider();
            var timeService = serviceProvider.GetService<ITimeService>();

            // Assert
            timeService.Should().NotBeNull();
            timeService.Should().BeAssignableTo<TimeService>();
        }

        [Fact(DisplayName = "SCE-003: AddCoreApplicationServices uses provided time zone")]
        public void SCE003()
        {
            // Arrange
            var services = new ServiceCollection();
            var defaultTimeZoneId = TimeZoneId.Create("UTC").Unwrap();

            // Act
            services.AddApplicationServices(defaultTimeZoneId);
            var serviceProvider = services.BuildServiceProvider();
            var timeService = serviceProvider.GetService<ITimeService>();

            // Assert
            timeService.Should().NotBeNull();
            var getDefaultTimeZoneResult = timeService!.GetDefaultTimeZone();
            getDefaultTimeZoneResult.IsSuccess.Should().BeTrue();
            getDefaultTimeZoneResult.Value.Id.Should().Be("UTC");
        }

        [Fact(DisplayName = "SCE-004: AddCoreApplicationServices registers time service that can fetch timezone-based times")]
        public void SCE004()
        {
            // Arrange
            var services = new ServiceCollection();
            var defaultTimeZoneId = TimeZoneId.Create("UTC").Unwrap();

            // Act
            services.AddApplicationServices(defaultTimeZoneId);
            var serviceProvider = services.BuildServiceProvider();
            var timeService = serviceProvider.GetService<ITimeService>();

            // Assert
            timeService.Should().NotBeNull();
            var result = timeService!.GetCurrentTimeWithTimezone(null);
            result.IsSuccess.Should().BeTrue();
            result.Value.UsedTimezoneId.Value.Should().Be("UTC");
        }

        [Fact(DisplayName = "SCE-005: AddCoreApplicationServices works with null defaultTimeZoneId")]
        public void SCE005()
        {
            // Arrange
            var services = new ServiceCollection();
            TimeZoneId? defaultTimeZoneId = null;

            // Act
            services.AddApplicationServices(defaultTimeZoneId);
            var serviceProvider = services.BuildServiceProvider();
            var timeService = serviceProvider.GetService<ITimeService>();

            // Assert
            timeService.Should().NotBeNull();
            timeService.Should().BeAssignableTo<TimeService>();
        }
    }
}