namespace Discord
{
    /// <summary>
    ///     A Unicode emoji.
    /// </summary>
    public class Emoji : IEmoji
    {
        /// <inheritdoc />
        public string Name { get; }
        /// <summary>
        ///     Gets the Unicode representation of this emoji.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="Emoji.Name"/>.
        /// </returns>
        public override string ToString() => Name;

        /// <summary>
        ///     Initializes a new <see cref="Emoji"/> class with the provided Unicode.
        /// </summary>
        /// <param name="unicode">The pure UTF-8 encoding of an emoji.</param>
        internal Emoji(string unicode)
        {
            Name = unicode;
        }

        /// <summary>
        ///     Determines whether the specified emoji is equal to the current one.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (other == this)
                return true;

            return other is Emoji otherEmoji && string.Equals(Name, otherEmoji.Name);
        }

        /// <summary> Tries to parse an <see cref="Emoji"/> from its raw format. </summary>
        /// <param name="text">The raw encoding of an emoji. For example: <code>:heart: or ❤</code></param>
        /// <param name="result">An emoji.</param>
        public static bool TryParse(string text, out Emoji result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (EmojiUtils.NameEmojis.ContainsKey(text))
                result = new Emoji(EmojiUtils.NameEmojis[text]);

            if (EmojiUtils.UnicodeEmojis.ContainsKey(text))
                result = new Emoji(text);

            return result != null;
        }

        /// <inheritdoc />
        public override int GetHashCode() => Name.GetHashCode();
    }
}
