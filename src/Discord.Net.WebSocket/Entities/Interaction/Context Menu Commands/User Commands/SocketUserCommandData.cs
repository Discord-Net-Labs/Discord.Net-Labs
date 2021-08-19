using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data tied with the <see cref="SocketApplicationUserCommand"/> interaction.
    /// </summary>
    public class SocketApplicationUserCommandData : SocketEntity<ulong>, IApplicationCommandInteractionData
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        public SocketUser Member { get; private set; }

        internal Dictionary<ulong, SocketGuildUser> guildMembers { get; private set; }
            = new Dictionary<ulong, SocketGuildUser>();
        internal Dictionary<ulong, SocketGlobalUser> users { get; private set; }
            = new Dictionary<ulong, SocketGlobalUser>();
        internal Dictionary<ulong, SocketChannel> channels { get; private set; }
            = new Dictionary<ulong, SocketChannel>();
        internal Dictionary<ulong, SocketRole> roles { get; private set; }
            = new Dictionary<ulong, SocketRole>();

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionData.Options => throw new System.NotImplementedException();

        private ulong? guildId;

        private ApplicationCommandType Type;

        internal SocketApplicationUserCommandData(DiscordSocketClient client, Model model, ulong? guildId)
            : base(client, model.Id)
        {
            this.guildId = guildId;

            this.Type = (ApplicationCommandType)model.Type;

            if (model.Resolved.IsSpecified)
            {
                var guild = this.guildId.HasValue ? Discord.GetGuild(this.guildId.Value) : null;

                var resolved = model.Resolved.Value;

                if (resolved.Users.IsSpecified)
                {
                    foreach (var user in resolved.Users.Value)
                    {
                        var socketUser = Discord.GetOrCreateUser(this.Discord.State, user.Value);

                        this.users.Add(ulong.Parse(user.Key), socketUser);
                    }
                }

                if (resolved.Channels.IsSpecified)
                {
                    foreach (var channel in resolved.Channels.Value)
                    {
                        SocketChannel socketChannel = guild != null
                            ? guild.GetChannel(channel.Value.Id)
                            : Discord.GetChannel(channel.Value.Id);

                        if (socketChannel == null)
                        {
                            var channelModel = guild != null
                                ? Discord.Rest.ApiClient.GetChannelAsync(guild.Id, channel.Value.Id).ConfigureAwait(false).GetAwaiter().GetResult()
                                : Discord.Rest.ApiClient.GetChannelAsync(channel.Value.Id).ConfigureAwait(false).GetAwaiter().GetResult();

                            socketChannel = guild != null
                                ? SocketGuildChannel.Create(guild, Discord.State, channelModel)
                                : (SocketChannel)SocketChannel.CreatePrivate(Discord, Discord.State, channelModel);
                        }    

                        Discord.State.AddChannel(socketChannel);
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
                        this.Member = user;
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

        internal static SocketApplicationUserCommandData Create(DiscordSocketClient client, Model model, ulong id, ulong? guildId)
        {
            var entity = new SocketApplicationUserCommandData(client, model, guildId);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            this.Name = model.Name;
        }
    }
}
