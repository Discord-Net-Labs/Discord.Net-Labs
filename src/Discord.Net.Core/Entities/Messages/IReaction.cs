namespace Discord
{
    /// <summary>
    ///     Represents a generic reaction object.
    /// </summary>
    public interface IReaction
    {
        /// <summary>
        ///     The <see cref="IEmoji" /> used in the reaction.
        /// </summary>
        IEmoji Emoji { get; }
    }
}
