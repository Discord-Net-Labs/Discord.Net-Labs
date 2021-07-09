using System.Collections.Generic;
using System.Linq;
using Model = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    /// Represents a Web-Socket based <see cref="IDiscordInteraction"/> that is created by a <see cref="IMessageComponent"/> interaction
    /// </summary>
    public class SocketMessageInteraction : SocketInteraction
    {
        /// <summary>
        /// The message that encapsulates the Interaction Event invoker
        /// </summary>
        public SocketMessage Message { get; private set; }
        /// <summary>
        /// Type of the component that crated this Interaction
        /// </summary>
        public MessageComponentType ComponentType { get; private set; }
        /// <summary>
        /// Dev-assigned Custom ID of the Message Component that created this Interaction
        /// </summary>
        public string CustomId { get; private set; }
        /// <summary>
        /// If the type of the component is <see cref="MessageComponentType.SelectMenu"/>, the values that are selected by the user
        /// </summary>
        public IEnumerable<string> Values { get; private set; }

        internal SocketMessageInteraction (DiscordSocketClient discord, ClientState state, SocketUser user, ISocketMessageChannel channel, Model model)
            : base(discord, state, user, channel, model)
        {
            Message = model.Message.IsSpecified ? Message : null;
            Update(state, model);
        }

        internal override void Update (ClientState state, Model model)
        {
            if (model.Data.IsSpecified)
            {
                var data = model.Data.Value;

                ComponentType = data.ComponentType;
                CustomId = data.CustomId;

                if (data.ComponentType == MessageComponentType.SelectMenu)
                    Values = data.Values.GetValueOrDefault(null)?.ToList();
            }
        }
    }
}
