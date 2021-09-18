using Discord.SlashCommands.Builders;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Porvides the information of a Interaction handler
    /// </summary>
    public class InteractionInfo : CommandInfo<CommandParameterInfo>
    {
        /// <inheritdoc/>
        public override IReadOnlyCollection<CommandParameterInfo> Parameters { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => true;

        internal InteractionInfo (InteractionBuilder builder, ModuleInfo module, SlashCommandService commandService) : base(builder, module, commandService)
        {
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services)
            => await ExecuteAsync(context, services, null).ConfigureAwait(false);

        /// <summary>
        /// Execute this command using dependency injection
        /// </summary>
        /// <param name="context">Context that will be injected to the <see cref="SlashModuleBase{T}"/></param>
        /// <param name="services">Services that will be used while initializing the <see cref="SlashModuleBase{T}"/></param>
        /// <param name="additionalArgs">Provide additional string parameters to the method along with the auto generated parameters</param>
        /// <returns>A task representing the asyncronous command execution process</returns>
        public async Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services, params string[] additionalArgs)
        {
            if (context.Interaction is SocketMessageComponent messageInteraction)
            {
                var args = new List<string>();

                if (additionalArgs != null)
                    args.AddRange(additionalArgs);

                if (messageInteraction.Data?.Values != null)
                    args.AddRange(messageInteraction.Data.Values);

                return await ExecuteAsync(context, Parameters, args, services);
            }
            else
                throw new ArgumentException("Cannot execute command from the provided command context");
        }

        /// <inheritdoc/>
        public async Task<IResult> ExecuteAsync (ISlashCommandContext context, IEnumerable<CommandParameterInfo> paramList, IEnumerable<string> values,
            IServiceProvider services)
        {
            object[] args = GenerateArgs(paramList, values);

            return await RunAsync(context, args, services).ConfigureAwait(false);
        }

        private static object[] GenerateArgs (IEnumerable<CommandParameterInfo> paramList, IEnumerable<string> argList)
        {
            var result = new object[paramList.Count()];
            var index = 0;

            foreach (var parameter in paramList)
            {
                if (argList?.ElementAt(index) == null)
                {
                    if (!parameter.IsRequired)
                        result[index] = parameter.DefaultValue;
                    else
                        throw new InvalidOperationException($"Interaction handler is executed with too few args.");
                }
                else if (parameter.IsParameterArray)
                {
                    string[] paramArray = new string[argList.Count() - index];
                    argList.ToArray().CopyTo(paramArray, index);
                    result[index] = paramArray;
                }
                else
                    result[index] = argList?.ElementAt(index);

                index++;
            }

            return result;
        }

        protected override Task InvokeModuleEvent (ISlashCommandContext context, IResult result)
            => CommandService._interactionExecutedEvent.InvokeAsync(this, context, result);

        protected override string GetLogString (ISlashCommandContext context)
        {
            if (context.Guild != null)
                return $"Component Interaction: \"{Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Component Interaction: \"{Name}\" for {context.User} in {context.Channel}";
        }
    }
}
