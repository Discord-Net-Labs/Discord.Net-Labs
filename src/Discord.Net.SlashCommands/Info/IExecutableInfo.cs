using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represent a command information object that can be executed
    /// </summary>
    public interface IExecutableInfo
    {
        /// <summary>
        /// Name of the method
        /// </summary>
        string Name { get; }

        /// <summary>
        /// If true, this command will be registered and executed as a standalone command, unaffected by the <see cref="SlashGroupAttribute"/>s of its declaring types
        /// </summary>
        bool IgnoreGroupNames { get; }

        /// <summary>
        /// Get wheter this command type supports wild card pattern
        /// </summary>
        bool SupportsWildCards { get; }

        /// <summary>
        /// Module the method belongs to
        /// </summary>
        ModuleInfo Module { get; }

        /// <summary>
        /// Get the the underlying command service
        /// </summary>
        SlashCommandService CommandService { get; }

        /// <summary>
        /// Executes the command with the provided context
        /// </summary>
        /// <param name="context">Context of the command</param>
        /// <param name="services">Dependencies that will be used to create the module instance</param>
        /// <returns></returns>
        Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services);
    }
}
