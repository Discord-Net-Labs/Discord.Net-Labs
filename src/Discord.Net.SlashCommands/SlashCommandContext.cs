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

        public SlashCommandContext (IDiscordClient client, IDiscordInteraction interaction)
        {
            Client = client;
            Channel = interaction.Channel;
            Guild = ( interaction.Channel as IGuildChannel )?.Guild;
            User = interaction.User;
            Interaction = interaction;
        }
    }
}
