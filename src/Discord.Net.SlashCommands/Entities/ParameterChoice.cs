namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents a Slash Command parameter choice
    /// </summary>
    public class ParameterChoice
    {
        /// <summary>
        /// Name of the choice
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The underlying value of the choice
        /// </summary>
        public object Value { get; }

        internal ParameterChoice (string name, string value)
        {
            Name = name;
            Value = value;
        }

        internal ParameterChoice (string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
