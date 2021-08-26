using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represent a command information object that can be executed
    /// </summary>
    internal interface IExecutableInfo
    {
        /// <summary>
        /// Name of the method
        /// </summary>
        string Name { get; }
        bool IgnoreGroupNames { get; }
        bool SupportsWildCards { get; }
        /// <summary>
        /// Module the method belongs to
        /// </summary>
        ModuleInfo Module { get; }
        /// <summary>
        /// Executes the command with the provided context
        /// </summary>
        /// <param name="context">Context of the command</param>
        /// <param name="services">Dependencies that will be used to create the module instance</param>
        /// <returns></returns>
        Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services);
    }
}
