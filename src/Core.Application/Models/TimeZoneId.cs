using Ave.Extensions.Functional;
using System;

namespace Core.Application.Models
{
    public record TimeZoneId
    {
        public string Value { get; }

        private TimeZoneId(string value)
        {
            Value = value;
        }

        public static Result<TimeZoneId, Error> Create(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<TimeZoneId, Error>.Failure( Errors.NullValueForTimeZoneIdValue);
            }

            if (!TimeZoneInfo.TryFindSystemTimeZoneById(id, out var timeZone))
            {
                return Result<TimeZoneId, Error>.Failure(Errors.InvalidTimeZoneIdValue(id));
            }

            return Result<TimeZoneId, Error>.Success(new TimeZoneId(id));
        }
    }
}
