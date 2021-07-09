namespace Discord.SlashCommands
{
    /// <summary>
    /// Sub-Command Group information that will be displayed on Discord
    /// </summary>
    /// <remarks>
    /// This will be substituted for the module name and description while registering the command to Discord,
    /// if the Name and Description values are missing from <see cref="SlashModuleInfo"/>
    /// </remarks>
    public class SlashGroupInfo
    {
        /// <summary>
        /// Get the name of the command group
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Get the description of the command group
        /// </summary>
        /// <remarks>
        /// This may be neglected if any other <see cref="SlashGroupInfo"/> with the same name has a description
        /// </remarks>
        public string Description { get; }
        internal SlashGroupInfo (string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
