using Discord.SlashCommands.Builders;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Porvides the information of a Interaction handler
    /// </summary>
    public class SlashInteractionInfo : IExecutableInfo
    {
        private readonly Func<ISlashCommandContext, object[], IServiceProvider, SlashInteractionInfo, Task> _action;

        /// <summary>
        /// <see cref="SlashCommandService"/> this command belongs to
        /// </summary>
        public SlashCommandService CommandService { get; }
        public string Name { get; }
        /// <inheritdoc/>
        public SlashGroupInfo Group { get; }
        /// <inheritdoc/>
        public SlashModuleInfo Module { get; }
        /// <summary>
        /// Get the information on Parameters that belong to this command
        /// </summary>
        public IReadOnlyList<ParameterInfo> Parameters { get; }
        /// <summary>
        /// Get the list of attributes of this command
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }

        internal SlashInteractionInfo (SlashInteractionBuilder builder, SlashModuleInfo module, SlashCommandService commandService)
        {
            CommandService = commandService;
            Module = module;

            Name = builder.Name;
            Group = builder.Group;
            Parameters = builder.Parameters.ToImmutableArray();
            Attributes = builder.Attributes.ToImmutableArray();

            _action = builder.Callback;
        }

        /// <inheritdoc/>
        public async Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services)
        {
            if (context.Interaction is SocketMessageInteraction messageInteraction)
                return await ExecuteAsync(context, Parameters, messageInteraction.Values, services);
            else
                throw new ArgumentException("Cannot execute command from the provided command context");
        }

        public async Task<IResult> ExecuteAsync (ISlashCommandContext context, IEnumerable<ParameterInfo> paramList, IEnumerable<string> values,
            IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            try
            {
                object[] args = GenerateArgs(paramList, values);

                _ = Task.Run(async ( ) =>
                {
                    await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);
                });
                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        private async Task<IResult> ExecuteInternalAsync (ISlashCommandContext context, object[] args, IServiceProvider services)
        {
            await Module.CommandService._cmdLogger.DebugAsync($"Executing {GetLogString(context)}").ConfigureAwait(false);

            try
            {
                var task = _action.Invoke(context, args, services, this);
                await task.ConfigureAwait(false);
                var result = ExecuteResult.FromSuccess();
                await Module.CommandService._interactionExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
            finally
            {
                await Module.CommandService._cmdLogger.VerboseAsync($"Executed {GetLogString(context)}").ConfigureAwait(false);
            }
        }

        private static object[] GenerateArgs (IEnumerable<ParameterInfo> paramList, IEnumerable<string> argList)
        {
            var result = new object[paramList.Count()];
            var index = 0;

            foreach (var parameter in paramList)
            {
                if (argList?.ElementAt(index) == null)
                    result[index] = Type.Missing;
                else if (parameter.CustomAttributes.Any(x => x.AttributeType == typeof(ParamArrayAttribute)))
                {
                    string[] paramArray = new string[argList.Count() - index];
                    argList.ToArray().CopyTo(paramArray, index);
                    result[index] = paramArray;
                }
                else
                    result[index] = argList?.ElementAt(index);
            }

            return result;
        }

        private string GetLogString (ISlashCommandContext context)
        {
            if (context.Guild != null)
                return $"\"{Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"\"{Name}\" for {context.User} in {context.Channel}";
        }
    }
}
