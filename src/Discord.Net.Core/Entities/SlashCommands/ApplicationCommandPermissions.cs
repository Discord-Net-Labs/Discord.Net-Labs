using System.Collections.Generic;
using System.Linq;

namespace Discord
{
    public class ApplicationCommandPermissions : IApplicationCommandPermissions
    {
        public IApplicationCommand Command { get; }
        public IReadOnlyDictionary<IUser, bool> UserPermissions { get; }
        public IReadOnlyDictionary<IRole, bool> RolePermissions { get; }

        IApplicationCommand IApplicationCommandPermissions.Command => Command;


        IEnumerable<KeyValuePair<IRole, bool>> IApplicationCommandPermissions.RolePermissions => RolePermissions;

        IEnumerable<KeyValuePair<IUser, bool>> IApplicationCommandPermissions.UserPermissions => UserPermissions;

        internal ApplicationCommandPermissions (IApplicationCommand command, IDictionary<IUser, bool> users, IDictionary<IRole, bool> roles)
        {
            Command = command;
            UserPermissions = users.ToDictionary(x => x.Key, x => x.Value);
            RolePermissions = roles.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
