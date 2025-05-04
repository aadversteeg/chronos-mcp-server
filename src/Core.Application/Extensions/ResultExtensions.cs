using Ave.Extensions.Functional;
using Core.Application.Models;
using System;

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
    }
}