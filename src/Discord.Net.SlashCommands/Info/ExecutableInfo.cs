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
        protected readonly Func<ISlashCommandContext, object[], IServiceProvider, ExecutableInfo, Task> _action;

        /// <summary>
        /// <see cref="SlashCommandService"/> this command belongs to
        /// </summary>
        public SlashCommandService CommandService { get; }
        /// <summary>
        /// Module this commands belongs to
        /// </summary>
        public ModuleInfo Module { get; }
        /// <summary>
        /// Get the name of this command that will be used to both execute and register this command
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// If true, this command will be registered and executed as a standalone command, unaffected by the <see cref="SlashGroupAttribute"/>s of its declaring types
        /// </summary>
        public bool IgnoreGroupNames { get; }

        public abstract bool SupportsWildCards { get; }

        internal ExecutableInfo (string name, bool ignoreGroupNames, ModuleInfo module, SlashCommandService commandService,
            Func<ISlashCommandContext, object[], IServiceProvider, ExecutableInfo, Task> Callback)
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
