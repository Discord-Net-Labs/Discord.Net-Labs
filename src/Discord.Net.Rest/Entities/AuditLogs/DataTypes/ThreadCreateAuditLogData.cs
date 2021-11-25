using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a thread creation.
    /// </summary>
    public class ThreadCreateAuditLogData : IAuditLogData
    {
        private ThreadCreateAuditLogData(IThreadChannel thread, ulong id, string name, ThreadType type, bool archived,
            ThreadArchiveDuration autoArchiveDuration, bool locked)
        {
            Thread = thread;
            ThreadId = id;
            ThreadName = name;
            ThreadType = type;
            IsArchived = archived;
            AutoArchiveDuration = autoArchiveDuration;
            IsLocked = locked;
        }

        internal static ThreadCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var id = entry.TargetId.Value;

            var nameModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var typeModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "type");

            var archivedModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "archived");
            var autoArchiveDurationModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "auto_archive_duration");
            var lockedModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "locked");

            var name = nameModel.NewValue.ToObject<string>(discord.ApiClient.Serializer);
            var type = typeModel.NewValue.ToObject<ThreadType>(discord.ApiClient.Serializer);

            var archived = archivedModel.NewValue.ToObject<bool>(discord.ApiClient.Serializer);
            var autoArchiveDuration = autoArchiveDurationModel.NewValue.ToObject<ThreadArchiveDuration>(discord.ApiClient.Serializer);
            var locked = lockedModel.NewValue.ToObject<bool>(discord.ApiClient.Serializer);

            var threadInfo = log.Threads.FirstOrDefault(x => x.Id == id);
            var threadChannel = threadInfo == null ? null : RestThreadChannel.Create(discord, (IGuild)null, threadInfo);

            return new ThreadCreateAuditLogData(threadChannel, id, name, type, archived, autoArchiveDuration, locked);
        }

        // Doc Note: Corresponds to the *current* data

        /// <summary>
        ///     Gets the thread that was created if it still exists.
        /// </summary>
        /// <returns>
        ///     A thread object representing the thread that was created if it still exists, otherwise returns <c>null</c>.
        /// </returns>
        public IThreadChannel Thread { get; }

        /// <summary>
        ///     Gets the snowflake ID of the thread.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the thread.
        /// </returns>
        public ulong ThreadId { get; }
        /// <summary>
        ///     Gets the name of the thread.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the thread.
        /// </returns>
        public string ThreadName { get; }
        /// <summary>
        ///     Gets the type of the thread.
        /// </summary>
        /// <returns>
        ///     The type of thread.
        /// </returns>
        public ThreadType ThreadType { get; }
        /// <summary>
        ///     Gets the value that indicates whether the thread is archived.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this thread has the Archived flag enabled; otherwise <c>false</c>.
        /// </returns>
        public bool IsArchived { get; }
        /// <summary>
        ///     Gets the auto archive duration of the thread.
        /// </summary>
        /// <returns>
        ///     The thread auto archive duration of the thread.
        /// </returns>
        public ThreadArchiveDuration AutoArchiveDuration { get; }
        /// <summary>
        ///     Gets the value that indicates whether the thread is locked.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this thread has the Locked flag enabled; otherwise <c>false</c>.
        /// </returns>
        public bool IsLocked { get; }
    }
}
