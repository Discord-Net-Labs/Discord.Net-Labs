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
    public class InteractionInfo : ExecutableInfo
    {
        /// <summary>
        /// Get the information on Parameters that belong to this command
        /// </summary>
        public IReadOnlyList<ParameterInfo> Parameters { get; }
        /// <summary>
        /// Get the list of attributes of this command
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }

        public override bool SupportsWildCards => true;

        internal InteractionInfo (InteractionBuilder builder, ModuleInfo module, SlashCommandService commandService)
            :base(builder.Name, builder.IgnoreGroupNames, module, commandService, builder.Callback)
        {
            Parameters = builder.Parameters.ToImmutableArray();
            Attributes = builder.Attributes.ToImmutableArray();
        }

        public override async Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services)
            => await ExecuteAsync(context, services, null).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services, params string[] additionalArgs)
        {
            if (context.Interaction is SocketMessageComponent messageInteraction)
            {
                var args = new List<string>();

                if (additionalArgs != null)
                    args.AddRange(additionalArgs);

                if(messageInteraction.Data?.Values != null)
                    args.AddRange(messageInteraction.Data.Values);

                return await ExecuteAsync(context, Parameters, args, services);
            }
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

        private static object[] GenerateArgs (IEnumerable<ParameterInfo> paramList, IEnumerable<string> argList)
        {
            var result = new object[paramList.Count()];
            var index = 0;

            foreach (var parameter in paramList)
            {
                if (argList?.ElementAt(index) == null)
                {
                    if (parameter.IsOptional)
                        result[index] = Type.Missing;
                    else
                        throw new InvalidOperationException($"Interaction handler is executed with too few args.");
                }
                else if (parameter.CustomAttributes.Any(x => x.AttributeType == typeof(ParamArrayAttribute)))
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
