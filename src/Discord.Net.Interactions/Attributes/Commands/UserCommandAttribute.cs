using System;
using System.Reflection;

namespace Discord.Interactions
{
    /// <summary>
    /// Use to create an User Context Command.
    /// </summary>
    /// <remarks>
    /// Not affected by the <see cref="GroupAttribute"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UserCommandAttribute : ContextCommandAttribute
    {
        /// <summary>
        /// Register a command as a User Context Command
        /// </summary>
        /// <param name="name">Name of this context command</param>
        public UserCommandAttribute (string name) : base(name, ApplicationCommandType.User) { }

        internal override void CheckMethodDefinition (MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length != 1 || !parameters[0].ParameterType.IsAssignableFrom(typeof(IUser)))
                throw new InvalidOperationException($"User Commands must have only one parameter that is a type of {nameof(IUser)}");
        }
    }
}
