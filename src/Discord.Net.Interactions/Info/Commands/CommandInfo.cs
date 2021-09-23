using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    /// Represents a cached method execution delegate
    /// </summary>
    /// <param name="context">Execution context that will be injected to the module class</param>
    /// <param name="args">Method arguments array</param>
    /// <param name="serviceProvider">Service collection for initializing the module</param>
    /// <param name="commandInfo">Command info class of the executed method</param>
    /// <returns>A task representing the execution operation</returns>
    internal delegate Task ExecuteCallback (IInteractionCommandContext context, object[] args, IServiceProvider serviceProvider, ICommandInfo commandInfo);

    /// <summary>
    /// The base information class for <see cref="InteractionService"/> commands
    /// </summary>
    /// <typeparam name="TParameter">The type of <see cref="IParameterInfo"/> that is used by this command type</typeparam>
    public abstract class CommandInfo<TParameter> : ICommandInfo where TParameter : class, IParameterInfo
    {
        private readonly ExecuteCallback _action;

        internal ILookup<string, PreconditionAttribute> _groupedPreconditions { get; }

        /// <inheritdoc/>
        public ModuleInfo Module { get; }

        /// <inheritdoc/>
        public InteractionService CommandService { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string MethodName { get; }

        /// <inheritdoc/>
        public virtual bool IgnoreGroupNames { get; }

        /// <inheritdoc/>
        public abstract bool SupportsWildCards { get; }

        /// <inheritdoc/>
        public bool IsTopLevelCommand => IgnoreGroupNames || !Module.IsTopLevelGroup;

        /// <inheritdoc/>
        public RunMode RunMode { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <inheritdoc cref="ICommandInfo.Parameters"/>
        public abstract IReadOnlyCollection<TParameter> Parameters { get; }

        /// <inheritdoc/>
        IReadOnlyCollection<IParameterInfo> ICommandInfo.Parameters => Parameters;

        internal CommandInfo (Builders.ICommandBuilder builder, ModuleInfo module, InteractionService commandService)
        {
            CommandService = commandService;
            Module = module;

            Name = builder.Name;
            MethodName = builder.MethodName;
            IgnoreGroupNames = builder.IgnoreGroupNames;
            RunMode = builder.RunMode != RunMode.Default ? builder.RunMode : commandService._runMode;
            Attributes = builder.Attributes.ToImmutableArray();
            Preconditions = builder.Preconditions.ToImmutableArray();

            _action = builder.Callback;
            _groupedPreconditions = builder.Preconditions.ToLookup(x => x.Group, x => x, StringComparer.Ordinal);
        }

        /// <inheritdoc/>
        public abstract Task<IResult> ExecuteAsync (IInteractionCommandContext context, IServiceProvider services);
        protected abstract Task InvokeModuleEvent (IInteractionCommandContext context, IResult result);
        protected abstract string GetLogString (IInteractionCommandContext context);

        /// <inheritdoc/>
        public async Task<PreconditionResult> CheckPreconditionsAsync (IInteractionCommandContext context, IServiceProvider services)
        {
            async Task<PreconditionResult> CheckGroups (ILookup<string, PreconditionAttribute> preconditions, string type)
            {
                foreach (IGrouping<string, PreconditionAttribute> preconditionGroup in preconditions)
                {
                    if (preconditionGroup.Key == null)
                    {
                        foreach (PreconditionAttribute precondition in preconditionGroup)
                        {
                            var result = await precondition.CheckRequirementsAsync(context, this, services).ConfigureAwait(false);
                            if (!result.IsSuccess)
                                return result;
                        }
                    }
                    else
                    {
                        var results = new List<PreconditionResult>();
                        foreach (PreconditionAttribute precondition in preconditionGroup)
                            results.Add(await precondition.CheckRequirementsAsync(context, this, services).ConfigureAwait(false));

                        if (!results.Any(p => p.IsSuccess))
                            return PreconditionGroupResult.FromError($"{type} precondition group {preconditionGroup.Key} failed.", results);
                    }
                }
                return PreconditionGroupResult.FromSuccess();
            }

            foreach(var parameter in Parameters)
            {
                var result = await parameter.CheckPreconditionsAsync(context, services).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            var moduleResult = await CheckGroups(Module._groupedPreconditions, "Module").ConfigureAwait(false);
            if (!moduleResult.IsSuccess)
                return moduleResult;

            var commandResult = await CheckGroups(_groupedPreconditions, "Command").ConfigureAwait(false);
            if (!commandResult.IsSuccess)
                return commandResult;

            return PreconditionResult.FromSuccess();
        }

        protected async Task<IResult> RunAsync (IInteractionCommandContext context, object[] args, IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            var result = await CheckPreconditionsAsync(context, services).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                await InvokeModuleEvent(context, result).ConfigureAwait(false);
                return result;
            }

            try
            {
                switch (RunMode)
                {
                    case RunMode.Sync:
                        return await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);
                    case RunMode.Async:
                        _ = Task.Run(async ( ) =>
                        {
                            await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);
                        });
                        break;
                    default:
                        throw new InvalidOperationException($"RunMode {RunMode} is not supported.");
                }

                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        private async Task<IResult> ExecuteInternalAsync (IInteractionCommandContext context, object[] args, IServiceProvider services)
        {
            await CommandService._cmdLogger.DebugAsync($"Executing {GetLogString(context)}").ConfigureAwait(false);

            try
            {
                var task = _action(context, args, services, this);

                if (task is Task<IResult> resultTask)
                {
                    var result = await resultTask.ConfigureAwait(false);
                    await InvokeModuleEvent(context, result).ConfigureAwait(false);
                    if (result is RuntimeResult || result is ExecuteResult)
                        return result;
                }
                else
                {
                    await task.ConfigureAwait(false);
                    var result = ExecuteResult.FromSuccess();
                    await InvokeModuleEvent(context, result).ConfigureAwait(false);
                    return result;
                }


                return ExecuteResult.FromError(InteractionCommandError.Unsuccessful, "Command execution failed for an unknown reason");
            }
            catch (Exception ex)
            {
                var originalEx = ex;
                while (ex is TargetInvocationException)
                    ex = ex.InnerException;

                await Module.CommandService._cmdLogger.ErrorAsync(ex).ConfigureAwait(false);

                var result = ExecuteResult.FromError(ex);
                await InvokeModuleEvent(context, result).ConfigureAwait(false);

                if (Module.CommandService._throwOnError)
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
                await CommandService._cmdLogger.VerboseAsync($"Executed {GetLogString(context)}").ConfigureAwait(false);
            }
        }
    }
}
