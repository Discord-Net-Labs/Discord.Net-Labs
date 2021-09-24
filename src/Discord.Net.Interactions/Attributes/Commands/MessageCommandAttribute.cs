using System;
using System.Reflection;

namespace Discord.Interactions
{
    /// <summary>
    /// Use to create an Message Context Command.
    /// </summary>
    /// <remarks>
    /// Not affected by the <see cref="GroupAttribute"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MessageCommandAttribute : ContextCommandAttribute
    {
        /// <summary>
        /// Register a method as a Message Context Command
        /// </summary>
        /// <param name="name">Name of the context command</param>
        public MessageCommandAttribute (string name) : base(name, ApplicationCommandType.Message) { }

        internal override void CheckMethodDefinition (MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length != 1 || !parameters[0].ParameterType.IsAssignableFrom(typeof(IMessage)))
                throw new InvalidOperationException($"Message Commands must have only one parameter that is a type of {nameof(IMessage)}");
        }
    }
}
