using Discord.Interactions.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the base parameter info class for <see cref="InteractionService"/> modals.
    /// </summary>
    public class ModalCommandParameterInfo : CommandParameterInfo
    {
        internal ModalParameterInitializer _modalParameterInitializer { get; }

        public IReadOnlyDictionary<string, Action<IModal, object>> TextInputComponents { get; }

        /// <inheritdoc/>
        public new ModalCommandInfo Command => base.Command as ModalCommandInfo;

        internal ModalCommandParameterInfo(ModalCommandParameterBuilder builder, ICommandInfo command) : base(builder, command)
        {
            _modalParameterInitializer = builder.ModalParameterInitializer;

            TextInputComponents = builder.TextInputComponents.ToImmutableDictionary();
        }
    }

    public delegate IModal ModalParameterInitializer(object[] args);
}
