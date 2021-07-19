using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify an <see cref="CustomEmoji" /> with the specified changes.
    /// </summary>
    /// <seealso cref="IGuild.ModifyEmojiAsync"/>
    public class EmojiProperties
    {
        /// <summary>
        ///     Gets or sets the name of the <see cref="CustomEmoji"/>.
        /// </summary>
        public Optional<string> Name { get; set; }
        /// <summary>
        ///     Gets or sets the roles that can access this <see cref="CustomEmoji"/>.
        /// </summary>
        public Optional<IEnumerable<IRole>> Roles { get; set; }
    }
}
