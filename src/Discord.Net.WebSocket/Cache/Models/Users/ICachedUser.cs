using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface ICachedUser : ICacheModel
    {
        ulong Id { get; }
        string Username { get; }
        string Discriminator { get; }
        bool IsBot { get; }
        string Avatar { get; }
        CurrentUser? CurrentUser { get; }
    }
}
