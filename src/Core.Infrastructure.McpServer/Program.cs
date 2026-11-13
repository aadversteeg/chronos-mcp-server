using Ave.Extensions.Functional;
using Core.Application;
using Core.Infrastructure.McpServer.Configuration;
using Core.Infrastructure.McpServer.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Core.Infrastructure.McpServer
{
    internal class Program
    {
        /// <summary>
        /// Gets the version from the assembly's informational version attribute,
        /// which is set from the Version property in the project file.
        /// </summary>
        /// <returns>The version string, or "0.0.0" if not available</returns>
        private static string GetServerVersion()
        {
            return Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? "0.0.0";
        }
        
        static async Task Main(string[] args)
        {
            Console.Error.WriteLine("Starting MCP Chronos Server...");
            var builder = McpHostBuilder.Create(args);

            // Configure logging
            builder.Logging.AddConsole(consoleLogOptions =>
            {
                // Configure all logs to go to stderr
                consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
            });

            var configuredDefaultTimeZoneId = builder.Configuration.GetValue<string>("DefaultTimeZoneId");
            var maybeDefaultTimeZoneId = DefaultTimeZoneIdResolver.Resolve(configuredDefaultTimeZoneId)
                .Match(
                    onSuccess: resolvedTimeZoneId => resolvedTimeZoneId,
                    onError: error => throw new InvalidOperationException(
                        $"Invalid DefaultTimeZoneId '{configuredDefaultTimeZoneId}'. {error.Message}"));

            builder.Services.AddApplicationServices(
                maybeDefaultTimeZoneId,
                () => DateTime.UtcNow);
                
            // Register MCP server and reference the ChronosTools
            builder.Services
                .AddMcpServer(options =>
                {
                    options.ServerInfo = new()
                    {
                        Name = "Chronos",
                        Version = GetServerVersion()
                    };
                })
                .WithStdioServerTransport()
                .WithTools<ChronosTools>();
            
            await builder.Build().RunAsync();
        }
    }
}