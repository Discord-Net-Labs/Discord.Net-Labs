using System;
using System.Reflection;

namespace Discord.Interactions
{
    /// <summary>
    ///     Base attribute for creating a Context Commands
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ContextCommandAttribute : Attribute
    {
        /// <summary>
        ///     Name of this Context Command
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Type of this Context Command
        /// </summary>
        public ApplicationCommandType CommandType { get; }

        public RunMode RunMode { get; set; } = RunMode.Default;

        internal ContextCommandAttribute (string name, ApplicationCommandType commandType)
        {
            Name = name;
            CommandType = commandType;
        }

        internal virtual void CheckMethodDefinition (MethodInfo methodInfo) { }
    }
}
