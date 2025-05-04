using System;
using Core.Application;
using Core.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace UnitTests.Application
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact(DisplayName = "SCE-001: AddCoreApplicationServices throws on null defaultTimeZone")]
        public void SCE001()
        {
            // Arrange
            var services = new ServiceCollection();
            TimeZoneInfo defaultTimeZone = null!;

            // Act
            Action act = () => services.AddApplicationServices(defaultTimeZone);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("defaultTimeZone");
        }

        [Fact(DisplayName = "SCE-002: AddCoreApplicationServices registers ITimeService")]
        public void SCE002()
        {
            // Arrange
            var services = new ServiceCollection();
            var defaultTimeZone = TimeZoneInfo.Utc;

            // Act
            services.AddApplicationServices(defaultTimeZone);
            var serviceProvider = services.BuildServiceProvider();
            var timeService = serviceProvider.GetService<ITimeService>();

            // Assert
            timeService.Should().NotBeNull();
            timeService.Should().BeAssignableTo<TimeService>();
        }

        [Fact(DisplayName = "SCE-003: AddCoreApplicationServices uses provided date time provider")]
        public void SCE003()
        {
            // Arrange
            var services = new ServiceCollection();
            var defaultTimeZone = TimeZoneInfo.Utc;
            var fixedDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Func<DateTime> dateTimeProvider = () => fixedDate;

            // Act
            services.AddApplicationServices(defaultTimeZone, dateTimeProvider);
            var serviceProvider = services.BuildServiceProvider();
            var timeService = serviceProvider.GetService<ITimeService>();

            // Assert
            timeService.Should().NotBeNull();
            var currentTime = timeService!.GetCurrentTime();
            currentTime.Should().Be(fixedDate);
        }

        [Fact(DisplayName = "SCE-004: AddCoreApplicationServices uses default date time provider if not provided")]
        public void SCE004()
        {
            // Arrange
            var services = new ServiceCollection();
            var defaultTimeZone = TimeZoneInfo.Utc;
            var beforeRegistration = DateTime.UtcNow;

            // Act
            services.AddApplicationServices(defaultTimeZone);
            var serviceProvider = services.BuildServiceProvider();
            var timeService = serviceProvider.GetService<ITimeService>();
            var afterRegistration = DateTime.UtcNow.AddSeconds(1); // Add buffer for test execution

            // Assert
            timeService.Should().NotBeNull();
            var currentTime = timeService!.GetCurrentTime();
            currentTime.Should().BeOnOrAfter(beforeRegistration);
            currentTime.Should().BeOnOrBefore(afterRegistration);
        }
    }
}