namespace Discord.Rest
{
    /// <summary>
    ///     Represents information for a thread.
    /// </summary>
    public class ThreadInfo
    {
        /// <summary>
        ///     Gets the name of the thread.
        /// </summary>
        public string Name { get; }
        /// <summary>
        ///     Gets the value that indicates whether the thread is archived.
        /// </summary>
        public bool IsArchived { get; }
        /// <summary>
        ///     Gets the auto archive duration of thread.
        /// </summary>
        public ThreadArchiveDuration AutoArchiveDuration { get; }
        /// <summary>
        ///     Gets the value that indicates whether the thread is locked.
        /// </summary>
        public bool IsLocked { get; }

        internal ThreadInfo(string name, bool archived, ThreadArchiveDuration autoArchiveDuration, bool locked)
        {
            Name = name;
            IsArchived = archived;
            AutoArchiveDuration = autoArchiveDuration;
            IsLocked = locked;
        }
    }
}
