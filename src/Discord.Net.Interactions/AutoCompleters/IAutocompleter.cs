using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public interface IAutocompleter
    {
        Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionCommandContext context, SocketAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services);

        Task<IResult> ExecuteAsync(IInteractionCommandContext context, SocketAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services);
    }
}
