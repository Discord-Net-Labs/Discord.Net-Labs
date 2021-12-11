using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Registers a parameter as a complex parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ComplexParameterAttribute : Attribute
    {
    }
}
