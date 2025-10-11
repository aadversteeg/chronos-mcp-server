using System;
using System.Collections.Generic;

namespace Core.Application.Models
{
    /// <summary>
    /// Base class for all errors in the application.
    /// </summary>
    public abstract record Error
    {
        protected Error(string message, string code, Dictionary<string, object>? context = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            Message = message;

            if (code == null)
                throw new ArgumentNullException(nameof(code));

            Code = code;
            Context = context;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets optional context information about the error.
        /// </summary>
        public Dictionary<string, object>? Context { get; }
    }

    /// <summary>
    /// Represents errors caused by invalid client input (validation failures).
    /// These are thrown as McpException and are not visible to the LLM.
    /// </summary>
    public record ProtocolError : Error
    {
        public ProtocolError(string message, string code, Dictionary<string, object>? context = null)
            : base(message, code, context)
        {
        }
    }

    /// <summary>
    /// Represents operational errors (runtime failures like network issues).
    /// These are returned as CallToolResult.IsError and are visible to the LLM for retry logic.
    /// </summary>
    public record ToolError : Error
    {
        public ToolError(string message, string code, Dictionary<string, object>? context = null)
            : base(message, code, context)
        {
        }
    }
}
