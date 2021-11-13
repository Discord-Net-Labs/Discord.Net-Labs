using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Set the minimum value permitted for a number type parameter
    /// </summary>
    public sealed class MinValueAttribute : Attribute
    {
        /// <summary>
        ///     The minimum value permitted
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        ///     Set the minimum value permitted for a number type parameter
        /// </summary>
        /// <param name="value">The minimum value permitted</param>
        public MinValueAttribute(double value)
        {
            Value = value;
        }
    }
}
