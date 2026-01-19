using System;
using Ave.Extensions.Functional;
using Core.Application.Models;
using Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Application
{
    /// <summary>
    /// Extension methods for registering Core.Application services with the DI container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Core.Application services to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to</param>
        /// <param name="maybeDefaultTimeZoneId">Optional default timezone to use</param>
        /// <param name="currentDateTimeProvider">Provider function for the current date and time (defaults to DateTime.UtcNow)</param>
        /// <returns>The same service collection so that multiple calls can be chained</returns>
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            Maybe<TimeZoneId> maybeDefaultTimeZoneId,
            Func<DateTime>? currentDateTimeProvider = null)
        {
            // Make sure logging is registered
            services.AddLogging();
            services.AddSingleton<ITimeService, TimeService>( sp =>
            {
                return new TimeService(
                    sp.GetRequiredService<ILogger<TimeService>>(),
                    maybeDefaultTimeZoneId.HasValue ? maybeDefaultTimeZoneId.Value : null,
                    currentDateTimeProvider ?? (() => DateTime.UtcNow)
                );
            });

            return services;
        }
    }
}