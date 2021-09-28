using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Requires the module or class to pass the specified precondition before execution can begin.
    /// </summary>
    /// <seealso cref="ParameterPreconditionAttribute"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class PreconditionAttribute : Attribute
    {
        /// <summary>
        ///     Specifies a group that this precondition belongs to.
        /// </summary>
        /// <remarks>
        ///     <see cref="Preconditions" /> of the same group require only one of the preconditions to pass in order to
        ///     be successful (A || B). Specifying <see cref="Group" /> = <c>null</c> or not at all will
        ///     require *all* preconditions to pass, just like normal (A &amp;&amp; B).
        /// </remarks>
        public string Group { get; set; } = null;

        /// <summary>
        ///     When overridden in a derived class, uses the supplied string
        ///     as the error message if the precondition doesn't pass.
        ///     Setting this for a class that doesn't override
        ///     this property is a no-op.
        /// </summary>
        public virtual string ErrorMessage { get; }

        /// <summary>
        ///     Checks if the <paramref name="commandInfo"/> command to be executed meets the precondition requirements
        /// </summary>
        /// <param name="context">The context of the command.</param>
        /// <param name="commandInfo">The command being executed.</param>
        /// <param name="services">The service collection used for dependency injection.</param>
        public abstract Task<PreconditionResult> CheckRequirementsAsync (IInteractionCommandContext context, ICommandInfo commandInfo, IServiceProvider services);
    }
}
