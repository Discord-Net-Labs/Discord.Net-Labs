using System;

namespace Discord.Interactions
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class AutocompleteAttribute : Attribute
    {
        public Type AutocompleterType { get; }

        public AutocompleteAttribute(Type autocompleterType)
        {
            if (!typeof(IAutocompleter).IsAssignableFrom(autocompleterType))
                throw new InvalidOperationException($"{autocompleterType.FullName} isn't a valid Autocompleter type");

            AutocompleterType = autocompleterType;
        }

        public AutocompleteAttribute() { }
    }
}
