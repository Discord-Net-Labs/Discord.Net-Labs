using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord
{
    /// <summary>
    /// Represents the response for a <see cref="IDiscordInteraction"/>
    /// </summary>
    public class InteractionResponse
    {
        /// <summary>
        /// Wheter the response body is Text to Speech
        /// </summary>
        public bool IsTTS { get; internal set; }

        /// <summary>
        /// Text content of the response body
        /// </summary>
        public string Content { get; internal set; }

        /// <summary>
        /// Type of this response
        /// </summary>
        public InteractionCallbackType CallbackType { get; internal set; }

        /// <summary>
        /// Embeds that are present in the response body
        /// </summary>
        public IReadOnlyCollection<Embed> Embeds { get; internal set; }

        /// <summary>
        /// Get the allowed mentions in this response
        /// </summary>
        public AllowedMentions AllowedMentions { get; internal set; }

        /// <summary>
        /// Get the collection of <see cref="MessageComponent"/>s sent with this response
        /// </summary>
        public IReadOnlyCollection<MessageComponent> MessageComponents { get; internal set; }

        internal InteractionResponse (bool isTTs, string content, InteractionCallbackType type, IEnumerable<Embed> embeds,
            AllowedMentions allowedMentions, IEnumerable<MessageComponent> messageComponents)
        {
            IsTTS = isTTs;
            Content = content;
            CallbackType = type;
            Embeds = embeds.ToImmutableArray();
            AllowedMentions = allowedMentions;
            MessageComponents = messageComponents.ToImmutableArray();
        }

        internal InteractionResponse (InteractionCallbackType type)
        {
            CallbackType = type;
        }
    }
}
