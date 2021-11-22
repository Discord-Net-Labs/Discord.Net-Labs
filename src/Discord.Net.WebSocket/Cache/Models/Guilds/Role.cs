using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct Role
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public uint Color { get; set; }
        public bool Hoist { get; set; }
        public string Icon { get; set; }
        public string UnicodeEmoji { get; set; }
        public int Position { get; set; }
        public ulong Permissions { get; set; }
        public bool Managed { get; set; }
        public bool Mentionable { get; set; }
        public RoleTags? Tags { get; set; }
    }

    public struct RoleTags
    {
        public ulong? BotId { get; set; }
        public ulong? IntegrationId { get; set; }
        public bool PremiumSubscriber { get; set; }

        public Discord.RoleTags ToEntity()
        {
            return new Discord.RoleTags(
                BotId.HasValue ? BotId.Value : null,
                IntegrationId.HasValue ? IntegrationId.Value : null,
                PremiumSubscriber);
        }
    }
}
