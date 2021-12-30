using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for handling Modal Interaction events.
    /// </summary>
    public class ModalCommandInfo : CommandInfo<ModalCommandParameterInfo>
    {
        /// <summary>
        ///     Gets the type of <see cref="IModal"/> this command uses.
        /// </summary>
        public Type ModalType { get; }

        /// <summary>
        ///     Gets a dictionary of the text input components in the modal, with the component's custom id as the key.
        /// </summary>
        public Dictionary<string, PropertyInfo> TextInputComponents { get; }

        /// <remarks>
        ///     This is only initialized when <see cref="InteractionService._useCompiledLambda"/> is <see langword="true"/>
        /// </remarks>
        private Func<object[], object> ModalInitializer { get; }

        /// <remarks>
        ///     This is only initialized when <see cref="InteractionService._useCompiledLambda"/> is <see langword="false"/>
        /// </remarks>
        private ConstructorInfo ModalCtor { get; }

        /// <inheritdoc/>
        public override bool SupportsWildCards => true;

        /// <inheritdoc/>
        public override IReadOnlyCollection<ModalCommandParameterInfo> Parameters { get; }

        internal ModalCommandInfo(Builders.ModalCommandBuilder builder, ModuleInfo module, InteractionService commandService) : base(builder, module, commandService)
        {
            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
            ModalType = Parameters.First().ParameterType;
            TextInputComponents = ModalType.GetProperties()
                .Where(x => x.GetCustomAttribute<ModalTextInputAttribute>() != null)
                .ToDictionary(x => x.GetCustomAttribute<ModalTextInputAttribute>().CustomId, x => x);
                
            if (commandService._useCompiledLambda)
                ModalInitializer = ReflectionUtils<object>.CreateLambdaConstructorInvoker(ModalType.GetTypeInfo());
            else
                ModalCtor = ModalType.GetConstructor(Array.Empty<Type>());
        }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services)
            => await ExecuteAsync(context, services, null).ConfigureAwait(false);

        /// <summary>
        ///     Execute this command using dependency injection.
        /// </summary>
        /// <param name="context">Context that will be injected to the <see cref="InteractionModuleBase{T}"/>.</param>
        /// <param name="services">Services that will be used while initializing the <see cref="InteractionModuleBase{T}"/>.</param>
        /// <param name="additionalArgs">Provide additional string parameters to the method along with the auto generated parameters.</param>
        /// <returns>
        ///     A task representing the asynchronous command execution process.
        /// </returns>
        public async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services, params string[] additionalArgs)
        {
            if (context.Interaction is not IModalInteraction interaction)
                return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Modal Interaction.");

            var modal = GetModal(interaction.Data.Components);

            List<object> args = new() { modal };
            
            if (additionalArgs is not null)
                args.AddRange(additionalArgs);

            return await RunAsync(context, args.ToArray(), services);
        }

        /// <summary>
        ///     Creates an <see cref="IModal"/> and fills it with provided message components.
        /// </summary>
        /// <param name="components"Components that will be injected into the modal.></param>
        /// <returns>A <see cref="IModal"/> filled with the provided components.</returns>
        public IModal GetModal(IEnumerable<IComponentInteractionData> components)
        {
            var modal = CommandService._useCompiledLambda
                ? (IModal)ModalInitializer.Invoke(Array.Empty<object>())
                : (IModal)ModalCtor.Invoke(null);

            foreach (var component in components)
            {
                switch (component.Type)
                {
                    case ComponentType.TextInput:
                        TextInputComponents.GetValueOrDefault(component.CustomId).SetValue(modal, component.Value);
                        break;
                };
            }

            return modal;
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
