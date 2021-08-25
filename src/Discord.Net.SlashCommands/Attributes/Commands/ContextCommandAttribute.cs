using System;
using System.Reflection;

namespace Discord.SlashCommands
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ContextCommandAttribute : Attribute
    {
        public string Name { get; }
        public ApplicationCommandType CommandType { get; }

        internal ContextCommandAttribute (string name, ApplicationCommandType commandType)
        {
            Name = name;
            CommandType = commandType;
        }

        internal virtual void CheckMethodDefinition (MethodInfo methodInfo) { }
    }
}
