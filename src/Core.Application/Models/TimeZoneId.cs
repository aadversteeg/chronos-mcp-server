using Ave.Extensions.ErrorPaths;
using Ave.Extensions.Functional;
using System;

namespace Core.Application.Models
{
    public record TimeZoneId
    {
        public string Value { get; }

        internal TimeZoneId(string value)
        {
            Value = value;
        }

        public static Result<TimeZoneId, Error> Create(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<TimeZoneId, Error>.Failure(ChronosErrors.NullValueForTimeZoneIdValue);
            }

            if (!TimeZoneInfo.TryFindSystemTimeZoneById(id, out var timeZone))
            {
                return Result<TimeZoneId, Error>.Failure(ChronosErrors.InvalidTimeZoneIdValue(id));
            }

            return Result<TimeZoneId, Error>.Success(new TimeZoneId(id));
        }
    }
}
