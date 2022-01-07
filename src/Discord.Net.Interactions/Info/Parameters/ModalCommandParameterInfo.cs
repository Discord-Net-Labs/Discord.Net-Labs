using Discord.Interactions.Builders;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the base parameter info class for <see cref="InteractionService"/> modals.
    /// </summary>
    public class ModalCommandParameterInfo : CommandParameterInfo
    {
        public ModalInfo Modal { get; private set; }

        public bool IsModalParameter => Modal is not null;

        /// <inheritdoc/>
        public new ModalCommandInfo Command => base.Command as ModalCommandInfo;

        internal ModalCommandParameterInfo(ModalCommandParameterBuilder builder, ICommandInfo command) : base(builder, command)
        {
            Modal = builder.Modal;
        }
    }

    public delegate IModal ModalParameterInitializer(object[] args);
}
