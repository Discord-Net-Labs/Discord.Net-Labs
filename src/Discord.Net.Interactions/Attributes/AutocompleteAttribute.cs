using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Set the <see cref="ApplicationCommandOptionProperties.Autocomplete"/> to <see langword="true"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class AutocompleteAttribute : Attribute
    {
        /// <summary>
        ///     Type of the <see cref="Autocompleter"/>
        /// </summary>
        public Type AutocompleterType { get; }

        /// <summary>
        ///     Set the <see cref="ApplicationCommandOptionProperties.Autocomplete"/> to <see langword="true"/> and define a <see cref="Autocompleter"/> to handle
        ///     Autocomplete interactions targeting the parameter this <see cref="Attribute"/> is applied to
        /// </summary>
        /// <remarks>
        ///     <see cref="InteractionServiceConfig.EnableAutocompleters"/> must be set to <see langword="true"/> to use this constructor
        /// </remarks>
        public AutocompleteAttribute(Type autocompleterType)
        {
            if (!typeof(IAutocompleter).IsAssignableFrom(autocompleterType))
                throw new InvalidOperationException($"{autocompleterType.FullName} isn't a valid Autocompleter type");

            AutocompleterType = autocompleterType;
        }

        /// <summary>
        ///     Set the <see cref="ApplicationCommandOptionProperties.Autocomplete"/> to <see langword="true"/> without specifying a <see cref="Autocompleter"/>
        /// </summary>
        public AutocompleteAttribute() { }
    }
}
