using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represent a command information object that can be executed
    /// </summary>
    public interface ICommandInfo
    { 
        /// <summary>
        /// Name of the command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Name of the command handler method
        /// </summary>
        string MethodName { get; }

        /// <summary>
        /// If true, this command will be registered and executed as a standalone command, unaffected by the <see cref="SlashGroupAttribute"/>s of its declaring types
        /// </summary>
        bool IgnoreGroupNames { get; }

        /// <summary>
        /// Get whether this command type supports wild card pattern
        /// </summary>
        bool SupportsWildCards { get; }

        /// <summary>
        /// <see langword="true"/> if this command is a top level application command and has not parent module with a Group name
        /// </summary>
        bool IsTopLevel { get; }

        /// <summary>
        /// Module the method belongs to
        /// </summary>
        ModuleInfo Module { get; }

        /// <summary>
        /// Get the the underlying command service
        /// </summary>
        SlashCommandService CommandService { get; }

        RunMode RunMode { get; }

        IReadOnlyCollection<Attribute> Attributes { get; }
        IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }
        IReadOnlyCollection<IParameterInfo> Parameters { get; }

        /// <summary>
        /// Executes the command with the provided context
        /// </summary>
        /// <param name="context">Context of the command</param>
        /// <param name="services">Dependencies that will be used to create the module instance</param>
        /// <returns></returns>
        Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services);

        Task<PreconditionResult> CheckPreconditionsAsync (ISlashCommandContext context, IServiceProvider services);
    }
}
