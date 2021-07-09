using System;

namespace Discord
{
    /// <summary>
    /// Interaction Response flags
    /// </summary>
    [Flags]
    public enum InteractionApplicationCommandCallbackFlags
    {
        /// <summary>
        /// Only the author can see the Interaction Response
        /// </summary>
        Ephemeral = 1 << 6
    }
}
