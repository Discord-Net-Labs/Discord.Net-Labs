namespace Discord.Interactions
{
    public class TextInputComponentInfo : InputComponentInfo
    {
        /// <summary>
        ///     Gets the style of the text input.
        /// </summary>
        public TextInputStyle Style { get; }

        /// <summary>
        ///     Gets the placeholder of the text input.
        /// </summary>
        public string Placeholder { get; }

        /// <summary>
        ///     Gets the minimum length of the text input.
        /// </summary>
        public int MinLength { get; }

        /// <summary>
        ///     Gets the maximum length of the text input.
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        ///     Gets or sets the default value of the text input.
        /// </summary>
        public string DefaultValue { get; }

        internal TextInputComponentInfo(Builders.TextInputComponentBuilder builder, ModalInfo modal) : base(builder, modal)
        {
            Style = builder.Style;
            Placeholder = builder.Placeholder;
            MinLength = builder.MinLength;
            MaxLength = builder.MaxLength;
            DefaultValue = builder.DefaultValue;
        }
    }
}
