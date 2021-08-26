using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Provides the information of a Slash Command
    /// </summary>
    public class SlashCommandInfo : ExecutableInfo, IApplicationCommandInfo
    {
        public string Description { get; }
        public ApplicationCommandType CommandType { get; } = ApplicationCommandType.Slash;
        /// <summary>
        /// Whether this command is executable by default
        /// </summary>
        public bool DefaultPermission { get; }
        /// <summary>
        /// Get the information on Parameters that belong to this command
        /// </summary>
        public IReadOnlyList<SlashParameterInfo> Parameters { get; }
        /// <summary>
        /// Get the list of attributes of this command
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }

        public override bool SupportsWildCards => false;

        internal SlashCommandInfo (Builders.SlashCommandBuilder builder, ModuleInfo module, SlashCommandService commandService)
            :base(builder.Name, builder.IgnoreGroupNames, module, commandService, builder.Callback)
        {
            Description = builder.Description;
            DefaultPermission = builder.DefaultPermission;
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
            Attributes = builder.Attributes.ToImmutableArray();
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services)
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
                return ExecuteResult.FromError(SlashCommandError.ParseFailed, $"Provided {nameof(ISlashCommandContext)} belongs to a message component");
        }

        public async Task<IResult> ExecuteAsync (ISlashCommandContext context, IEnumerable<SlashParameterInfo> paramList,
            IEnumerable<SocketSlashCommandDataOption> argList, IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            try
            {
                object[] args = await GenerateArgs(context, paramList, argList, services).ConfigureAwait(false);

                if (CommandService._runAsync)
                {
                    _ = Task.Run(async ( ) =>
                    {
                        await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);
                    });
                }
                else
                    return await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);

                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        private async Task<object[]> GenerateArgs (ISlashCommandContext context, IEnumerable<SlashParameterInfo> paramList,
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
                        result.Add(Type.Missing);
                }
                else
                {
                    var typeReader = parameter.TypeReader;

                    if (!typeReader.CanConvertTo(parameter.ParameterType))
                        throw new InvalidOperationException($"Type {nameof(parameter.ParameterType)} cannot be read by the registered Type Reader");

                    var readResult = await typeReader.ReadAsync(context, arg, services).ConfigureAwait(false);

                    if (!readResult.IsSuccess)
                        throw new InvalidOperationException($"Argument Read was not successful: {readResult.ErrorReason}");

                    result.Add(readResult.Value);
                }
            }

            return result.ToArray();
        }

        protected override Task InvokeModuleEvent (ISlashCommandContext context, IResult result)
            => CommandService._slashCommandExecutedEvent.InvokeAsync(this, context, result);

        protected override string GetLogString (ISlashCommandContext context)
        {
            if (context.Guild != null)
                return $"Slash Command: \"{Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Slash Command: \"{Name}\" for {context.User} in {context.Channel}";
        }
    }
}
