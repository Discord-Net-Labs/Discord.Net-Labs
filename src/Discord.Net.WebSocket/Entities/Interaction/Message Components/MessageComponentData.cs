using System.Collections.Generic;
using Model = Discord.API.MessageComponentInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data sent with a <see cref="InteractionType.MessageComponent"/>.
    /// </summary>
    public class MessageComponentData
    {
        /// <summary>
        ///     The components Custom Id that was clicked
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     The type of the component clicked
        /// </summary>
        public ComponentType Type { get; }

        /// <summary>
        ///     The value(s) of a <see cref="SelectMenu"/> interaction response.
        /// </summary>
        public IReadOnlyCollection<string> Values { get; }

        internal MessageComponentData (Model model)
        {
            this.CustomId = model.CustomId;
            this.Type = model.ComponentType;
            this.Values = model.Values.GetValueOrDefault();
        }
    }
}
