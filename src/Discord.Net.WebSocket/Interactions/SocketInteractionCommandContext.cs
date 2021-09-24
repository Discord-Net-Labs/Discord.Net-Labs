using Discord.WebSocket;

namespace Discord.Interactions
{
    /// <summary>
    /// Represents a Web-Socket based context of an Application Command
    /// </summary>
    public class SocketInteractionCommandContext : IInteractionCommandContext
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
        public SocketInteraction Interaction { get; }

        /// <inheritdoc/>
        IDiscordClient IInteractionCommandContext.Client => Client;

        /// <inheritdoc/>
        IGuild IInteractionCommandContext.Guild => Guild;

        /// <inheritdoc/>
        IMessageChannel IInteractionCommandContext.Channel => Channel;

        /// <inheritdoc/>
        IUser IInteractionCommandContext.User => User;

        /// <inheritdoc/>
        IDiscordInteraction IInteractionCommandContext.Interaction => Interaction;

        /// <summary>
        /// Initializes a new <see cref="SocketInteractionCommandContext"/> 
        /// </summary>
        /// <param name="client">The underlying client</param>
        /// <param name="interaction">The underlying interaction</param>
        public SocketInteractionCommandContext (DiscordSocketClient client, SocketInteraction interaction)
        {
            Client = client;
            Channel = interaction.Channel;
            Guild = ( interaction.User as SocketGuildUser )?.Guild;
            User = interaction.User;
            Interaction = interaction;
        }
    }
}
