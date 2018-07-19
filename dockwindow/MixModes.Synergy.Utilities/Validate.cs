using System;

namespace MixModes.Synergy.Utilities
{
    /// <summary>
    /// Validation class
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Validates that a parameter is not null
        /// </summary>
        /// <param name="param">Parameter to verify</param>
        /// <param name="paramName">Name of the parameter</param>
        /// <exception cref="ArgumentNullException">Parameter is null</exception>
        public static void NotNull(object param, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Validates that a string parameter is not null or empty
        /// </summary>
        /// <param name="param">String parameter to verify</param>
        /// <param name="paramName">Name of the string parameter</param>
        /// <exception cref="ArgumentOutOfRangeException">String parameter is null or empty</exception>
        public static void StringNotNullOrEmpty(string param, string paramName)
        {
            NotNull(param, paramName);
            if (string.IsNullOrEmpty(param))
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        /// <summary>
        /// Asserts the specified condition
        /// </summary>
        /// <typeparam name="E">Type of exception to throw if assertion fails</typeparam>
        /// <param name="condition">Assertion condition</param>
        /// <exception cref="E">Condition evaluates to false</exception>
        public static void Assert<E>(bool condition) where E : Exception, new()
        {
            if (!condition)
            {
                throw new E();
            }
        }
    }
}
