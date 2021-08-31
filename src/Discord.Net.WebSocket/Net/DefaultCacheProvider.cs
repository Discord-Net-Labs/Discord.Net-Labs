using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Net
{
    public class DefaultCacheProvider : CacheProvider
    {
        private readonly ConcurrentDictionary<ulong, byte[]> _channels;
        private readonly ConcurrentDictionary<ulong, byte[]> _dmChannels;
        private readonly ConcurrentDictionary<ulong, byte[]> _guilds;
        private readonly ConcurrentDictionary<ulong, byte[]> _users;
        private readonly ConcurrentHashSet<ulong> _groupChannels;
        private readonly ConcurrentDictionary<ulong, byte[]> _commands;

        public DefaultCacheProvider()
        {

        }

        public override void AddOrUpdateEntity(ulong id, CacheType type, IEnumerable<byte> data) => throw new NotImplementedException();
        public override void AddRange(IDictionary<ulong, IEnumerable<byte>> data, CacheType type) => throw new NotImplementedException();
        public override IEnumerable<byte> GetEntity(ulong id, CacheType type) => throw new NotImplementedException();
        public override IEnumerable<IEnumerable<byte>> ListCache(CacheType type) => throw new NotImplementedException();
        public override void PruneCache(CacheType type) => throw new NotImplementedException();
        public override IEnumerable<byte> RemoveEntity(ulong id, CacheType type) => throw new NotImplementedException();
        public override void StoreEntity(ulong id, CacheType type, IEnumerable<byte> data) => throw new NotImplementedException();
        public override void UpdateEntity(ulong id, CacheType type, IEnumerable<byte> data) => throw new NotImplementedException();
    }
}
