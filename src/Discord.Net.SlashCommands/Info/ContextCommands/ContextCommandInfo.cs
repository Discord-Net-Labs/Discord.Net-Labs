using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Base information class for attribute based context command handlers
    /// </summary>
    public abstract class ContextCommandInfo : ExecutableInfo, IApplicationCommandInfo
    {
        /// <inheritdoc/>
        public ApplicationCommandType CommandType { get; }

        /// <summary>
        /// Get the initial value of this commands default permission
        /// </summary>
        public bool DefaultPermission { get; }

        /// <summary>
        /// Get a collection of the method parameters of this command
        /// </summary>
        public IReadOnlyList<ParameterInfo> Parameters { get; }

        /// <summary>
        /// Get a collection of attributes applied to this command
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => false;

        internal ContextCommandInfo (Builders.ContextCommandBuilder builder, ModuleInfo module, SlashCommandService commandService)
            : base(builder.Name, true, module, commandService, builder.Callback)
        {
            CommandType = builder.CommandType;
            DefaultPermission = builder.DefaultPermission;
            Parameters = builder.Parameters;
            Attributes = builder.Attributes;
        }

        internal static ContextCommandInfo Create (Builders.ContextCommandBuilder builder, ModuleInfo module, SlashCommandService commandService)
        {
            switch (builder.CommandType)
            {
                case ApplicationCommandType.User:
                    return new UserCommandInfo(builder, module, commandService);
                case ApplicationCommandType.Message:
                    return new MessageCommandInfo(builder, module, commandService);
                case ApplicationCommandType.Slash:
                default:
                    throw new InvalidOperationException("This command type is not a supported Context Command");
            }
        }

        /// <inheritdoc/>
        protected override Task InvokeModuleEvent (ISlashCommandContext context, IResult result)
            => CommandService._contextCommandExecutedEvent.InvokeAsync(this, context, result);
    }
}
