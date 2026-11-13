using Ave.Extensions.ErrorPaths;
using Ave.Extensions.Functional;
using Core.Application.Models;

namespace Core.Infrastructure.McpServer.Configuration
{
    /// <summary>
    /// Resolves the optional default timezone configuration value into a domain TimeZoneId.
    /// </summary>
    public static class DefaultTimeZoneIdResolver
    {
        /// <summary>
        /// Resolves a configured default timezone identifier.
        /// </summary>
        /// <param name="configuredTimeZoneId">Raw configured timezone identifier.</param>
        /// <returns>
        /// A successful result containing Maybe.None when no default is configured,
        /// a successful result containing Maybe.Some when the value is valid,
        /// or a failure result when the configured value is invalid.
        /// </returns>
        public static Result<Maybe<TimeZoneId>, Error> Resolve(string? configuredTimeZoneId)
        {
            if (string.IsNullOrWhiteSpace(configuredTimeZoneId))
            {
                return Result<Maybe<TimeZoneId>, Error>.Success(Maybe<TimeZoneId>.None);
            }

            return TimeZoneId.Create(configuredTimeZoneId)
                .OnSuccessBind(timeZoneId =>
                    Result<Maybe<TimeZoneId>, Error>.Success(Maybe<TimeZoneId>.From(timeZoneId)));
        }
    }
}
