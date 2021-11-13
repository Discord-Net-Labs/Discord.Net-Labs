using Discord.WebSocket;
using System;

namespace Discord.Interactions
{
    /// <summary>
    /// Represents a Web-Socket based context of an Application Command
    /// </summary>
    public class SocketInteractionContext<TInteraction> : IInteractionContext
        where TInteraction : SocketInteraction
    {
        /// <summary>
        /// Get the <see cref="DiscordSocketClient"/> that the command will be executed with
        /// </summary>
        public DiscordSocketClient Client { get; }

        /// <summary>
        /// Get the <see cref="SocketGuild"/> the command originated from
        /// </summary>
        /// <remarks>
        /// Will be null if the command is from a DM Channel
        /// </remarks>
        public SocketGuild Guild { get; }

        /// <summary>
        /// Get the <see cref="ISocketMessageChannel"/> the command originated from
        /// </summary>
        public ISocketMessageChannel Channel { get; }

        /// <summary>
        /// Get the <see cref="SocketUser"/> who executed the command
        /// </summary>
        public SocketUser User { get; }

        /// <summary>
        /// Get the <see cref="SocketInteraction"/> the command was recieved with
        /// </summary>
        public TInteraction Interaction { get; }

        /// <inheritdoc/>
        IDiscordClient IInteractionContext.Client => Client;

        /// <inheritdoc/>
        IGuild IInteractionContext.Guild => Guild;

        /// <inheritdoc/>
        IMessageChannel IInteractionContext.Channel => Channel;

        /// <inheritdoc/>
        IUser IInteractionContext.User => User;

        /// <inheritdoc/>
        IDiscordInteraction IInteractionContext.Interaction => Interaction;

        /// <summary>
        ///     Initializes a new <see cref="InteractionContext"/> 
        /// </summary>
        /// <param name="client">The underlying client</param>
        /// <param name="interaction">The underlying interaction</param>
        public SocketInteractionContext (DiscordSocketClient client, TInteraction interaction)
        {
            Client = client;
            Channel = interaction.Channel;
            Guild = ( interaction.User as SocketGuildUser )?.Guild;
            User = interaction.User;
            Interaction = interaction;
        }
    }

    public class SocketInteractionContext : SocketInteractionContext<SocketInteraction>
    {
        public SocketInteractionContext (DiscordSocketClient client, SocketInteraction interaction) : base(client, interaction) { }
    }
}
