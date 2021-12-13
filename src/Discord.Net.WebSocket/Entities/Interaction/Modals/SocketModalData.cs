using System.Collections.Generic;
using System.Linq;
using System;
using Model = Discord.API.ModalInteractionData;
using InterationModel = Discord.API.Interaction;
using DataModel = Discord.API.MessageComponentInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents data sent from a <see cref="InteractionType.ModalSubmit"/>.
    /// </summary>
    public class SocketModalData : IComponentInteractionData
    {
        /// <inheritdoc/>
        public string CustomId { get; }

        /// <summary>
        ///     Represents the <see cref="Modal"/>s components submitted by the user.
        /// </summary>
        public IReadOnlyCollection<SocketMessageComponentData> Components { get; }

        /// <inheritdoc/>
        public ComponentType Type => ComponentType.ModalSubmit;

        /// <inheritdoc/>
        [Obsolete("Modal interactions do not have values!", true)]
        public IReadOnlyCollection<string> Values
            => throw new NotSupportedException("Modal interactions do not have values!");

        [Obsolete("Modal interactions do not have value!", true)]
        public string Value
            => throw new NotSupportedException("Modal interactions do not have value!");

        internal SocketModalData(Model model)
        {
            CustomId = model.CustomId;
            Components = model.Components
                .SelectMany(x => x.Components)
                .Select(x => new SocketMessageComponentData(x))
                .ToArray();
                
        }
    }
}
