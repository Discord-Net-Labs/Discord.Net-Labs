using Discord.WebSocket;
using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public delegate Task<AutocompletionResult> AutocompleteCallback(IInteractionCommandContext context, IServiceProvider services, IParameterInfo parameterInfo);

    public abstract class Autocompleter : IAutocompleter
    {
        public InteractionService InteractionService { get; set; }

        public abstract Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionCommandContext context, SocketAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services);

        protected abstract string GetLogString(IInteractionCommandContext context);

        public async Task<IResult> ExecuteAsync(IInteractionCommandContext context, SocketAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services)
        {
            switch (InteractionService._runMode)
            {
                case RunMode.Sync:
                    {
                        return await ExecuteInternalAsync(context, autocompleteInteraction, parameter, services).ConfigureAwait(false);
                    }
                case RunMode.Async:
                    _ = Task.Run(async () =>
                    {
                        await ExecuteInternalAsync(context, autocompleteInteraction, parameter, services).ConfigureAwait(false);
                    });
                    break;
                default:
                    throw new InvalidOperationException($"RunMode {InteractionService._runMode} is not supported.");
            }

            return ExecuteResult.FromSuccess();
        }

        private async Task<IResult> ExecuteInternalAsync(IInteractionCommandContext context, SocketAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services)
        {
            try
            {
                var result = await GenerateSuggestionsAsync(context, autocompleteInteraction, parameter, services).ConfigureAwait(false);

                if (result.IsSuccess)
                    await autocompleteInteraction.RespondAsync(result.Suggestions).ConfigureAwait(false);

                await InteractionService._autocompleterExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                var originalEx = ex;
                while (ex is TargetInvocationException)
                    ex = ex.InnerException;

                await InteractionService._cmdLogger.ErrorAsync(ex).ConfigureAwait(false);

                var result = ExecuteResult.FromError(ex);
                await InteractionService._autocompleterExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);

                if (InteractionService._throwOnError)
                {
                    if (ex == originalEx)
                        throw;
                    else
                        ExceptionDispatchInfo.Capture(ex).Throw();
                }

                return result;
            }
            finally
            {
                await InteractionService._cmdLogger.VerboseAsync($"Executed {GetLogString(context)}").ConfigureAwait(false);
            }
        }
    }
}


