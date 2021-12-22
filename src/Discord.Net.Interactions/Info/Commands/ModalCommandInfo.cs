using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public class ModalCommandInfo : CommandInfo<ModalCommandParameterInfo>
    {
        /// <inheritdoc/>
        public override bool SupportsWildCards => true;

        /// <inheritdoc/>
        public override IReadOnlyCollection<ModalCommandParameterInfo> Parameters { get; }

        internal ModalCommandInfo(Builders.ModalCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        }

        /// <inheritdoc/>
        public override Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services)
        {

        }

        /// <inheritdoc/>
        protected override Task InvokeModuleEvent(IInteractionContext context, IResult result)
            => CommandService._modalCommandExecutedEvent.InvokeAsync(this, context, result);

        /// <inheritdoc/>
        protected override string GetLogString(IInteractionContext context)
        {
            if (context.Guild != null)
                return $"Modal Command: \"{base.ToString()}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Modal Command: \"{base.ToString()}\" for {context.User} in {context.Channel}";
        }
    }
}
