using System;
using System.Reflection;

namespace Discord.SlashCommands
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UserCommandAttribute : ContextCommandAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public UserCommandAttribute (string name) : base(name, ApplicationCommandType.User) { }

        internal override void CheckMethodDefinition (MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length != 1 || !parameters[0].ParameterType.IsAssignableFrom(typeof(IUser)))
                throw new InvalidOperationException($"User Commands must have only one parameter that is a type of {nameof(IUser)}");
        }
    }
}
