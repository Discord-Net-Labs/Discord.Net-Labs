using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord
{
    /// <summary>
    /// Represents a container for storing a <see cref="MessageSelectMenuComponent"/> or up to 5 <see cref="MessageButtonComponent"/>
    /// </summary>
    public class MessageActionRowComponent : MessageComponent
    {
        /// <summary>
        /// Get the collection of components that are stored in this row
        /// </summary>
        public IReadOnlyList<MessageComponent> MessageComponents { get; }

        internal MessageActionRowComponent ( IEnumerable<MessageComponent> childComponents ) : base(MessageComponentType.ActionRow)
        {
            MessageComponents = childComponents.ToImmutableArray();
        }
    }
}
