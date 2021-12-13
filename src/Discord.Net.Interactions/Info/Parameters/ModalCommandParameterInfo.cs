using Discord.Interactions.Builders;

namespace Discord.Interactions
{
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
