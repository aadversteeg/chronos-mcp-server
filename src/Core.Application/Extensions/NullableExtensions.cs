using Core.Application.Models;
using Ave.Extensions.Functional;
using System;

namespace Core.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for nullable types to simplify working with nullable values in a functional way.
    /// </summary>
    public static class NullableExtensions
    {
        /// <summary>
        /// Transforms a nullable string using a provided function if the string is not null.
        /// </summary>
        /// <typeparam name="T">The return type of the transformation function.</typeparam>
        /// <param name="source">The nullable string to transform.</param>
        /// <param name="func">The function to apply to the non-null string value.</param>
        /// <returns>
        /// A Result containing a nullable T with a value of default if the source was null,
        /// or the result of applying the function to the source string if it was not null.
        /// </returns>
        public static Result<T?, Error> Bind<T>(this string? source, Func<string, Result<T, Error>> func)
        {
            if (source == null)
            {
                return Result<T?, Error>.Success(default);
            }

            return func(source).Match(
                onSuccess: Result<T?, Error>.Success,
                onError: Result<T?, Error>.Failure);
        }

        /// <summary>
        /// Converts a nullable string to a Maybe&lt;string&gt;.
        /// Null or whitespace strings are converted to Maybe.None, otherwise to Maybe.From(value).
        /// </summary>
        /// <param name="source">The nullable string to convert.</param>
        /// <returns>
        /// A Maybe&lt;string&gt; that:
        /// - Has no value (None) when source is null or whitespace
        /// - Has a value (Some) when source is a non-empty string
        /// </returns>
        public static Maybe<string> ToMaybe(this string? source)
        {
            return string.IsNullOrWhiteSpace(source)
                ? Maybe<string>.None
                : Maybe<string>.From(source);
        }
    }
}