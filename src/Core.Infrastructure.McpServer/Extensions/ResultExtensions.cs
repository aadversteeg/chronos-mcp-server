using Ave.Extensions.ErrorPaths;
using Ave.Extensions.Functional;
using System.Text.Json.Serialization;
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;

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
        /// Converts a Result to a CallToolResult for MCP tool responses.
        /// - On success: Returns CallToolResult with JSON-serialized transformed value as content
        /// - On validation/configuration error: Throws McpException (invisible to LLM)
        /// - On operational error: Returns CallToolResult.IsError (visible to LLM for retry)
        /// </summary>
        /// <typeparam name="TIn">The type of the value contained in the Result.</typeparam>
        /// <typeparam name="TOut">The type to transform the Result value into before serialization.</typeparam>
        /// <param name="source">The Result object to convert.</param>
        /// <param name="map">A function to transform the Result value before serialization.</param>
        /// <returns>
        /// A CallToolResult containing the JSON-serialized transformed value on success,
        /// or an error result for tool execution failures.
        /// </returns>
        /// <exception cref="McpException">
        /// Thrown when the Result contains a validation or configuration error.
        /// </exception>
        public static CallToolResult ToCallToolResult<TIn, TOut>(this Result<TIn, Error> source, Func<TIn, TOut> map)
        {
            if (source.IsSuccess)
            {
                var mappedValue = map(source.Value);

                // Special handling for string results - don't JSON-serialize them
                var content = mappedValue is string str
                    ? str
                    : JsonSerializer.Serialize(mappedValue, JsonOptions);

                return new CallToolResult
                {
                    Content = [new TextContentBlock { Text = content }],
                    IsError = false
                };
            }
            else
            {
                return source.Error.ToCallToolResult();
            }
        }

        /// <summary>
        /// Converts a Result directly to a CallToolResult for MCP tool responses.
        /// - On success: Returns CallToolResult with JSON-serialized value as content
        /// - On validation/configuration error: Throws McpException (invisible to LLM)
        /// - On operational error: Returns CallToolResult.IsError (visible to LLM for retry)
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the Result.</typeparam>
        /// <param name="source">The Result object to convert.</param>
        /// <returns>
        /// A CallToolResult containing the JSON-serialized value on success,
        /// or an error result for tool execution failures.
        /// </returns>
        /// <exception cref="McpException">
        /// Thrown when the Result contains a validation or configuration error.
        /// </exception>
        public static CallToolResult ToCallToolResult<T>(this Result<T, Error> source)
        {
            if (source.IsSuccess)
            {
                var content = JsonSerializer.Serialize(source.Value, JsonOptions);
                return new CallToolResult
                {
                    Content = [new TextContentBlock { Text = content }],
                    IsError = false
                };
            }
            else
            {
                return source.Error.ToCallToolResult();
            }
        }

        /// <summary>
        /// Converts an awaitable Result to a CallToolResult for MCP tool responses.
        /// - On success: Returns CallToolResult with JSON-serialized transformed value as content
        /// - On validation/configuration error: Throws McpException (invisible to LLM)
        /// - On operational error: Returns CallToolResult.IsError (visible to LLM for retry)
        /// </summary>
        /// <typeparam name="TIn">The type of the value contained in the Result.</typeparam>
        /// <typeparam name="TOut">The type to transform the Result value into before serialization.</typeparam>
        /// <param name="awaitableSource">A task that resolves to a Result.</param>
        /// <param name="map">A function to transform the Result value before serialization.</param>
        /// <returns>
        /// A task that resolves to a CallToolResult containing the JSON-serialized transformed value on success,
        /// or an error result for tool execution failures.
        /// </returns>
        /// <exception cref="McpException">
        /// Thrown when the Result contains a validation or configuration error.
        /// </exception>
        public static async Task<CallToolResult> ToCallToolResult<TIn, TOut>(this Task<Result<TIn, Error>> awaitableSource, Func<TIn, TOut> map)
        {
            var source = await awaitableSource.ConfigureAwait(false);
            return source.ToCallToolResult(map);
        }

        /// <summary>
        /// Converts an awaitable Result directly to a CallToolResult for MCP tool responses.
        /// - On success: Returns CallToolResult with JSON-serialized value as content
        /// - On validation/configuration error: Throws McpException (invisible to LLM)
        /// - On operational error: Returns CallToolResult.IsError (visible to LLM for retry)
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the Result.</typeparam>
        /// <param name="awaitableSource">A task that resolves to a Result.</param>
        /// <returns>
        /// A task that resolves to a CallToolResult containing the JSON-serialized value on success,
        /// or an error result for tool execution failures.
        /// </returns>
        /// <exception cref="McpException">
        /// Thrown when the Result contains a validation or configuration error.
        /// </exception>
        public static async Task<CallToolResult> ToCallToolResult<T>(this Task<Result<T, Error>> awaitableSource)
        {
            var source = await awaitableSource.ConfigureAwait(false);
            return source.ToCallToolResult();
        }

        /// <summary>
        /// Converts an Error to a CallToolResult or throws McpException based on error code hierarchy.
        /// - Validation or Configuration errors: Throws McpException (invisible to LLM)
        /// - All other errors: Returns CallToolResult.IsError (visible to LLM for retry)
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <returns>A CallToolResult for operational errors.</returns>
        /// <exception cref="McpException">Thrown for validation or configuration errors.</exception>
        private static CallToolResult ToCallToolResult(this Error error)
        {
            if (error.Is(ErrorCodes.Validation._) || error.Is(ErrorCodes.Internal.Configuration))
            {
                throw new McpException(FormatErrorMessage(error));
            }

            return new CallToolResult
            {
                Content = [new TextContentBlock { Text = FormatErrorMessage(error) }],
                IsError = true
            };
        }

        /// <summary>
        /// Formats an error message including error code and metadata if available.
        /// </summary>
        /// <param name="error">The error to format.</param>
        /// <returns>A formatted error message.</returns>
        private static string FormatErrorMessage(Error error)
        {
            var message = $"[{error.Code}] {error.Message}";

            if (error.Metadata != null && error.Metadata.Count > 0)
            {
                var contextJson = JsonSerializer.Serialize(error.Metadata, JsonOptions);
                message = $"{message}\nContext: {contextJson}";
            }

            return message;
        }
    }
}
