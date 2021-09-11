using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents a <see cref="ICommandInfo"/> parameter
    /// </summary>
    public interface IParameterInfo
    {
        /// <summary>
        /// Command this paramter belongs to
        /// </summary>
        ICommandInfo Command { get; }

        /// <summary>
        /// Get the name of this parameter
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Type of this parameter
        /// </summary>
        Type ParameterType { get; }

        /// <summary>
        /// Whether this parameter is required or not
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        /// Whether this parameter is marked with a <see langword="params"/> keyword
        /// </summary>
        bool IsParameterArray { get; }

        /// <summary>
        /// Default value of this parameter if the parameter is optional
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// Get a list of the attributes this parameter has
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        /// Get a list of the preconditions this parameter has
        /// </summary>
        IReadOnlyCollection<ParameterPreconditionAttribute> Preconditions { get; }

        /// <summary>
        /// Check if an execution context meets the parameter precondition requirements
        /// </summary>
        /// <param name="context">The execution context</param>
        /// <param name="services">The service collection that is used for dependency injection</param>
        /// <returns>A task representing the precondition checking process with a <see cref="PreconditionResult"/> result</returns>
        Task<PreconditionResult> CheckPreconditionsAsync (ISlashCommandContext context, IServiceProvider services);
    }
}
