using System;

namespace Discord.Interactions
{
    public class MaxValueAttribute : Attribute
    {
        public double Value { get; set; }

        public MaxValueAttribute(double value)
        {
            Value = value;
        }
    }
}
