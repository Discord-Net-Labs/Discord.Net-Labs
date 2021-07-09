using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    /// Contains the permissions data of a <see cref="IApplicationCommand"/>
    /// </summary>
    public interface IApplicationCommandPermissions
    {
        /// <summary>
        /// Command the permissions affect
        /// </summary>
        IApplicationCommand Command { get; }
        /// <summary>
        /// The collection of roles with boolean values showing wheter the role is authorized to use the command
        /// </summary>
        IEnumerable<KeyValuePair<IRole, bool>> RolePermissions { get; }
        /// <summary>
        /// The collection of users with boolean values showing wheter the user is authorized to use the command
        /// </summary>
        IEnumerable<KeyValuePair<IUser, bool>> UserPermissions { get; }
    }
}
