namespace Discord
{
    /// <summary>
    /// Parameter types that are supported by the Application Commands API
    /// </summary>
    public enum ApplicationCommandOptionType
    {
        /// <summary>
        /// A Slash Command that is nested in another Slash Command or a Group
        /// </summary>
        SubCommand = 1,
        /// <summary>
        /// A Group for Sub-Slash Commands
        /// </summary>
        SubCommandGroup = 2,
        /// <summary>
        /// Discord parameter type for <see cref="string"/>
        /// </summary>
        String = 3,
        /// <summary>
        /// Discord parameter type for <see cref="int"/>
        /// </summary>
        Integer = 4,
        /// <summary>
        /// Discord parameter type for <see cref="bool"/>
        /// </summary>
        Boolean = 5,
        /// <summary>
        /// Discord parameter type for <see cref="IUser"/>
        /// </summary>
        User = 6,
        /// <summary>
        /// Discord parameter type for <see cref="IChannel"/>
        /// </summary>
        Channel = 7,
        /// <summary>
        /// Discord parameter type for <see cref="IRole"/>
        /// </summary>
        Role = 8,
        /// <summary>
        /// Discord parameter type for <see cref="IMentionable"/>
        /// </summary>
        /// <remarks>
        /// Since all of the complex object types supported by Application Commands API are <see cref="IMentionable"/>s,
        /// this type of parameter is only registered either when it is explicitly declared, or as fallback
        /// </remarks>
        Mentionable = 9
    }
}
