using Discord.SlashCommands.Builders;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Base class for any Slash command handling modules
    /// </summary>
    /// <typeparam name="T">Type of slash command context to be injected into the module</typeparam>
    public abstract class SlashModuleBase<T> : ISlashModuleBase where T : class, ISlashCommandContext
    {
        /// <summary>
        /// Command execution context for an user interaction.
        /// </summary>
        public T Context { get; private set; }

        /// <summary>
        /// Method body to be executed after an application command execution
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command</param>
        public virtual void AfterExecute (ExecutableInfo command) { }

        /// <summary>
        /// Method body to be executed before executing an application command
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command</param>
        public virtual void BeforeExecute (ExecutableInfo command) { }

        /// <summary>
        /// Method body to be executed before the derived module is builded
        /// </summary>
        /// <param name="commandService">Command service the derived module belongs to</param>
        /// <param name="builder">Module builder responsible of building the derived type</param>
        public virtual void OnModuleBuilding (SlashCommandService commandService, ModuleInfo module) { }
        public virtual void SetContext (ISlashCommandContext context)
        {
            var newValue = context as T;
            Context = newValue ?? throw new InvalidOperationException($"Invalid context type. Expected {typeof(T).Name}, got {context.GetType().Name}.");
        }

        protected virtual async Task RespondAsync (string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) =>
            await Context.Interaction.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, component, embed).ConfigureAwait(false);

        protected virtual async Task FolloupAsync (string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) =>
            await Context.Interaction.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, component, embed).ConfigureAwait(false);

        protected virtual async Task ReplyAsync (string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null,
            AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent component = null) =>
            await Context.Channel.SendMessageAsync(text, false, embed, options, allowedMentions, messageReference, component).ConfigureAwait(false);

        protected virtual async Task DeleteOriginalResponseAsync ( )
        {
            var response = await Context.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
            await response.DeleteAsync().ConfigureAwait(false);
        }

        protected virtual async Task<SocketInteraction> WaitNextAsync (TimeSpan timeout, Predicate<SocketInteraction> predicate)
        {
            if (!( Context.Client is BaseSocketClient baseSocketClient ))
                throw new InvalidOperationException("Provided client type is not supported");

            return await InteractionUtility.WaitForInteraction(baseSocketClient, timeout, predicate).ConfigureAwait(false);
        }
    }
}
