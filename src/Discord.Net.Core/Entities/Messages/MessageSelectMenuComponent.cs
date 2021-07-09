using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// Represents a Dropdown menu object that can be sent with a message
    /// </summary>
    public class MessageSelectMenuComponent : MessageComponent, IDiscordInteractable
    {
        /// <inheritdoc/>
        public string CustomId { get; }

        /// <summary>
        /// The displayed placeholder when nothing is selected
        /// </summary>
        public string Placeholder { get; }

        /// <summary>
        /// The minimum number of items that must be chosen
        /// </summary>
        public int MinValues { get; }

        /// <summary>
        /// The maximum number of items that must be chosen
        /// </summary>
        public int MaxValues { get; }

        /// <summary>
        /// Selectable items of this dropdown menu
        /// </summary>
        public IReadOnlyList<SelectOption> Options { get; }

        internal MessageSelectMenuComponent (string customid, IEnumerable<SelectOption> options, string placeholder = null, int min = 1, int max = 1 )
            : base(MessageComponentType.SelectMenu)
        {
            CustomId = customid;
            Placeholder = placeholder;
            Options = options.ToImmutableArray();
            Placeholder = placeholder;
            MinValues = min;
            MaxValues = max;
        }

        internal MessageSelectMenuComponent(SelectMenuBuilder builder) : base(MessageComponentType.SelectMenu)
        {
            CustomId = builder.CustomId;
            Placeholder = builder.Placeholder;
            Options = builder.Options.ToImmutableArray();
            MinValues = builder.MinValues;
            MaxValues = builder.MaxValues;
        }
    }

    /// <summary>
    /// Represents a selectable item of a <see cref="MessageSelectMenuComponent"/>
    /// </summary>
    public class SelectOption
    {
        /// <summary>
        /// Label of the option
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// The value that will be sent back by Discord whenever this option is selected
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Description of this option
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Emoji that will be displayed next to this options name
        /// </summary>
        public Emote Emoji { get; }

        /// <summary>
        /// Wheter this option appears selected by default
        /// </summary>
        public bool IsDefault { get; }

        /// <summary>
        /// Initializes a new <see cref="SelectOption"/>
        /// </summary>
        /// <param name="label">Label of the option</param>
        /// <param name="value">The value that will be sent back by Discord whenever this option is selected</param>
        /// <param name="description">Description of this option</param>
        /// <param name="emoji">Emoji that will be displayed next to this options name</param>
        /// <param name="isDefault">Wheter this option appears selected by default</param>
        public SelectOption (string label, string value, string description = null, Emote emoji = null, bool isDefault = false)
        {
            Label = label;
            Value = value;
            Description = description;
            Emoji = emoji;
            IsDefault = isDefault;
        }
    }
}
