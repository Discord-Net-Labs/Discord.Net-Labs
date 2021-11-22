using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct GuildEmote
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public ulong[] Roles { get; set; }
        public ulong? UserId { get; set; }
        public bool RequireColons { get; set; }
        public bool Managed { get; set; }
        public bool Animated { get; set; }
        public bool? Available { get; set; }

        internal Discord.GuildEmote ToEntity()
        {
            return new Discord.GuildEmote(Id, Name, Animated, Managed, RequireColons, Roles, UserId);
        }
    }
}
