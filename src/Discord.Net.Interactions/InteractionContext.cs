namespace Discord.Interactions
{
    /// <inheritdoc cref="IInteractionContext"/>
    public class InteractionContext : IInteractionContext
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

        public InteractionContext(IDiscordClient client, IDiscordInteraction interaction, IUser user, IMessageChannel channel = null)
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
