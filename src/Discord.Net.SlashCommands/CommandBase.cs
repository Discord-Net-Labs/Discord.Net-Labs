using Discord.SlashCommands.Builders;
using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Base class for any Slash command handling modules
    /// </summary>
    /// <typeparam name="T">Type of slash command context to be injected into the module</typeparam>
    public abstract class CommandBase<T> : ISlashModuleBase where T : class, ISlashCommandContext
    {
        /// <summary>
        /// Command execution context for an user interaction.
        /// </summary>
        public T Context { get; private set; }

        /// <summary>
        /// Method body to be executed after an application command execution
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command</param>
        public virtual void AfterExecute (SlashCommandInfo command) { }

        /// <summary>
        /// Method body to be executed before executing an application command
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command</param>
        public virtual void BeforeExecute (SlashCommandInfo command) { }

        /// <summary>
        /// Method body to be executed before the derived module is builded
        /// </summary>
        /// <param name="commandService">Command service the derived module belongs to</param>
        /// <param name="builder">Module builder responsible of building the derived type</param>
        public virtual void OnModuleBuilding (SlashCommandService commandService, SlashModuleBuilder builder) { }
        public virtual void SetContext (ISlashCommandContext context)
        {
            var newValue = context as T;
            Context = newValue ?? throw new InvalidOperationException($"Invalid context type. Expected {typeof(T).Name}, got {context.GetType().Name}.");
        }
    }
}
