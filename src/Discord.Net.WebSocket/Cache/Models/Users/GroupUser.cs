using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct GroupUser : ICacheModel, ICachedUser
    {
        public ulong ChannelId { get; set; }
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public bool IsBot { get; set; }
        public string Avatar { get; set; }
        public CurrentUser? CurrentUser { get; set; }

        internal User ToUser()
            => new User
            {
                Avatar = Avatar,
                CurrentUser = CurrentUser,
                Discriminator = Discriminator,
                Id = Id,
                IsBot = IsBot,
                Username = Username
            };
    }
}
