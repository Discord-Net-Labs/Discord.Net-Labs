using Discord.Interactions.Builders;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the base parameter info class for <see cref="InteractionService"/> modals.
    /// </summary>
    public class ModalCommandParameterInfo : CommandParameterInfo
    {
        internal ModalParameterInitializer _modalParameterInitializer { get; }

        /// <inheritdoc/>
        public new ModalCommandInfo Command => base.Command as ModalCommandInfo;

        internal ModalCommandParameterInfo(ModalCommandParameterBuilder builder, ICommandInfo command) : base(builder, command)
        {
            _modalParameterInitializer = builder.ModalParameterInitializer;
        }
    }

    public delegate IModal ModalParameterInitializer(object[] args);
}
