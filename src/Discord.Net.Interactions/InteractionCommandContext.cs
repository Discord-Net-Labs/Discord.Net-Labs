using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <inheritdoc cref="IInteractionCommandContext"/>
    public class InteractionCommandContext : IInteractionCommandContext
    {
        /// <inheritdoc/>
        public IDiscordClient Client { get; }
        /// <inheritdoc/>
        public IGuild Guild { get; }
        /// <inheritdoc/>
        public IMessageChannel Channel { get; }
        /// <inheritdoc/>
        public IUser User { get; }
        /// <inheritdoc/>
        public IDiscordInteraction Interaction { get; }

        public InteractionCommandContext(IDiscordClient client, IDiscordInteraction interaction, IUser user, IMessageChannel channel = null)
        {
            Client = client;
            Interaction = interaction;

            Channel = channel;
            Guild = (interaction as IGuildUser)?.Guild;
            User = user;
            Interaction = interaction;
        }
    }
}
