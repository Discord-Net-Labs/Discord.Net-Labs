using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct User : ICacheModel, ICachedUser
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public bool IsBot { get; set; }
        public string Avatar { get; set; }

        public CurrentUser? CurrentUser { get; set; }
    }
}
