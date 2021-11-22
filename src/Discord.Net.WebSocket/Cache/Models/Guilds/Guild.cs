using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct Guild : ICacheModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Splash { get; set; }
        public string DiscoverySplash { get; set; }
        public ulong OwnerId { get; set; }
        public string Region { get; set; }
        public ulong? AFKChannelId { get; set; }
        public int AFKTimeout { get; set; }
        public VerificationLevel VerificationLevel { get; set; }
        public DefaultMessageNotifications DefaultMessageNotifications { get; set; }
        public ExplicitContentFilterLevel ExplicitContentFilter { get; set; }
        public Role[] Roles { get; set; }
        public GuildEmote[] Emotes { get; set; }
        public GuildFeature Features { get; set; }
        public string[] ExperimentalGuildFeatures { get; set; }
        public MfaLevel MfaLevel { get; set; }
        public ulong? ApplicationId { get; set; }
        public bool? WidgetEnabled { get; set; }
        public ulong? WidgetChannelId { get; set; }
        public ulong? SystemChannelId { get; set; }
        public PremiumTier PremiumTier { get; set; }
        public string VanityURLCode { get; set; }
        public string Banner { get; set; }
        public string Description { get; set; }
        public SystemChannelMessageDeny SystemChannelFlags { get; set; }
        public ulong? RulesChannelId { get; set; }
        public int? MaxPresences { get; set; }
        public int? MaxMembers { get; set; }
        public int? PremiumSubscriptionCount { get; set; }
        public string PreferredLocale { get; set; }
        public ulong? PublicUpdatesChannelId { get; set; }
        public int? MaxVideoChannelUsers { get; set; }
        public int ApproximateMemberCount { get; set; }
        public int ApproximatePresenceCount { get; set; }
        public bool BoostProgressBarEnabled { get; set; }
        public NsfwLevel NsfwLevel { get; set; }
        public GuildSticker[] Stickers { get; set; }
    }
}
