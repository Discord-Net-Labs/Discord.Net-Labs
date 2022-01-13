using System;

namespace Discord.Interactions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EnabledInDmAttribute : Attribute
    {
        public bool IsEnabled { get; }

        public EnabledInDmAttribute(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }
}
