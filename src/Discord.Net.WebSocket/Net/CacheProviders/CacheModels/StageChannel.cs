using Discord.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChannelModel = Discord.API.Channel;

namespace Discord.WebSocket.CacheModels
{
    internal class StageChannel : Channel
    {
        public StagePrivacyLevel? PrivacyLevel { get; set; }
        public string StageTopic { get; set; }
        public bool? DiscoverableDisabled { get; set; }
        public bool Live { get; set; }
        public StageChannel() { }
        public StageChannel(ChannelModel model)
        {
            this.Id = model.Id;
            this.Type = model.Type;
            this.Bitrate = model.Bitrate;
            this.CategoryId = model.CategoryId;
            this.GuildId = model.GuildId;
            this.Icon = model.Icon;
            this.LastMessageId = model.LastMessageId;
            this.LastPinTimestamp = model.LastPinTimestamp;
            this.MemberCount = model.MemberCount;
            this.MessageCount = model.MessageCount;
            this.Name = model.Name;
            this.Nsfw = model.Nsfw;
            this.OwnerId = model.OwnerId;
            this.PermissionOverwrites = model.PermissionOverwrites;
            this.Position = model.Position;
            this.Recipients = model.Recipients;
            this.SlowMode = model.SlowMode;
            this.ThreadMember = model.ThreadMember;
            this.ThreadMetadata = model.ThreadMetadata;
            this.Topic = model.Topic;
            this.UserLimit = model.UserLimit;
        }
    }
}
