using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a cached object initialization delegate.
    /// </summary>
    /// <param name="args">Property arguments array.</param>
    /// <returns>
    ///     Returns the constructed object.
    /// </returns>
    public delegate IModal ModalInitializer(object[] args);

    /// <summary>
    ///     Represents the info class of an <see cref="IModal"/> form.
    /// </summary>
    public class ModalInfo
    {
        private readonly IReadOnlyDictionary<string, InputComponentInfo> _inputComponentDictionary;
        internal readonly ModalInitializer _initializer;

        /// <summary>
        ///     Gets the title of this modal.
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     Gets a collection of the text components of this modal.
        /// </summary>
        public IReadOnlyCollection<TextInputComponentInfo> TextComponents { get; }

        internal ModalInfo(Builders.ModalBuilder builder)
        {
            Title = builder.Title;
            TextComponents = builder.TextComponents.Select(x => x.Build(this)).ToImmutableArray();

            _initializer = builder.ModalInitializer;
            _inputComponentDictionary = TextComponents.ToDictionary(x => x.CustomId, x => (InputComponentInfo)x).ToImmutableDictionary();
        }

        /// <summary>
        ///     Creates an <see cref="IModal"/> and fills it with provided message components.
        /// </summary>
        /// <param name="components"><see cref="IModalInteraction"/> that will be injected into the modal.</param>
        /// <returns>
        ///     A <see cref="IModal"/> filled with the provided components.
        /// </returns>
        public IModal CreateModal(IModalInteraction modalInteraction)
        {
            var args = new object[_inputComponentDictionary.Count];
            var components = modalInteraction.Data.Components.ToList();

            for (var i = 0; i < _inputComponentDictionary.Count; i++)
            {
                var input = _inputComponentDictionary.ElementAt(i);
                var component = components.Find(x => x.CustomId == input.Value.CustomId);

                if (component is null)
                {
                    if (!input.Value.IsRequired)
                        args[i] = input.Value.DefaultValue;
                    else
                        throw new InvalidOperationException($"Modal interaction is missing the required field: {input.Key}");
                }
                else
                    args[i] = component.Value;
            }

            return _initializer(args);
        }
    }
}
