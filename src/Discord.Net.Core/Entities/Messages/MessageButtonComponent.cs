using System;

namespace Discord
{
    /// <summary>
    /// Represents a UI button that can be sent with a message
    /// </summary>
    public class MessageButtonComponent : MessageComponent, IDiscordInteractable
    {
        /// <summary>
        /// Style of this button
        /// </summary>
        public ButtonStyles Style { get; }

        /// <summary>
        /// Label of this button
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Emoji that will be displayed next to the label
        /// </summary>
        public Emote? Emoji { get; }

        /// <inheritdoc/>
        /// <remarks>
        /// <see cref="Url"/> and <see cref="CustomId"/> are mutually exclusive but at least one of them must be set
        /// </remarks>
        public string CustomId { get; }

        /// <summary>
        /// The url for link-style buttons
        /// </summary>
        /// <remarks>
        /// <see cref="Url"/> and <see cref="CustomId"/> are mutually exclusive but at least one of them must be set.
        /// If the Url is set, no Interaction event will be raised whenever this button is clicked
        /// </remarks>
        public string Url { get; }

        /// <summary>
        /// Wheter this button is disabled or not
        /// </summary>
        public bool IsDisabled { get; } = false;

        /// <summary>
        /// Wheter this button is a hyperlink
        /// </summary>
        public bool IsLinkButton => !string.IsNullOrEmpty(Url);

        internal MessageButtonComponent (string label = null, string customId = null, string url = null,
            Emote emoji = null, ButtonStyles style = ButtonStyles.Primary, bool isDisabled = false) : base(MessageComponentType.Button)
        {
            if (string.IsNullOrEmpty(label) && string.IsNullOrEmpty(customId))
                throw new ArgumentException($"Either one of {nameof(label)} or {nameof(customId)} must be set.");

            Label = label;
            CustomId = customId;
            Url = url;
            Emoji = emoji;
            Style = style;
            IsDisabled = isDisabled;
        }

        internal MessageButtonComponent (ButtonBuilder builder) : base(MessageComponentType.Button)
        {
            if (string.IsNullOrEmpty(builder.Label) && string.IsNullOrEmpty(builder.CustomId))
                throw new ArgumentException($"Either one of {nameof(builder.Label)} or {nameof(builder.CustomId)} must be set.");

            Label = builder.Label;
            CustomId = builder.CustomId;
            Url = builder.Url;
            Emoji = builder.Emoji;
            IsDisabled = builder.IsDisabled;
            Style = builder.Style;
        }

        public override string ToString ( ) => Label;
    }
}
