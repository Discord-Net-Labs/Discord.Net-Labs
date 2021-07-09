using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
    public class MessageUIBuilder

    {
        private List<List<IMessageComponent>> _rows = new List<List<IMessageComponent>>();
        public IReadOnlyList<IReadOnlyList<IMessageComponent>> Rows => _rows;

        public MessageUIBuilder ( ) { }

        public MessageUIBuilder AddRow (params IMessageComponent[] components)
        {
            CheckRows(components);

            _rows.Add(components.ToList());
            return this;
        }

        public MessageUIBuilder ModifyRow (int index, IEnumerable<IMessageComponent> components)
        {
            if (index > 5 || index > _rows.Count - 1 || index < 0)
                throw new InvalidOperationException("Index is out of bounds");

            CheckRows(components);

            _rows[index] = components.ToList();
            return this;
        }

        private void CheckRows (IEnumerable<IMessageComponent> components)
        {
            if (_rows.Count >= 5)
                throw new InvalidOperationException("A messageUI cannot have more than 5 rows");

            if (components.Count() > 1 && components.Any(x => x is MessageSelectMenuComponent))
                throw new InvalidOperationException("A row containing a select menu cannot also contain buttons.");

            if (components.Count() > 5)
                throw new InvalidOperationException("A row cannot contain more than 5 buttons.");
        }

        public MessageUIBuilder RemoveRow (int index)
        {
            if (index > 5 || index > _rows.Count - 1 || index < 0)
                throw new InvalidOperationException("Index is out of bounds (Max row count can be 5).");

            _rows.RemoveAt(index);
            return this;
        }

        public IReadOnlyList<MessageActionRowComponent> Build ( )
        {
            var result = new List<MessageActionRowComponent>();
            foreach (var row in Rows)
                result.Add(new MessageActionRowComponent(row.Cast<MessageComponent>()));
            return result;
        }
    }

    public class SelectMenuBuilder
    {
        private readonly List<SelectOption> _options = new List<SelectOption>();
        public string CustomId { get; private set; }
        public string Placeholder { get; set; }
        public int MinValues { get; set; } = 1;
        public int MaxValues { get; set; } = 1;
        public IReadOnlyList<SelectOption> Options => _options;

        public SelectMenuBuilder (string customId)
        {
            if (customId.Length > 100)
                throw new ArgumentException("CustomId length must be shorter then 100 characters.");

            CustomId = customId;
        }

        public SelectMenuBuilder WithCustomId (string id)
        {
            CustomId = id;
            return this;
        }

        public SelectMenuBuilder WithPlaceholder (string placeholder)
        {
            if (placeholder.Length > 100)
                throw new ArgumentException("Placeholder length must be shorter then 100 characters.");

            Placeholder = placeholder;
            return this;
        }

        public SelectMenuBuilder WithMinValue (int min)
        {
            if (min < 0 || min > 25)
                throw new ArgumentException("Min value bounds must be withing 0 and 25.");

            MinValues = min;
            return this;
        }

        public SelectMenuBuilder WithMaxValue (int max)
        {
            if (max < 1 || max > 25)
                throw new ArgumentException("Max value bounds must be withing 1 and 25.");

            MaxValues = max;
            return this;
        }

        public SelectMenuBuilder AddOption (SelectOption option)
        {
            if (_options.Count >= 25)
                throw new InvalidOperationException("Number of choices cannot be higher than 25.");

            _options.Add(option);
            return this;
        }

        public SelectMenuBuilder RemoveOptionAt (int index)
        {
            if (index < 0 || index > _options.Count)
                throw new InvalidOperationException("Index out of bounds");

            _options.RemoveAt(index);
            return this;
        }

        public MessageSelectMenuComponent Build ( ) =>
            new MessageSelectMenuComponent(this);
    }

    public class ButtonBuilder
    {
        public string Label { get; set; }
        public string CustomId { get; private set; }
        public Emote Emoji { get; set; }
        public ButtonStyles Style { get; set; }
        public string Url { get; set; }
        public bool IsDisabled { get; set; }

        public ButtonBuilder (string customId, ButtonStyles style)
        {
            CustomId = customId;
            Style = style;
        }

        public ButtonBuilder WithLabel (string label)
        {
            if (label.Length > 80)
                throw new ArgumentException("Label length must be shorter than 80 characters.");

            Label = label;
            return this;
        }

        public ButtonBuilder WithCustomId (string id)
        {
            if (id.Length > 100)
                throw new ArgumentException("CustomId length must be shorter then 100 characters.");

            CustomId = id;
            Url = null;
            return this;
        }

        public ButtonBuilder WithEmoji (Emote emote)
        {
            Emoji = emote;
            return this;
        }

        public ButtonBuilder WithUrl (string url)
        {
            Url = url;
            CustomId = null;
            return this;
        }

        public ButtonBuilder SetDisabled (bool isDisabled)
        {
            IsDisabled = isDisabled;
            return this;
        }

        public ButtonBuilder WithStyle (ButtonStyles style)
        {
            Style = style;
            return this;
        }

        public MessageButtonComponent Build ( ) =>
            new MessageButtonComponent(this);
    }
}
