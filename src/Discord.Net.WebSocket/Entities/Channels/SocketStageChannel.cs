using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Channel;
using StageInstance = Discord.API.Gateway.StageInstance;

namespace Discord.WebSocket
{
    public class SocketStageChannel : SocketVoiceChannel, IStageChannel
    {
        /// <inheritdoc/>
        public string Topic { get; private set; }

        /// <inheritdoc/>
        public StagePrivacyLevel PrivacyLevel { get; private set; }

        /// <inheritdoc/>
        public bool DiscoverableDisabled { get; private set; }

        /// <inheritdoc/>
        public bool Live { get; private set; } = false;

        /// <summary>
        ///     Gets a collection of users who are speakers within the stage.
        /// </summary>
        public IReadOnlyCollection<SocketGuildUser> Speakers
            => this.Users.Where(x => !x.IsSuppressed).ToImmutableArray();

        internal new SocketStageChannel Clone() => MemberwiseClone() as SocketStageChannel;


        internal SocketStageChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id, guild)
        {
            
        }

        internal new static SocketStageChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketStageChannel(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }

        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);
        }

        internal void Update(StageInstance model, bool? isLive = null)
        {
            this.Topic = model.Topic;
            this.PrivacyLevel = model.PrivacyLevel;
            this.DiscoverableDisabled = model.DiscoverableDisabled;

            if (isLive.HasValue)
            {
                this.Live = isLive.Value;
            }
        }

        IReadOnlyCollection<IGuildUser> IStageChannel.Speakers => Speakers;
    }
}
