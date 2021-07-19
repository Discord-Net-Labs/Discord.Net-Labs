using System;
using System.Globalization;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A custom image-based emoji.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class CustomEmoji : IEmoji, ISnowflakeEntity
    {
        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public ulong Id { get; }
        /// <summary>
        ///     Gets whether this emoji is animated.
        /// </summary>
        /// <returns>
        ///     A boolean that determines whether or not this emoji is an animated one.
        /// </returns>
        public bool Animated { get; }
        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <summary>
        ///     Gets the image URL of this emoji.
        /// </summary>
        /// <returns>
        ///     A string that points to the URL of this emoji.
        /// </returns>
        public string Url => CDN.GetEmojiUrl(Id, Animated);

        internal CustomEmoji(ulong id, string name, bool animated)
        {
            Id = id;
            Name = name;
            Animated = animated;
        }

        /// <summary>
        ///     Determines whether the specified emoji is equal to the current emoji.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other == this) return true;

            var otherEmote = other as CustomEmoji;
            if (otherEmote == null) return false;

            return Id == otherEmote.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
            => Id.GetHashCode();

        /// <summary> Parses an <see cref="CustomEmoji"/> from its raw format. </summary>
        /// <param name="text">The raw encoding of an emoji (e.g. <c>&lt;:dab:277855270321782784&gt;</c>).</param>
        /// <returns>An emoji.</returns>
        /// <exception cref="ArgumentException">Invalid emoji format.</exception>
        public static CustomEmoji Parse(string text)
        {
            if (TryParse(text, out CustomEmoji result))
                return result;
            throw new ArgumentException(message: "Invalid emoji format.", paramName: nameof(text));
        }

        /// <summary> Tries to parse an <see cref="CustomEmoji"/> from its raw format. </summary>
        /// <param name="text">The raw encoding of an emoji; for example, &lt;:dab:277855270321782784&gt;.</param>
        /// <param name="result">An emoji.</param>
        public static bool TryParse(string text, out CustomEmoji result)
        {
            result = null;
            if (text.Length >= 4 && text[0] == '<' && (text[1] == ':' || (text[1] == 'a' && text[2] == ':')) && text[text.Length - 1] == '>')
            {
                bool animated = text[1] == 'a';
                int startIndex = animated ? 3 : 2;

                int splitIndex = text.IndexOf(':', startIndex);
                if (splitIndex == -1)
                    return false;

                if (!ulong.TryParse(text.Substring(splitIndex + 1, text.Length - splitIndex - 2), NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
                    return false;

                string name = text.Substring(startIndex, splitIndex - startIndex);
                result = new CustomEmoji(id, name, animated);
                return true;
            }
            return false;

        }

        private string DebuggerDisplay => $"{Name} ({Id})";
        /// <summary>
        ///     Returns the raw representation of the emoji.
        /// </summary>
        /// <returns>
        ///     A string representing the raw presentation of the emoji (e.g. <c>&lt;:thonkang:282745590985523200&gt;</c>).
        /// </returns>
        public override string ToString() => $"<{(Animated ? "a" : "")}:{Name}:{Id}>";
    }
}
