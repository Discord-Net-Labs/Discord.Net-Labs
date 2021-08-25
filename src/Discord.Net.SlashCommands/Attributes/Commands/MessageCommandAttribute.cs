using System;
using System.Reflection;

namespace Discord.SlashCommands
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MessageCommandAttribute : ContextCommandAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public MessageCommandAttribute (string name) : base(name, ApplicationCommandType.Message) { }

        internal override void CheckMethodDefinition (MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length != 1 || !parameters[0].ParameterType.IsAssignableFrom(typeof(IMessage)))
                throw new InvalidOperationException($"Message Commands must have only one parameter that is a type of {nameof(IMessage)}");
        }
    }
}
