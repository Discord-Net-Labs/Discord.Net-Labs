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
        /// <param name="type">The type of cache the entity should be stored in.</param>
        /// <param name="data">The entity data.</param>
        void StoreEntity(ulong id, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Gets an entity from a cache with the provided id.
        /// </summary>
        /// <param name="id">The id of the entity to get.</param>
        /// <param name="type">The cache type to get the entity from.</param>
        /// <returns>The encoded data of the entity with the provided id; if no entity is found with that id then <see langword="null"/>.</returns>
        IEnumerable<byte> GetEntity(ulong id, CacheType type);

        /// <summary>
        ///     Updates an entity within the cache.
        /// </summary>
        /// <param name="id">The id of the entity to update.</param>
        /// <param name="type">The type of cache the entity is in.</param>
        /// <param name="data">The encoded data of the entity.</param>
        void UpdateEntity(ulong id, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Adds or updates an entity within the cache.
        /// </summary>
        /// <param name="id">The id of the entity to add or update.</param>
        /// <param name="type">The type of cache where the entity will be added, or updated if it exists.</param>
        /// <param name="data">The encoded data of the entity.</param>
        void AddOrUpdateEntity(ulong id, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Removes an entity from the cache.
        /// </summary>
        /// <param name="id">The id of the entity to remove.</param>
        /// <param name="type">The cache which this entity is in.</param>
        /// <returns>The removed entities encoded data if found; otherwise <see langword="null"/>.</returns>
        IEnumerable<byte> RemoveEntity(ulong id, CacheType type);

        /// <summary>
        ///     Clears a cache for this bot.
        /// </summary>
        /// <param name="type">The type of cache to clear.</param>
        void PruneCache(CacheType type);

        /// <summary>
        ///     Gets all items within a cache.
        /// </summary>
        /// <param name="type">The type of cache to list.</param>
        /// <returns>A collection of buffers that contain the entity data.</returns>
        IEnumerable<IEnumerable<byte>> ListCache(CacheType type);

        /// <summary>
        ///     Adds a range of entities to a cache.
        /// </summary>
        /// <param name="data">The collection of entities to add</param>
        /// <param name="type">the cache type to add</param>
        void AddRange(IDictionary<ulong, IEnumerable<byte>> data, CacheType type);
    }

    /// <summary>
    ///     Represents a base class used when creating Cache Providers.
    /// </summary>
    public abstract class CacheProvider : ICacheProvider
    {
        /// <inheritdoc/>
        public abstract void AddOrUpdateEntity(ulong id, CacheType type, IEnumerable<byte> data);
        /// <inheritdoc/>
        public abstract void AddRange(IDictionary<ulong, IEnumerable<byte>> data, CacheType type);
        /// <inheritdoc/>
        public abstract IEnumerable<byte> GetEntity(ulong id, CacheType type);
        /// <inheritdoc/>
        public abstract IEnumerable<IEnumerable<byte>> ListCache(CacheType type);
        /// <inheritdoc/>
        public abstract void PruneCache(CacheType type);
        /// <inheritdoc/>
        public abstract IEnumerable<byte> RemoveEntity(ulong id, CacheType type);
        /// <inheritdoc/>
        public abstract void StoreEntity(ulong id, CacheType type, IEnumerable<byte> data);
        /// <inheritdoc/>
        public abstract void UpdateEntity(ulong id, CacheType type, IEnumerable<byte> data);
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
        /// <param name="type">The type of cache the entity should be stored in.</param>
        /// <param name="data">The entity data.</param>
        public abstract Task StoreEntityAsync(ulong id, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Gets an entity from a cache with the provided id.
        /// </summary>
        /// <param name="id">The id of the entity to get.</param>
        /// <param name="type">The cache type to get the entity from.</param>
        /// <returns>The encoded data of the entity with the provided id; if no entity is found with that id then <see langword="null"/>.</returns>
        public abstract Task<IEnumerable<byte>> GetEntityAsync(ulong id, CacheType type);

        /// <summary>
        ///     Updates an entity within the cache.
        /// </summary>
        /// <param name="id">The id of the entity to update.</param>
        /// <param name="type">The type of cache the entity is in.</param>
        /// <param name="data">The encoded data of the entity.</param>
        public abstract Task UpdateEntityAsync(ulong id, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Adds or updates an entity within the cache.
        /// </summary>
        /// <param name="id">The id of the entity to add or update.</param>
        /// <param name="type">The type of cache where the entity will be added, or updated if it exists.</param>
        /// <param name="data">The encoded data of the entity.</param>
        public abstract Task AddOrUpdateEntityAsync(ulong id, CacheType type, IEnumerable<byte> data);

        /// <summary>
        ///     Clears a cache for this bot.
        /// </summary>
        /// <param name="type">The type of cache to clear.</param>
        public abstract Task PruneCacheAsync(CacheType type);

        /// <summary>
        ///     Removes an entity from the cache.
        /// </summary>
        /// <param name="id">The id of the entity to remove.</param>
        /// <param name="type">The cache which this entity is in.</param>
        /// <returns>The removed entities encoded data if found; otherwise <see langword="null"/>.</returns>
        public abstract Task<IEnumerable<byte>> RemoveEntityAsync(ulong id, CacheType type);

        /// <summary>
        ///     Gets all items within a cache.
        /// </summary>
        /// <param name="type">The type of cache to list.</param>
        /// <returns>A collection of buffers that contain the entity data.</returns>
        public abstract Task<IEnumerable<IEnumerable<byte>>> ListCacheAsync(CacheType type);
        /// <summary>
        ///     Adds a range of entities to a cache.
        /// </summary>
        /// <param name="data">The collection of entities to add</param>
        /// <param name="type">the cache type to add</param>
        public abstract Task AddRangeAsync(IDictionary<ulong, IEnumerable<byte>> data, CacheType type);

        /// <inheritdoc/>
        void ICacheProvider.AddOrUpdateEntity(ulong id, CacheType type, IEnumerable<byte> data)
            => AddOrUpdateEntityAsync(id, type, data).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <inheritdoc/>
        IEnumerable<byte> ICacheProvider.GetEntity(ulong id, CacheType type)
            => GetEntityAsync(id, type).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <inheritdoc/>
        void ICacheProvider.PruneCache(CacheType type)
            => PruneCacheAsync(type).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <inheritdoc/>
        void ICacheProvider.StoreEntity(ulong id, CacheType type, IEnumerable<byte> data)
            => StoreEntityAsync(id, type, data).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <inheritdoc/>
        void ICacheProvider.UpdateEntity(ulong id, CacheType type, IEnumerable<byte> data)
            => UpdateEntityAsync(id, type, data).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <inheritdoc/>
        IEnumerable<byte> ICacheProvider.RemoveEntity(ulong id, CacheType type)
            => RemoveEntityAsync(id, type).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <inheritdoc/>
        void ICacheProvider.AddRange(IDictionary<ulong, IEnumerable<byte>> data, CacheType type)
            => AddRangeAsync(data, type).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <inheritdoc/>
        IEnumerable<IEnumerable<byte>> ICacheProvider.ListCache(CacheType type)
            => ListCacheAsync(type).ConfigureAwait(false).GetAwaiter().GetResult();

    }
}
