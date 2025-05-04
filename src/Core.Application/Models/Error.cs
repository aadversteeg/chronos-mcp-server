using System;

namespace Core.Application.Models
{
    public record Error
    {
        public Error(string message, string code)
        {
            if (message == null) 
                throw new ArgumentNullException(nameof(message));

            Message = message;

            if (code == null) 
                throw new ArgumentNullException(nameof(code));

            Code = code;
        }

        public string Message { get; }

        public string Code { get; }
    }
}
