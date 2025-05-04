using Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.McpServer.Tools
{
    /// <summary>
    /// Settings for the Chronos tools.
    /// </summary>
    public class ChronosToolSettings
    {
        /// <summary>
        /// Gets or sets the default timezone info.
        /// </summary>
        public TimeZoneInfo DefaultTimezoneInfo { get; set; } = TimeZoneInfo.Utc;
        
        /// <summary>
        /// Gets or sets the function that provides the current date and time.
        /// </summary>
        public Func<DateTime> CurrentDateTimeProvider { get; set; } = () => DateTime.UtcNow;
        
        /// <summary>
        /// Registers the time service with the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to register with</param>
        /// <returns>The service collection for chaining</returns>
        public IServiceCollection RegisterTimeService(IServiceCollection services)
        {
            return Core.Application.ServiceCollectionExtensions.AddApplicationServices(
                services,
                DefaultTimezoneInfo,
                CurrentDateTimeProvider);
        }
    }
}
