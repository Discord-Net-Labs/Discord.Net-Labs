namespace Discord.Rest
{
    /// <summary>
    ///     Represents a type of Rest-based command.
    /// </summary>
    public enum ApplicationCommandType
    {
        /// <summary>
        ///     Specifies that this command is a Global command.
        /// </summary>
        GlobalCommand,

        /// <summary>
        ///     Specifies that this command is a Guild specific command.
        /// </summary>
        GuildCommand
    }
}
