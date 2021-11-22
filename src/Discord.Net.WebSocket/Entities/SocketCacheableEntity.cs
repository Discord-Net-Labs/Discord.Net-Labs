using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public abstract class SocketCacheableEntity<TModel, TId> : SocketEntity<TId>
        where TModel : ICacheModel
        where TId : IEquatable<TId>
    {
        internal SocketCacheableEntity(DiscordSocketClient discord, TId id)
            : base(discord, id) { }

        internal abstract TModel ToCacheModel();

        internal abstract void Update(DiscordSocketClient discord, TModel model);
    }
}
