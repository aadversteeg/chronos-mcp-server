using Ave.Extensions.Functional;
using Core.Application;
using Core.Application.Extensions;
using Core.Application.Models;
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
            var builder = Host.CreateApplicationBuilder(args);

            // Add appsettings.json configuration, use full path in case working folder is different
            string? basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            basePath ??= Directory.GetCurrentDirectory();

            builder.Configuration.AddJsonFile(
                Path.Combine(basePath, "appsettings.json"), 
                optional: true, 
                reloadOnChange: true);

            builder.Configuration.AddJsonFile(
                Path.Combine(basePath, $"appsettings.{builder.Environment.EnvironmentName}.json"), 
                optional: true, 
                reloadOnChange: true);

            builder.Configuration.AddEnvironmentVariables();

            // Configure logging
            builder.Logging.AddConsole(consoleLogOptions =>
            {
                // Configure all logs to go to stderr
                consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
            });

            var maybeDefaultTimeZoneId = builder.Configuration
                .GetValue<string>("DefaultTimeZoneId")
                .ToMaybe()
                .OnSomeBind(str => TimeZoneId.Create(str).ToMaybe());

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