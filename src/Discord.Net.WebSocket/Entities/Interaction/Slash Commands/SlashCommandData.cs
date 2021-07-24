using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data tied with the <see cref="SocketSlashCommand"/> interaction.
    /// </summary>
    public class SlashCommandData : IApplicationCommandInteractionData
    {
        /// <inheritdoc/>
        public ulong CommandId { get; private set; }
        /// <inheritdoc/>
        public string CommandName { get; private set; }

        /// <summary>
        ///     The <see cref="SocketSlashCommandDataOption"/>'s received with this interaction.
        /// </summary>
        public IReadOnlyCollection<SlashCommandDataOption> Options { get; private set; }

        internal Dictionary<ulong, SocketGuildUser> guildMembers { get; private set; }
            = new Dictionary<ulong, SocketGuildUser>();
        internal Dictionary<ulong, SocketGlobalUser> users { get; private set; }
            = new Dictionary<ulong, SocketGlobalUser>();
        internal Dictionary<ulong, SocketChannel> channels { get; private set; }
            = new Dictionary<ulong, SocketChannel>();
        internal Dictionary<ulong, SocketRole> roles { get; private set; }
            = new Dictionary<ulong, SocketRole>();

        private ulong? guildId;

        internal SlashCommandData(DiscordSocketClient client, Model model, ulong? guildId)
        {
            this.guildId = guildId;

            if (model.Resolved.IsSpecified)
            {
                var guild = this.guildId.HasValue ? client.GetGuild(this.guildId.Value) : null;

                var resolved = model.Resolved.Value;

                if (resolved.Users.IsSpecified)
                {
                    foreach (var user in resolved.Users.Value)
                    {
                        var socketUser = client.GetOrCreateUser(client.State, user.Value);

                        this.users.Add(ulong.Parse(user.Key), socketUser);
                    }
                }

                if (resolved.Channels.IsSpecified)
                {
                    foreach (var channel in resolved.Channels.Value)
                    {
                        SocketChannel socketChannel = guild != null
                            ? guild.GetChannel(channel.Value.Id)
                            : client.GetChannel(channel.Value.Id);

                        if (socketChannel == null)
                        {
                            var channelModel = guild != null
                                ? client.Rest.ApiClient.GetChannelAsync(guild.Id, channel.Value.Id).ConfigureAwait(false).GetAwaiter().GetResult()
                                : client.Rest.ApiClient.GetChannelAsync(channel.Value.Id).ConfigureAwait(false).GetAwaiter().GetResult();

                            socketChannel = guild != null
                                ? SocketGuildChannel.Create(guild, client.State, channelModel)
                                : (SocketChannel)SocketChannel.CreatePrivate(client, client.State, channelModel);
                        }

                        client.State.AddChannel(socketChannel);
                        this.channels.Add(ulong.Parse(channel.Key), socketChannel);
                    }
                }

                if (resolved.Members.IsSpecified)
                {
                    foreach (var member in resolved.Members.Value)
                    {
                        member.Value.User = resolved.Users.Value[member.Key];
                        var user = guild.AddOrUpdateUser(member.Value);
                        this.guildMembers.Add(ulong.Parse(member.Key), user);
                    }
                }

                if (resolved.Roles.IsSpecified)
                {
                    foreach (var role in resolved.Roles.Value)
                    {
                        var socketRole = guild.AddOrUpdateRole(role.Value);
                        this.roles.Add(ulong.Parse(role.Key), socketRole);
                    }
                }
            }
        }

        internal static SlashCommandData Create(DiscordSocketClient client, Model model, ulong id, ulong? guildId)
        {
            var entity = new SlashCommandData(client, model, guildId);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            CommandId = model.Id;
            this.CommandName = model.Name;

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => new SlashCommandDataOption(this, x)).ToImmutableArray()
                : null;
        }

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionData.Options => Options;
    }
}
