using Ave.Extensions.ErrorPaths;
using Ave.Extensions.Functional;
using System;
using System.Threading.Tasks;

namespace Core.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Result{T, TError}"/> to simplify working with Result objects.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Extracts the value from a successful Result or throws an exception if the Result is a failure.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the Result.</typeparam>
        /// <param name="result">The Result object to unwrap.</param>
        /// <returns>The value contained in the Result if it's a success.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Result is a failure, with the error message from the Result's Error.</exception>
        public static T Unwrap<T>(this Result<T, Error> result)
        {
            if (result.IsSuccess)
            {
                return result.Value;
            }

            throw new InvalidOperationException(result.Error.Message);
        }

        /// <summary>
        /// Extracts the value from a successful Result or throws an exception with a custom error message if the Result is a failure.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the Result.</typeparam>
        /// <param name="result">The Result object to unwrap.</param>
        /// <param name="errorMessage">The custom error message to use if the Result is a failure.</param>
        /// <returns>The value contained in the Result if it's a success.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Result is a failure, with the provided custom error message.</exception>
        public static T Unwrap<T>(this Result<T, Error> result, string errorMessage)
        {
            if (result.IsSuccess)
            {
                return result.Value;
            }

            throw new InvalidOperationException(errorMessage);
        }

        /// <summary>
        /// Binds an async operation to a Result, allowing chaining from Result to Task&lt;Result&gt;.
        /// If the source Result is successful, executes the async bind function with the success value.
        /// If the source Result is a failure, propagates the error without executing the bind function.
        /// </summary>
        /// <typeparam name="TIn">The type of the value in the source Result.</typeparam>
        /// <typeparam name="TOut">The type of the value in the output Result.</typeparam>
        /// <param name="source">The source Result to bind from.</param>
        /// <param name="bindAsync">The async function to bind with if the source is successful.</param>
        /// <returns>
        /// A Task that resolves to:
        /// - The result of the bind function if the source is successful
        /// - A failure Result with the source error if the source is a failure
        /// </returns>
        public static async Task<Result<TOut, Error>> OnSuccessBindAsync<TIn, TOut>(
            this Result<TIn, Error> source,
            Func<TIn, Task<Result<TOut, Error>>> bindAsync)
        {
            if (source.IsSuccess)
            {
                return await bindAsync(source.Value).ConfigureAwait(false);
            }

            return Result<TOut, Error>.Failure(source.Error);
        }

        /// <summary>
        /// Converts a Result to a Maybe, discarding any error information.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the Result.</typeparam>
        /// <param name="result">The Result to convert.</param>
        /// <returns>
        /// A Maybe that:
        /// - Has a value (Some) when the Result is successful
        /// - Has no value (None) when the Result is a failure
        /// </returns>
        public static Maybe<T> ToMaybe<T>(this Result<T, Error> result)
        {
            if (result.IsSuccess)
            {
                return Maybe<T>.From(result.Value);
            }

            return Maybe<T>.None;
        }
    }
}
