using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Interactions
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
        /// If true, this command will be registered and executed as a standalone command, unaffected by the <see cref="GroupAttribute"/>s of its declaring types
        /// </summary>
        bool IgnoreGroupNames { get; }

        /// <summary>
        /// Get whether this command type supports wild card pattern
        /// </summary>
        bool SupportsWildCards { get; }

        /// <summary>
        /// <see langword="true"/> if this command is a top level command and has not parent module with a Group name
        /// </summary>
        bool IsTopLevelCommand { get; }

        /// <summary>
        /// Module the method belongs to
        /// </summary>
        ModuleInfo Module { get; }

        /// <summary>
        /// Get the the underlying command service
        /// </summary>
        InteractionService CommandService { get; }

        /// <summary>
        /// Get the <see cref="RunMode"/> that will be used by this command
        /// </summary>
        RunMode RunMode { get; }

        /// <summary>
        /// Get a collection of the attributes of this command
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        /// Get a collection of the preconditions of this command
        /// </summary>
        IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        /// Get a collection of the parameters of this command
        /// </summary>
        IReadOnlyCollection<IParameterInfo> Parameters { get; }

        /// <summary>
        /// Executes the command with the provided context
        /// </summary>
        /// <param name="context">Context of the command</param>
        /// <param name="services">Dependencies that will be used to create the module instance</param>
        /// <returns>A task representing the execution process with a <see cref="IResult"/> result</returns>
        Task<IResult> ExecuteAsync (IInteractionCommandContext context, IServiceProvider services);

        /// <summary>
        /// Check if an execution context meets the command precondition requirements
        /// </summary>
        /// <param name="context">The execution context</param>
        /// <param name="services">The service collection that is used for dependency injection</param>
        /// <returns>A task representing the precondition checking process with a <see cref="PreconditionResult"/> result</returns>
        Task<PreconditionResult> CheckPreconditionsAsync (IInteractionCommandContext context, IServiceProvider services);
    }
}
