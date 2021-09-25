using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Provides a base class for a command module to inherit from.
    /// </summary>
    /// <typeparam name="T">Type of slash command context to be injected into the module</typeparam>
    public abstract class InteractionModuleBase<T> : IInteractionModuleBase where T : class, IInteractionCommandContext
    {
        /// <summary>
        ///     The underlying context of the command.
        /// </summary>
        public T Context { get; private set; }

        /// <inheritdoc/>
        public virtual void AfterExecute (ICommandInfo command) { }

        /// <inheritdoc/>
        public virtual void BeforeExecute (ICommandInfo command) { }

        /// <inheritdoc/>
        public virtual void OnModuleBuilding (InteractionService commandService, ModuleInfo module) { }

        public void SetContext (IInteractionCommandContext context)
        {
            var newValue = context as T;
            Context = newValue ?? throw new InvalidOperationException($"Invalid context type. Expected {typeof(T).Name}, got {context.GetType().Name}.");
        }

        /// <inheritdoc cref="IDiscordInteraction.RespondAsync(string, Embed[], bool, bool, AllowedMentions, RequestOptions, MessageComponent, Embed)"/>
        protected virtual async Task RespondAsync (string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) =>
            await Context.Interaction.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, component, embed).ConfigureAwait(false);

        /// <inheritdoc cref="IDiscordInteraction.FollowupAsync(string, Embed[], bool, bool, AllowedMentions, RequestOptions, MessageComponent, Embed)"/>
        protected virtual async Task<IUserMessage> FollowupAsync (string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) =>
            await Context.Interaction.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, component, embed).ConfigureAwait(false);

        /// <inheritdoc cref="IMessageChannel.SendMessageAsync(string, bool, Embed, RequestOptions, AllowedMentions, MessageReference, MessageComponent)"/>
        protected virtual async Task<IUserMessage> ReplyAsync (string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null,
            AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent component = null) =>
            await Context.Channel.SendMessageAsync(text, false, embed, options, allowedMentions, messageReference, component).ConfigureAwait(false);

        /// <inheritdoc cref="IDeletable.DeleteAsync(RequestOptions)"/>
        protected virtual async Task DeleteOriginalResponseAsync ( )
        {
            var response = await Context.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
            await response.DeleteAsync().ConfigureAwait(false);
        }
    }
}
