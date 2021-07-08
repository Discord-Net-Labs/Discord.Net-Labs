namespace Discord
{
    /// <summary>
    /// Represents a parameter recived with a Slash Command Interaction
    /// </summary>
    public class InteractionParameter
    {
        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value of the parameter
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Type of the parameter
        /// </summary>
        public ApplicationCommandOptionType Type { get; }

        internal InteractionParameter (string name, object value, ApplicationCommandOptionType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }
    }
}
