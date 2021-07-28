namespace Discord.SlashCommands
{
    /// <inheritdoc cref="ISlashCommandContext"/>
    public class SlashCommandContext : ISlashCommandContext
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
        /// Wheter the origin channel of the Interaction is a private channel
        /// </summary>
        public bool IsPrivate => Channel is IPrivateChannel;

        public InteractionType InvokerType => Interaction.Type;

        public SlashCommandContext (IDiscordClient client, IMessageChannel channel, IUser user, IDiscordInteraction interaction)
        {
            Client = client;
            Channel = channel;
            Guild = ( channel as IGuildChannel )?.Guild;
            User = user;
            Interaction = interaction;
        }
    }
}
