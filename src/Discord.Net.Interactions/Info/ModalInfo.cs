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
        internal readonly ModalInitializer _initializer;

        /// <summary>
        ///     Gets the title of this modal.
        /// </summary>
        public string Title { get; }

        public IReadOnlyCollection<InputComponentInfo> Components { get; }

        /// <summary>
        ///     Gets a collection of the text components of this modal.
        /// </summary>
        public IReadOnlyCollection<TextInputComponentInfo> TextComponents { get; }

        internal ModalInfo(Builders.ModalBuilder builder)
        {
            Title = builder.Title;
            Components = builder.Components.Select(x => x switch
            {
                Builders.TextInputComponentBuilder textComponent => textComponent.Build(this),
                _ => throw new InvalidOperationException($"{x.GetType().FullName} isn't a supported modal input component builder type.")
            }).ToImmutableArray();

            TextComponents = Components.Where(x => x is TextInputComponentInfo).Cast<TextInputComponentInfo>().ToImmutableArray();

            _initializer = builder.ModalInitializer;
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
            var args = new object[Components.Count];
            var components = modalInteraction.Data.Components.ToList();

            for (var i = 0; i < Components.Count; i++)
            {
                var input = Components.ElementAt(i);
                var component = components.Find(x => x.CustomId == input.CustomId);

                if (component is null)
                {
                    if (!input.IsRequired)
                        args[i] = input.DefaultValue;
                    else
                        throw new InvalidOperationException($"Modal interaction is missing the required field: {input.CustomId}");
                }
                else
                    args[i] = component.Value;
            }

            return _initializer(args);
        }
    }
}
