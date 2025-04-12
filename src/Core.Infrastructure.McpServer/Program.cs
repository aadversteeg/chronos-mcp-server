using Core.Infrastructure.McpServer.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.McpServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Error.WriteLine("Starting MCP Chronos Server...");
            var builder = Host.CreateApplicationBuilder(args);

            // Add appsettings.json configuration, use full path in case working folder is different
            string? basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
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
            
            // Get DefaultTimeZoneId from config 
            string defaultTimeZoneId = builder.Configuration.GetValue<string>("DefaultTimeZoneId") ?? "UTC";

            var defaultTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(defaultTimeZoneId);

            // Register ChronosToolSettings with DI
            builder.Services.AddSingleton(
                new ChronosToolSettings() { 
                    DefaultTimezoneInfo = defaultTimeZoneInfo
                });
                
            // Register MCP server and reference the ChronosTools
            builder.Services
                .AddMcpServer(options =>
                {
                    options.ServerInfo = new()
                    {
                        Name = "Chronos",
                        Version = "0.0.1"
                    };
                })
                .WithStdioServerTransport()
                .WithTools<ChronosTools>();
            
            await builder.Build().RunAsync();
        }
    }
}