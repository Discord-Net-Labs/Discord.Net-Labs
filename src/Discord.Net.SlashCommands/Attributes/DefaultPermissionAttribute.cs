using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Attribute used to set the "Default Permission" property of an Application Command
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DefaultPermissionAttribute : Attribute
    {
        /// <summary>
        /// Wheter the users are allowed to use a Slash Command by default
        /// </summary>
        public bool Allow { get; }

        /// <summary>
        /// Set the default permission of a Slash Command
        /// </summary>
        /// <param name="allow">Set if the users are allowed to use this command</param>
        public DefaultPermissionAttribute (bool allow)
        {
            Allow = allow;
        }
    }
}
