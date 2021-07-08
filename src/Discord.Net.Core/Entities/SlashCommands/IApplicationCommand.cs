using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// Represents an Discord Application Command
    /// </summary>
    public interface IApplicationCommand : ISnowflakeEntity, IUpdateable, IDeletable
    {
        /// <summary>
        /// Get the name of this command
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Get the description of this command
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Get the Snowflake ID of the application this command belongs to
        /// </summary>
        ulong ApplicationId { get; }
        /// <summary>
        /// Wheter this command is executable by defult
        /// </summary>
        bool DefaultPermission { get; }
        /// <summary>
        /// Get the guild this command belongs to if it is a Guild Command
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null"/> if the Command is a Global Command
        /// </remarks>
        IGuild Guild { get; }

        /// <summary>
        /// Contains the information on the options of this command, these may be Sub-Commands, Sub-Command Groups or command parameters
        /// </summary>
        IEnumerable<IApplicationCommandOption> Options { get; }

        /// <summary>
        /// Modify this Application Command on Discord
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="defaultPermission"></param>
        /// <param name="commandOptions"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<IApplicationCommand> Modify (string name, string description, bool defaultPermission, IEnumerable<IApplicationCommandOption> commandOptions,
            RequestOptions options);
    }
}
