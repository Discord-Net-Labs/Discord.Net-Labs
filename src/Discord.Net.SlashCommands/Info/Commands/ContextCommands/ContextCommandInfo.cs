using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Base information class for attribute based context command handlers
    /// </summary>
    public abstract class ContextCommandInfo : CommandInfo<CommandParameterInfo>, IApplicationCommandInfo
    {
        /// <inheritdoc/>
        public ApplicationCommandType CommandType { get; }

        /// <summary>
        /// Get the initial value of this commands default permission
        /// </summary>
        public bool DefaultPermission { get; }

        public override IReadOnlyCollection<CommandParameterInfo> Parameters { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => false;

        internal ContextCommandInfo (Builders.ContextCommandBuilder builder, ModuleInfo module, SlashCommandService commandService)
            : base(builder, module, commandService)
        {
            CommandType = builder.CommandType;
            DefaultPermission = builder.DefaultPermission;
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
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
