using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents the info class of an executable command 
    /// </summary>
    public abstract class ExecutableInfo : IExecutableInfo
    {
        public delegate Task ExecuteCallback (ISlashCommandContext context, object[] args, IServiceProvider serviceProvider, ExecutableInfo commandInfo);
        protected readonly ExecuteCallback _action;

        /// <inheritdoc/>
        public SlashCommandService CommandService { get; }

        /// <inheritdoc/>
        public ModuleInfo Module { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string MethodName { get; }

        /// <inheritdoc/>
        public bool IgnoreGroupNames { get; }

        /// <inheritdoc/>
        public abstract bool SupportsWildCards { get; }

        /// <inheritdoc/>
        public bool IsTopLevel => IgnoreGroupNames || !Module.IsTopLevel;

        internal ExecutableInfo (string name, bool ignoreGroupNames, ModuleInfo module, SlashCommandService commandService, ExecuteCallback Callback)
        {
            Name = name;
            IgnoreGroupNames = ignoreGroupNames;
            Module = module;
            CommandService = commandService;

            _action = Callback;
        }

        /// <summary>
        /// Execute this command using dependency injection
        /// </summary>
        /// <param name="context">Context that will be injected to the <see cref="SlashModuleBase{T}"/></param>
        /// <param name="services">Services that will be used while initializing the <see cref="SlashModuleBase{T}"/></param>
        /// <returns>A task representing the asyncronous command execution process</returns>
        public abstract Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services);

        protected async Task<IResult> ExecuteInternalAsync (ISlashCommandContext context, object[] args, IServiceProvider services)
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


                return ExecuteResult.FromError(SlashCommandError.Unsuccessful, "Command execution failed for an unknown reason");
            }
            catch (Exception ex)
            {
                var originalEx = ex;
                while (ex is TargetInvocationException)
                    ex = ex.InnerException;

                await Module.CommandService._cmdLogger.ErrorAsync(ex);

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

        protected abstract Task InvokeModuleEvent (ISlashCommandContext context, IResult result);
        protected abstract string GetLogString (ISlashCommandContext context);
    }
}
