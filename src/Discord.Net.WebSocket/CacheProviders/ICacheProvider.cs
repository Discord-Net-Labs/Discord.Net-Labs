using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface ICacheProvider
    {
        /// <summary>
        ///     Stores an encoded entity inside of a cache.
        /// </summary>
        /// <param name="id">The id of the entity to store</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache the entity should be stored in.</param>
        /// <param name="data">The entity data.</param>
        void StoreEntity(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Gets an entity from a cache with the provided id.
        /// </summary>
        /// <param name="id">The id of the entity to get.</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The cache type to get the entity from.</param>
        /// <returns>The encoded data of the entity with the provided id; if no entity is found with that id then <see langword="null"/>.</returns>
        IEnumerable<byte> GetEntity(ulong id, ulong? guildId, CacheType type);

        /// <summary>
        ///     Updates an entity within the cache.
        /// </summary>
        /// <param name="id">The id of the entity to update.</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache the entity is in.</param>
        /// <param name="data">The encoded data of the entity.</param>
        /// <returns>The old data of the updated entity if updated; otherwise <see langword="null"/>.</returns>
        IEnumerable<byte> UpdateEntity(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Adds or updates an entity within the cache.
        /// </summary>
        /// <param name="id">The id of the entity to add or update.</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache where the entity will be added, or updated if it exists.</param>
        /// <param name="data">The encoded data of the entity.</param>
        void AddOrUpdateEntity(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Removes an entity from the cache.
        /// </summary>
        /// <param name="id">The id of the entity to remove.</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The cache which this entity is in.</param>
        /// <returns>The removed entities encoded data if found; otherwise <see langword="null"/>.</returns>
        IEnumerable<byte> RemoveEntity(ulong id, ulong? guildId, CacheType type);

        /// <summary>
        ///     Clears a cache for this bot.
        /// </summary>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache to clear.</param>
        void PruneCache(ulong? guildId, CacheType type);

        /// <summary>
        ///     Gets all items within a cache.
        /// </summary>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache to list.</param>
        /// <returns>A collection of buffers that contain the entity data.</returns>
        IEnumerable<IEnumerable<byte>> ListCache(ulong? guildId, CacheType type);

        /// <summary>
        ///     Adds a range of entities to a cache.
        /// </summary>
        /// <param name="data">The collection of entities to add</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">the cache type to add</param>
        void AddRange(IDictionary<ulong, IEnumerable<byte>> data, ulong? guildId, CacheType type);
    }

    /// <summary>
    ///     Represents a base class used when creating Cache Providers.
    /// </summary>
    public abstract class CacheProvider : ICacheProvider
    {
        /// <inheritdoc/>
        public abstract void AddOrUpdateEntity(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data);
        /// <inheritdoc/>
        public abstract void AddRange(IDictionary<ulong, IEnumerable<byte>> data, ulong? guildId, CacheType type);
        /// <inheritdoc/>
        public abstract IEnumerable<byte> GetEntity(ulong id, ulong? guildId, CacheType type);
        /// <inheritdoc/>
        public abstract IEnumerable<IEnumerable<byte>> ListCache(ulong? guildId, CacheType type);
        /// <inheritdoc/>
        public abstract void PruneCache(ulong? guildId, CacheType type);
        /// <inheritdoc/>
        public abstract IEnumerable<byte> RemoveEntity(ulong id, ulong? guildId, CacheType type);
        /// <inheritdoc/>
        public abstract void StoreEntity(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data);
        /// <inheritdoc/>
        public abstract IEnumerable<byte> UpdateEntity(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data);
    }

    /// <summary>
    ///     Represents a base class used when creating asynchronous cache providers.
    /// </summary>
    public abstract class AsyncCacheProvider : ICacheProvider
    {
        /// <summary>
        ///     Stores an encoded entity inside of a cache.
        /// </summary>
        /// <param name="id">The id of the entity to store</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache the entity should be stored in.</param>
        /// <param name="data">The entity data.</param>
        public abstract Task StoreEntityAsync(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Gets an entity from a cache with the provided id.
        /// </summary>
        /// <param name="id">The id of the entity to get.</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The cache type to get the entity from.</param>
        /// <returns>The encoded data of the entity with the provided id; if no entity is found with that id then <see langword="null"/>.</returns>
        public abstract Task<IEnumerable<byte>> GetEntityAsync(ulong id, ulong? guildId, CacheType type);

        /// <summary>
        ///     Updates an entity within the cache.
        /// </summary>
        /// <param name="id">The id of the entity to update.</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache the entity is in.</param>
        /// <param name="data">The encoded data of the entity.</param>
        public abstract Task<IEnumerable<byte>> UpdateEntityAsync(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Adds or updates an entity within the cache.
        /// </summary>
        /// <param name="id">The id of the entity to add or update.</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache where the entity will be added, or updated if it exists.</param>
        /// <param name="data">The encoded data of the entity.</param>
        public abstract Task AddOrUpdateEntityAsync(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Clears a cache for this bot.
        /// </summary>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache to clear.</param>
        public abstract Task PruneCacheAsync(ulong? guildId, CacheType type);

        /// <summary>
        ///     Removes an entity from the cache.
        /// </summary>
        /// <param name="id">The id of the entity to remove.</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The cache which this entity is in.</param>
        /// <returns>The removed entities encoded data if found; otherwise <see langword="null"/>.</returns>
        public abstract Task<IEnumerable<byte>> RemoveEntityAsync(ulong id, ulong? guildId, CacheType type);

        /// <summary>
        ///     Gets all items within a cache.
        /// </summary>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">The type of cache to list.</param>
        /// <returns>A collection of buffers that contain the entity data.</returns>
        public abstract Task<IEnumerable<IEnumerable<byte>>> ListCacheAsync(ulong? guildId, CacheType type);
        /// <summary>
        ///     Adds a range of entities to a cache.
        /// </summary>
        /// <param name="data">The collection of entities to add</param>
        /// <param name="guildId">The guild id of the entity if the entity is within a guild; otherwise <see langword="null"/>.</param>
        /// <param name="type">the cache type to add</param>
        public abstract Task AddRangeAsync(IDictionary<ulong, IEnumerable<byte>> data, ulong? guildId, CacheType type);
        void ICacheProvider.StoreEntity(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data)
            => StoreEntityAsync(id, guildId, type, data).ConfigureAwait(false).GetAwaiter().GetResult();
        IEnumerable<byte> ICacheProvider.GetEntity(ulong id, ulong? guildId, CacheType type)
            => GetEntityAsync(id, guildId, type).ConfigureAwait(false).GetAwaiter().GetResult();
        IEnumerable<byte> ICacheProvider.UpdateEntity(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data)
            => UpdateEntityAsync(id, guildId, type, data).ConfigureAwait(false).GetAwaiter().GetResult();
        void ICacheProvider.AddOrUpdateEntity(ulong id, ulong? guildId, CacheType type, IEnumerable<byte> data)
            => AddOrUpdateEntityAsync(id, guildId, type, data).ConfigureAwait(false).GetAwaiter().GetResult();
        IEnumerable<byte> ICacheProvider.RemoveEntity(ulong id, ulong? guildId, CacheType type)
            => RemoveEntityAsync(id, guildId, type).ConfigureAwait(false).GetAwaiter().GetResult();
        void ICacheProvider.PruneCache(ulong? guildId, CacheType type)
            => PruneCacheAsync(guildId, type).ConfigureAwait(false).GetAwaiter().GetResult();
        IEnumerable<IEnumerable<byte>> ICacheProvider.ListCache(ulong? guildId, CacheType type)
            => ListCacheAsync(guildId, type).ConfigureAwait(false).GetAwaiter().GetResult();
        void ICacheProvider.AddRange(IDictionary<ulong, IEnumerable<byte>> data, ulong? guildId, CacheType type)
            => AddRangeAsync(data, guildId, type).ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
