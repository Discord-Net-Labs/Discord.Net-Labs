using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Attribute used to add a pre-determined argument value
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class ChoiceAttribute : Attribute
    {
        /// <summary>
        /// Name of the choice
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Type of this choice
        /// </summary>
        public SlashCommandChoiceType Type { get; }
        /// <summary>
        /// Value that will be used whenever this choice is selected
        /// </summary>
        public object Value { get; }

        private ChoiceAttribute (string name)
        {
            Name = name;
        }

        /// <summary>
        /// Create a parameter choice with type <see cref="SlashCommandChoiceType.String"/>
        /// </summary>
        /// <param name="name">Name of the choice</param>
        /// <param name="value">Predefined value of the choice</param>
        public ChoiceAttribute (string name, string value) : this(name)
        {
            Type = SlashCommandChoiceType.String;
            Value = value;
        }

        /// <summary>
        /// Create a parameter choice with type <see cref="SlashCommandChoiceType.Integer"/>
        /// </summary>
        /// <param name="name">Name of the choice</param>
        /// <param name="value">Predefined value of the choice</param>
        public ChoiceAttribute (string name, int value) : this(name)
        {
            Type = SlashCommandChoiceType.Integer;
            Value = value;
        }
    }
}
