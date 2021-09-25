using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Set the "Default Permission" property of an Application Command
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DefaultPermissionAttribute : Attribute
    {
        /// <summary>
        ///     Whether the users are allowed to use a Slash Command by default or not
        /// </summary>
        public bool Allow { get; }

        /// <summary>
        ///     Set the default permission of a Slash Command
        /// </summary>
        /// <param name="allow"><see cref="true"/> if the users are allowed to use this command</param>
        public DefaultPermissionAttribute (bool allow)
        {
            Allow = allow;
        }
    }
}
