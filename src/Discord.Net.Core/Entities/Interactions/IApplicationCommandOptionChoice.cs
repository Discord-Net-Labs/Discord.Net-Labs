namespace Discord
{
    /// <summary>
    ///     Specifies choices for command group.
    /// </summary>
    public interface IApplicationCommandOptionChoice
    {
        /// <summary>
        ///     1-100 character choice name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     value of the choice.
        /// </summary>
        object Value { get; }

    }
}
