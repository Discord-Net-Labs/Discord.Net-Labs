namespace Discord
{
    /// <summary>
    ///     Represents a general container for any type of emoji in a message.
    /// </summary>
    public interface IEmoji
    {
        /// <summary>
        ///     Gets the display name or Unicode representation of this emoji.
        /// </summary>
        /// <returns>
        ///     A string representing the display name or the Unicode representation (e.g. <c>🤔</c>) of this emoji.
        /// </returns>
        string Name { get; }
    }
}
