using System;

namespace Discord.Interactions
{
    public class MinValueAttribute : Attribute
    {
        public double Value { get; set; }

        public MinValueAttribute(double value)
        {
            Value = value;
        }
    }
}
