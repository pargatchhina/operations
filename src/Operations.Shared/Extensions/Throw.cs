using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Operations.Shared.Extensions
{
    public static class Throw
    {
        /// <summary>
        ///     Throws an ArgumentNullException is the value is null
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="paramName">Name to include in exception message</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T ThrowIfNull<T>(this T value, string paramName)
        {
            if (value == null) throw new ArgumentNullException(paramName);

            return value;
        }

        /// <summary>
        ///     Throws an ArgumentNullException is the value is equal to default
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="paramName">Name to include in exception message</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T ThrowIfDefault<T>(this T value, string paramName)
        {
            if (value.Equals(default(T))) throw new ArgumentException(paramName);

            return value;
        }

        /// <summary>
        ///     Throws the appropriate Exception if the value is Null or Empty
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="paramName">Name to include in exception message</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string ThrowIfNullOrEmpty(this string value, string paramName)
        {
            if (value == null) throw new ArgumentNullException(paramName);

            if (string.IsNullOrEmpty(value)) throw new ArgumentException(paramName);

            return value;
        }

        /// <summary>
        ///     Throws the appropriate Exception if the value is Null, Empty or Whitespace
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="paramName">Name to include in exception message</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string ThrowIfNullOrWhiteSpace(this string value, string paramName)
        {
            if (value == null) throw new ArgumentNullException(paramName);

            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(paramName);

            return value;
        }

        /// <summary>
        ///     Throw if the given IEnumerable is null or empty.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The name of the value.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">When the value is null.</exception>
        /// <exception cref="ArgumentException">>When the value is empty.</exception>
        public static IEnumerable<T> ThrowIfNullOrEmpty<T>(this IEnumerable<T> value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (!value.Any())
            {
                throw new ArgumentException(paramName);
            }

            return value;
        }

        /// <summary>
        ///     Throws the appropriate Exception if the value is undefined
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="paramName">Name to include in exception message</param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static T ThrowIfEnumIsUndefined<T>(this T value, string paramName) where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), value)) throw new InvalidEnumArgumentException(paramName);

            return value;
        }

        /// <summary>
        ///     Throw if  the given <see cref="Predicate{T}"/> is true.
        /// </summary>
        /// <param name="value">The value of T to check.</param>
        /// <param name="predicate">The <see cref="Predicate{T}"/> to evaluate.</param>
        /// <param name="message">The error message.</param>
        /// <param name="paramName">The name of the value.</param>
        /// <typeparam name="TValue">The instance of the value.</typeparam>
        /// <returns>The instance of TValue.</returns>
        /// <exception cref="ArgumentException">If the the given predicate evaluates to false.</exception>
        public static TValue ThrowIf<TValue>(this TValue value, Func<TValue, bool> predicate, string message, string paramName)
        {
            if (predicate(value))
            {
                throw new ArgumentException(message, paramName);
            }

            return value;
        }
    }
}
