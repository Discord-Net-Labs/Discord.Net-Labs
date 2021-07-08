using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    /// Represents a Web-Socket based Discord Interaction
    /// </summary>
    public abstract class SocketInteraction : SocketEntity<ulong>, IDiscordInteraction

    {
        /// <inheritdoc cref="IDiscordInteraction.User"/>
        public SocketUser User { get; }
        /// <inheritdoc/>
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc/>
        public InteractionType InteractionType { get; }
        /// <inheritdoc cref="IDiscordInteraction.Channel"/>
        public ISocketMessageChannel Channel { get; }
        /// <inheritdoc/>
        public int Version { get; }
        /// <inheritdoc cref="IDiscordInteraction.Token"/>
        public InteractionToken Token { get; }
        /// <inheritdoc cref="IDiscordInteraction.ApplicationId"/>
        internal ulong ApplicationId { get; }

        /// <inheritdoc/>
        IUser IDiscordInteraction.User => User;
        /// <inheritdoc/>
        IDiscordInteractionToken IDiscordInteraction.Token => Token;
        /// <inheritdoc/>
        IMessageChannel IDiscordInteraction.Channel => Channel;
        /// <inheritdoc/>
        ulong IDiscordInteraction.ApplicationId => ApplicationId;

        internal SocketInteraction (DiscordSocketClient discord, ClientState state, SocketUser user, ISocketMessageChannel channel, Model model)
            : base(discord, model.Id)
        {
            User = user;
            InteractionType = model.Type;
            Version = model.Version;
            Channel = channel;
            Token = new InteractionToken(model.Token, model.Id);
            ApplicationId = model.ApplicationId;
        }

        internal static SocketInteraction Create (DiscordSocketClient discord, ClientState state, SocketUser user, ISocketMessageChannel channel, Model model)
        {
            if (model.Type == InteractionType.ApplicationCommand)
                return new SocketCommandInteraction(discord, state, user, channel, model);
            else if (model.Type == InteractionType.MessageComponent)
                return new SocketMessageInteraction(discord, state, user, channel, model);
            else
                throw new ArgumentException("This kind of interaction is not supported.");
        }

        internal abstract void Update (ClientState state, Model model);

        /// <inheritdoc/>
        public async Task AcknowledgeAsync (RequestOptions options = null) =>
            await SlashCommandHelper.SendAcknowledgement(Discord, this, options).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task PopulateAcknowledgement (string text = null, bool isTTS = false, IEnumerable<Embed> embeds = null, AllowedMentions allowedMentions = null,
            InteractionApplicationCommandCallbackFlags flags = 0, IEnumerable<MessageComponent> messageComponents = null, RequestOptions options = null) =>
            await SlashCommandHelper.ModifyInteractionResponse(Discord, this, text, allowedMentions, embeds, messageComponents, options).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task SendResponse (string text = null, bool isTTS = false, IEnumerable<Embed> embeds = null, AllowedMentions allowedMentions = null,
            InteractionApplicationCommandCallbackFlags flags = 0, IEnumerable<MessageComponent> messageComponents = null, RequestOptions options = null) =>
            await SlashCommandHelper.SendInteractionResponse(Discord, this, text, isTTS, embeds, allowedMentions, messageComponents, flags, options).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task DeleteAsync (RequestOptions options = null) =>
            await SlashCommandHelper.DeleteInteractionResponse(Discord, this, null).ConfigureAwait(false);

        /// <inheritdoc cref="IDiscordInteraction.SendFollowupAsync(string, bool, string, string, IEnumerable{Embed}, AllowedMentions, RequestOptions)"/>
        public async Task<RestMessage> SendFollowupAsync (string text = null, bool isTTS = false, string username = null, string avatarUrl = null, IEnumerable<Embed> embeds = null,
            AllowedMentions allowedMentions = null, RequestOptions options = null) =>
            await SlashCommandHelper.SendInteractionFollowup(Discord, this, text, isTTS, embeds, username, avatarUrl, allowedMentions, options).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task ModifyFollowup (ulong messageId, string text = null, IEnumerable<Embed> embeds = null, AllowedMentions allowedMentions = null,
            RequestOptions options = null) =>
            await SlashCommandHelper.ModifyFollowupMessage(Discord, this, messageId, text, allowedMentions, embeds, options).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task DeleteFollowup (ulong messageId, RequestOptions options = null) =>
            await SlashCommandHelper.DeleteFollowupMessage(Discord, this, messageId, options).ConfigureAwait(false);

        /// <inheritdoc/>
        async Task<IMessage> IDiscordInteraction.SendFollowupAsync (string text, bool isTTS, string username, string avatarUrl,
            IEnumerable<Embed> embeds, AllowedMentions allowedMentions, RequestOptions options) =>
            await SendFollowupAsync(text, isTTS, username, avatarUrl, embeds, allowedMentions, options);
    }
}
