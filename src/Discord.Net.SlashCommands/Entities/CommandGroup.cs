namespace Discord.SlashCommands
{
    /// <summary>
    /// Holds the information of a command group
    /// </summary>
    public class CommandGroup
    {
        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Description of the group
        /// </summary>
        public string Description { get; }

        internal CommandGroup (string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
