using Model = Discord.API.Reaction;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST reaction object.
    /// </summary>
    public class RestReaction : IReaction
    {
        /// <inheritdoc />
        public IEmoji Emoji { get; }
        /// <summary>
        ///     Gets the number of reactions added.
        /// </summary>
        public int Count { get; }
        /// <summary>
        ///     Gets whether the reactions is added by the user.
        /// </summary>
        public bool Me { get; }

        internal RestReaction(IEmoji emoji, int count, bool me)
        {
            Emoji = emoji;
            Count = count;
            Me = me;
        }
        internal static RestReaction Create(Model model)
        {
            IEmoji emoji;
            if (model.Emoji.Id.HasValue)
                emoji = new CustomEmoji(model.Emoji.Id.Value, model.Emoji.Name, model.Emoji.Animated.GetValueOrDefault());
            else
                emoji = new Emoji(model.Emoji.Name);
            return new RestReaction(emoji, model.Count, model.Me);
        }
    }
}
