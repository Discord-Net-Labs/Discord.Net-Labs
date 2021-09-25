using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for command type <see cref="ApplicationCommandType.Slash"/>
    /// </summary>
    public class SlashCommandInfo : CommandInfo<SlashCommandParameterInfo>, IApplicationCommandInfo
    {
        /// <summary>
        ///     The command description that will be displayed on Discord
        /// </summary>
        public string Description { get; }

        /// <inheritdoc/>
        public ApplicationCommandType CommandType { get; } = ApplicationCommandType.Slash;

        /// <inheritdoc/>
        public bool DefaultPermission { get; }

        /// <inheritdoc/>
        public override IReadOnlyCollection<SlashCommandParameterInfo> Parameters { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => false;

        internal SlashCommandInfo (Builders.SlashCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Description = builder.Description;
            DefaultPermission = builder.DefaultPermission;
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync (IInteractionCommandContext context, IServiceProvider services)
        {
            if (context.Interaction is SocketSlashCommand commandInteraction)
            {
                var options = commandInteraction.Data.Options;

                IList<SocketSlashCommandDataOption> args = options?.ToList();
                while (args != null && args.Any(x => x?.Type == ApplicationCommandOptionType.SubCommand || x?.Type == ApplicationCommandOptionType.SubCommandGroup))
                    args = args.ElementAt(0)?.Options?.ToList();

                return await ExecuteAsync(context, Parameters, args, services);
            }
            else
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionCommandContext)} doesn't belong to a Slash Command Interaction");
        }

        public async Task<IResult> ExecuteAsync (IInteractionCommandContext context, IEnumerable<SlashCommandParameterInfo> paramList,
            IEnumerable<SocketSlashCommandDataOption> argList, IServiceProvider services)
        {
            object[] args = await GenerateArgs(context, paramList, argList, services).ConfigureAwait(false);

            return await RunAsync(context, args, services).ConfigureAwait(false);
        }

        private async Task<object[]> GenerateArgs (IInteractionCommandContext context, IEnumerable<SlashCommandParameterInfo> paramList,
            IEnumerable<SocketSlashCommandDataOption> options, IServiceProvider services)
        {
            if (paramList?.Count() < options?.Count())
                throw new InvalidOperationException("Command was invoked with too many parameters");

            var result = new List<object>();

            foreach (var parameter in paramList)
            {
                var arg = options?.FirstOrDefault(x => string.Equals(x.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));

                if (arg == null || arg == default)
                {
                    if (parameter.IsRequired)
                        throw new InvalidOperationException("Command was invoked with too few parameters");
                    else
                        result.Add(parameter.DefaultValue);
                }
                else
                {
                    var typeConverter = parameter.TypeConverter;

                    if (!typeConverter.CanConvertTo(parameter.ParameterType))
                        throw new InvalidOperationException($"Type {parameter.ParameterType.FullName} cannot be read by the registered {nameof(TypeConverter)}");

                    var readResult = await typeConverter.ReadAsync(context, arg, services).ConfigureAwait(false);

                    if (!readResult.IsSuccess)
                        throw new InvalidOperationException($"Argument was not read successfully: {readResult.ErrorReason}");

                    result.Add(readResult.Value);
                }
            }

            return result.ToArray();
        }

        protected override Task InvokeModuleEvent (IInteractionCommandContext context, IResult result)
            => CommandService._slashCommandExecutedEvent.InvokeAsync(this, context, result);

        protected override string GetLogString (IInteractionCommandContext context)
        {
            if (context.Guild != null)
                return $"Slash Command: \"{Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Slash Command: \"{Name}\" for {context.User} in {context.Channel}";
        }
    }
}
