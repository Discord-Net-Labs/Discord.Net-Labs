namespace Discord.Interactions
{
    /// <inheritdoc cref="IInteractionCommandContext"/>
    public class InteractionCommandContext : IInteractionCommandContext
    {
        /// <inheritdoc/>
        public virtual IDiscordClient Client { get; }

        /// <inheritdoc/>
        public virtual IGuild Guild { get; }

        /// <inheritdoc/>
        public virtual IMessageChannel Channel { get; }

        /// <inheritdoc/>
        public virtual IUser User { get; }

        /// <inheritdoc/>
        public virtual IDiscordInteraction Interaction { get; }

        /// <summary>
        ///     Indicates whether the channel that the command is executed in is a private channel.
        /// </summary>
        public bool IsPrivate => Channel is IPrivateChannel;

        public InteractionType InteractionType => Interaction.Type;

        public InteractionCommandContext (IDiscordClient client, IMessageChannel channel, IUser user, IDiscordInteraction interaction)
        {
            Client = client;
            Channel = channel;
            Guild = ( channel as IGuildChannel )?.Guild;
            User = user;
            Interaction = interaction;
        }
    }
}
