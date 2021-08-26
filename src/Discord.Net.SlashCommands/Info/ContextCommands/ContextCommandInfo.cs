using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    public abstract class ContextCommandInfo : ExecutableInfo, IApplicationCommandInfo
    {
        public ApplicationCommandType CommandType { get; }
        public bool DefaultPermission { get; }
        public IReadOnlyList<ParameterInfo> Parameters { get; }
        public IReadOnlyList<Attribute> Attributes { get; }

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

        protected override Task InvokeModuleEvent (ISlashCommandContext context, IResult result)
            => CommandService._contextCommandExecutedEvent.InvokeAsync(this, context, result);
    }
}
