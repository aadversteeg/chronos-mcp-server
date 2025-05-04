using Ave.Extensions.Functional;
using System.Text.Json.Serialization;
using System.Text.Json;
using ModelContextProtocol;
using Core.Application.Models;

namespace Core.Infrastructure.McpServer.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Result{T, TError}"/> specifically for MCP tool operations.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// JSON serializer options configured for MCP tool responses.
        /// </summary>
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>
        /// Converts a Result to a serialized JSON string for MCP tool responses.
        /// </summary>
        /// <typeparam name="TIn">The type of the value contained in the Result.</typeparam>
        /// <typeparam name="TOut">The type to transform the Result value into before serialization.</typeparam>
        /// <param name="source">The Result object to convert.</param>
        /// <param name="map">A function to transform the Result value before serialization.</param>
        /// <returns>
        /// A JSON-serialized string representation of the transformed Result value if the Result is a success.
        /// </returns>
        /// <exception cref="McpException">
        /// Thrown when the Result is a failure, with the error message from the Result's Error.
        /// </exception>
        public static string ToToolResult<TIn, TOut>(this Result<TIn, Error> source, Func<TIn,TOut> map)
        {
            if (source.IsSuccess)
            {
                return JsonSerializer.Serialize(map(source.Value), JsonOptions);
            }
            else
            {
                throw new McpException(source.Error.Message);
            }
        }
    }
}
