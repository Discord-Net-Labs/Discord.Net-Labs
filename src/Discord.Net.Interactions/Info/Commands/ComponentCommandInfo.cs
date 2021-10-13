using Discord.Interactions.Builders;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for handling Component Interaction events
    /// </summary>
    public class ComponentCommandInfo : CommandInfo<CommandParameterInfo>
    {
        /// <inheritdoc/>
        public override IReadOnlyCollection<CommandParameterInfo> Parameters { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => true;

        internal ComponentCommandInfo (ComponentCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync (IInteractionCommandContext context, IServiceProvider services)
            => await ExecuteAsync(context, services, null).ConfigureAwait(false);

        /// <summary>
        ///     Execute this command using dependency injection
        /// </summary>
        /// <param name="context">Context that will be injected to the <see cref="InteractionModuleBase{T}"/></param>
        /// <param name="services">Services that will be used while initializing the <see cref="InteractionModuleBase{T}"/></param>
        /// <param name="additionalArgs">Provide additional string parameters to the method along with the auto generated parameters</param>
        /// <returns>
        ///     A task representing the asyncronous command execution process
        /// </returns>
        public async Task<IResult> ExecuteAsync (IInteractionCommandContext context, IServiceProvider services, params string[] additionalArgs)
        {
            if (context.Interaction is SocketMessageComponent messageInteraction)
            {
                var args = new List<string>();

                if (additionalArgs is not null)
                    args.AddRange(additionalArgs);

                if (messageInteraction.Data?.Values is not null)
                    args.AddRange(messageInteraction.Data.Values);

                return await ExecuteAsync(context, Parameters, args, services);
            }
            else
                throw new ArgumentException("Cannot execute Component Interaction handler from the provided command context");
        }

        /// <inheritdoc/>
        public async Task<IResult> ExecuteAsync (IInteractionCommandContext context, IEnumerable<CommandParameterInfo> paramList, IEnumerable<string> values,
            IServiceProvider services)
        {
            try
            {
                object[] args = GenerateArgs(paramList, values);

                return await RunAsync(context, args, services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        private static object[] GenerateArgs (IEnumerable<CommandParameterInfo> paramList, IEnumerable<string> argList)
        {
            var result = new object[paramList.Count()];

            for(var i = 0; i < paramList.Count(); i++)
            {
                var parameter = paramList.ElementAt(i);

                if (argList?.ElementAt(i) == null)
                {
                    if (!parameter.IsRequired)
                        result[i] = parameter.DefaultValue;
                    else
                        throw new InvalidOperationException($"Component Interaction handler is executed with too few args.");
                }
                else if (parameter.IsParameterArray)
                {
                    string[] paramArray = new string[argList.Count() - i];
                    argList.ToArray().CopyTo(paramArray, i);
                    result[i] = paramArray;
                }
                else
                    result[i] = argList?.ElementAt(i);
            }

            return result;
        }

        protected override Task InvokeModuleEvent (IInteractionCommandContext context, IResult result)
            => CommandService._componentCommandExecutedEvent.InvokeAsync(this, context, result);

        protected override string GetLogString (IInteractionCommandContext context)
        {
            if (context.Guild != null)
                return $"Component Interaction: \"{base.ToString()}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Component Interaction: \"{base.ToString()}\" for {context.User} in {context.Channel}";
        }
    }
}
