using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions
{
    public delegate IModal ModalInitializer(object[] args);

    public class ModalInfo
    {
        private readonly IReadOnlyDictionary<string, InputComponentInfo> _inputComponentDictionary;
        internal readonly ModalInitializer _initializer;

        public InteractionService InteractionService { get; }
        public string CustomId { get; }
        public string Title { get; }
        public IReadOnlyCollection<TextInputComponentInfo> TextComponents { get; }

        internal ModalInfo(Builders.ModalBuilder builder)
        {
            InteractionService = builder.InteractionService;
            CustomId = builder.CustomId;
            Title = builder.Title;
            TextComponents = builder.TextComponents.Select(x => x.Build(this)).ToImmutableArray();

            _initializer = builder.ModalInitializer;
            _inputComponentDictionary = TextComponents.ToDictionary(x => x.CustomId, x => (InputComponentInfo)x).ToImmutableDictionary();
        }

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
                        args[i] = Type.Missing;
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
