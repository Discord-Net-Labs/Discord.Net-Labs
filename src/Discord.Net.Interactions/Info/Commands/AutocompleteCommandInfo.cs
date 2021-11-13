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
    ///     Represents the info class of an attribute based method for handling Autocomplete Interaction events
    /// </summary>
    public sealed class AutocompleteCommandInfo : CommandInfo<CommandParameterInfo>
    {
        /// <summary>
        ///     Name of the target parameter
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        ///     Name of the target command
        /// </summary>
        public string CommandName { get; }

        /// <inheritdoc/>
        public override IReadOnlyCollection<CommandParameterInfo> Parameters { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => false;

        internal AutocompleteCommandInfo(AutocompleteCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
            ParameterName = builder.ParameterName;
            CommandName = builder.CommandName;
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services)
        {
            if (context.Interaction is not SocketAutocompleteInteraction messageComponent)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Autocomplete Interaction");

            try
            {
                return await RunAsync(context, Array.Empty<object>(), services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        /// <inheritdoc/>
        protected override Task InvokeModuleEvent(IInteractionContext context, IResult result) =>
            CommandService._autocompleteCommandExecutedEvent.InvokeAsync(this, context, result);

        /// <inheritdoc/>
        protected override string GetLogString(IInteractionContext context)
        {
            if (context.Guild != null)
                return $"Slash Command: \"{base.ToString()}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Slash Command: \"{base.ToString()}\" for {context.User} in {context.Channel}";
        }

        internal string[] GetCommandKeywords()
        {
            var keywords = new List<string>() { ParameterName, CommandName };

            var currentParent = Module;

            while (currentParent != null)
            {
                if (!string.IsNullOrEmpty(currentParent.SlashGroupName))
                    keywords.Add(currentParent.SlashGroupName);

                currentParent = currentParent.Parent;
            }

            keywords.Reverse();

            return keywords.ToArray();
        }
    }
}
