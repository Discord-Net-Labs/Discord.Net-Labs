using System;

namespace Discord
{
    /// <inheritdoc/>
    public class InteractionToken : IDiscordInteractionToken
    {
        private TimeSpan TTL => TimeSpan.FromMinutes(15);

        /// <inheritdoc/>
        public string Token { get; }

        /// <summary>
        /// Time of creation of this token
        /// </summary>
        public DateTimeOffset CreatedAt { get; }

        /// <inheritdoc/>
        public bool IsValid => CreatedAt + TTL > DateTimeOffset.Now;

        /// <summary>
        /// Initializes a new <see cref="InteractionToken"/>
        /// </summary>
        /// <param name="token">Tokens value</param>
        /// <param name="snowflake">The snowflake of the entity this token was sent with.This is used to determine the <see cref="CreatedAt"/></param>
        internal InteractionToken (string token, ulong snowflake )
        {
            Token = token;
            CreatedAt = SnowflakeUtils.FromSnowflake(snowflake);
        }

        public override string ToString ( ) => Token;
    }
}
