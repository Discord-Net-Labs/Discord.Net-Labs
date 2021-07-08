namespace Discord
{
    /// <summary>
    /// Represents an Interaction Manipulation Token
    /// </summary>
    public interface IDiscordInteractionToken
    {
        /// <summary>
        /// The value of the token
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Wheter the token is expired
        /// </summary>
        bool IsValid { get; }
    }
}
