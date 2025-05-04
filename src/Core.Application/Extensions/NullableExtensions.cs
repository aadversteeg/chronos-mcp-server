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
        /// Ensures that a nullable reference type has a value, using a default value if provided.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <param name="source">The nullable source value.</param>
        /// <param name="defaultValue">The default value to use if source is null.</param>
        /// <param name="error">The error to return if both source and defaultValue are null.</param>
        /// <returns>
        /// A Result containing either:
        /// - The source value if it's not null
        /// - The defaultValue if source is null and defaultValue is not null
        /// - A failure with the specified error if both source and defaultValue are null
        /// </returns>
        public static Result<T, Error> Ensure<T>(this T? source, T? defaultValue, Error error) 
            where T : class
        {
            if (source == null)
            {
                if (defaultValue == null)
                {
                    return Result<T, Error>.Failure(error);
                }
                return Result<T, Error>.Success(defaultValue);
            }
            return Result<T, Error>.Success(source);
        }

        /// <summary>
        /// Ensures that a nullable value type has a value, using a default value if source is null.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="source">The nullable source value.</param>
        /// <param name="defaultValue">The default value to use if source is null.</param>
        /// <param name="error">The error to return if an error occurs (not used in this overload).</param>
        /// <returns>
        /// A Result containing either:
        /// - The source value if it has a value
        /// - The defaultValue if source is null
        /// </returns>
        public static Result<T, Error> Ensure<T>(this T? source, T defaultValue, Error error) 
            where T : struct
        {
            if (source == null)
            {
                return Result<T, Error>.Success(defaultValue);
            }
            return Result<T, Error>.Success(source.Value);
        }

        /// <summary>
        /// Ensures that a nullable value type has a value, returning an error if null.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="source">The nullable source value.</param>
        /// <param name="error">The error to return if the source is null.</param>
        /// <returns>
        /// A Result containing either:
        /// - The source value if it has a value
        /// - A failure with the specified error if source is null
        /// </returns>
        public static Result<T, Error> Ensure<T>(this T? source, Error error) 
            where T : struct
        {
            if (source == null)
            {
                return Result<T, Error>.Failure(error);
            }
            return Result<T, Error>.Success(source.Value);
        }
        
        /// <summary>
        /// Ensures that a nullable reference type has a value, returning an error if null.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <param name="source">The nullable source value.</param>
        /// <param name="error">The error to return if the source is null.</param>
        /// <returns>
        /// A Result containing either:
        /// - The source value if it's not null
        /// - A failure with the specified error if source is null
        /// </returns>
        public static Result<T, Error> Ensure<T>(this T? source, Error error) 
            where T : class
        {
            if (source == null)
            {
                return Result<T, Error>.Failure(error);
            }
            return Result<T, Error>.Success(source);
        }
    }
}