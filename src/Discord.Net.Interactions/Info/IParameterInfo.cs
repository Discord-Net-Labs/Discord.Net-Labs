using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a <see cref="ICommandInfo"/> parameter
    /// </summary>
    public interface IParameterInfo
    {
        /// <summary>
        ///     Command that this parameter belongs to
        /// </summary>
        ICommandInfo Command { get; }

        /// <summary>
        ///     Name of this parameter
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Type of this parameter
        /// </summary>
        Type ParameterType { get; }

        /// <summary>
        ///     Whether this parameter is required
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        ///     Whether this parameter is marked with a <see langword="params"/> keyword
        /// </summary>
        bool IsParameterArray { get; }

        /// <summary>
        ///     Default value of this parameter if the parameter is optional
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        ///     Get a list of the attributes this parameter has
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     Get a list of the preconditions this parameter has
        /// </summary>
        IReadOnlyCollection<ParameterPreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     Check if an execution context meets the parameter precondition requirements
        /// </summary>
        Task<PreconditionResult> CheckPreconditionsAsync (IInteractionContext context, object value, IServiceProvider services);
    }
}
