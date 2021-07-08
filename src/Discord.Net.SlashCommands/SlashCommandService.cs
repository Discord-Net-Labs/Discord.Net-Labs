using Discord.API;
using Discord.API.Rest;
using Discord.Logging;
using Discord.SlashCommands.Builders;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Provides the framework for self registering and self-executing Discord Application Commands
    /// </summary>
    public class SlashCommandService : IDisposable
    {
        /// <summary>
        /// Occurs when a Slash Command related information is recived
        /// </summary>
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        /// <summary>
        /// Occur when a Slash Command is executed
        /// </summary>
        public event Func<Optional<SlashCommandInfo>, ISlashCommandContext, IResult, Task> CommandExecuted { add { _commandExecutedEvent.Add(value); } remove { _commandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<Optional<SlashCommandInfo>, ISlashCommandContext, IResult, Task>> _commandExecutedEvent = new AsyncEvent<Func<Optional<SlashCommandInfo>, ISlashCommandContext, IResult, Task>>();

        /// <summary>
        /// Occurs when a Message Component Interaction is handled
        /// </summary>
        public event Func<Optional<SlashInteractionInfo>, ISlashCommandContext, IResult, Task> InteractionExecuted { add { _interactionExecutedEvent.Add(value); } remove { _interactionExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<Optional<SlashInteractionInfo>, ISlashCommandContext, IResult, Task>> _interactionExecutedEvent = new AsyncEvent<Func<Optional<SlashInteractionInfo>, ISlashCommandContext, IResult, Task>>();


        private readonly ConcurrentDictionary<Type, SlashModuleInfo> _typedModuleDefs;
        private readonly SlashCommandMap<SlashCommandInfo> _commandMap;
        private readonly SlashCommandMap<SlashInteractionInfo> _interactionCommandMap;
        private readonly HashSet<SlashModuleInfo> _moduleDefs;
        private readonly SemaphoreSlim _lock;
        private readonly ulong _applicationId;
        private readonly ConcurrentDictionary<ApplicationCommandOptionType, Func<ISlashCommandContext, InteractionParameter, IServiceProvider, object>> _typeReaders;
        internal readonly Logger _cmdLogger;
        internal readonly LogManager _logManager;

        internal readonly bool _runAsync, _throwOnError;

        /// <summary>
        /// Represents all of the modules that are loaded in the <see cref="SlashCommandService"/>
        /// </summary>
        public IReadOnlyList<SlashModuleInfo> Modules => _moduleDefs.ToList();

        /// <summary>
        /// Represents all of the executeable commands that are loaded in the <see cref="SlashCommandService"/> modules
        /// </summary>
        public IReadOnlyList<SlashCommandInfo> Commands => _moduleDefs.SelectMany(x => x.Commands).ToList();

        /// <summary>
        /// Represents all of the Interaction handlers that are loaded in the <see cref="SlashCommandService"/>
        /// </summary>
        public IReadOnlyCollection<SlashInteractionInfo> Interacions => _moduleDefs.SelectMany(x => x.Interactions).ToList();

        public IReadOnlyDictionary<ApplicationCommandOptionType, Func<ISlashCommandContext, InteractionParameter, IServiceProvider, object>> TypeReaders => _typeReaders;

        /// <summary>
        /// Client that the Application Commands will be registered for
        /// </summary>
        public BaseSocketClient Client { get; }

        /// <summary>
        /// Initialize a <see cref="SlashCommandService"/> with the default configurations
        /// </summary>
        /// <param name="discord">The client that will be used to register commands</param>
        public SlashCommandService (BaseSocketClient discord) : this(discord, new SlashCommandServiceConfig()) { }

        /// <summary>
        /// Initialize a <see cref="SlashCommandService"/> with configurations from a provided <see cref="SlashCommandServiceConfig"/>
        /// </summary>
        /// <param name="discord">The client that will be used to register commands</param>
        /// <param name="config">The configuration class</param>
        public SlashCommandService (BaseSocketClient discord, SlashCommandServiceConfig config)
        {
            _lock = new SemaphoreSlim(1, 1);
            _typedModuleDefs = new ConcurrentDictionary<Type, SlashModuleInfo>();
            _moduleDefs = new HashSet<SlashModuleInfo>();
            _typeReaders = new ConcurrentDictionary<ApplicationCommandOptionType, Func<ISlashCommandContext, InteractionParameter, IServiceProvider, object>>();

            _logManager = new LogManager(LogSeverity.Debug);
            _logManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
            _cmdLogger = _logManager.CreateLogger("Command");

            _commandMap = new SlashCommandMap<SlashCommandInfo>(this);
            _interactionCommandMap = new SlashCommandMap<SlashInteractionInfo>(this);

            Client = discord;
            _applicationId = Client.GetApplicationInfoAsync().GetAwaiter().GetResult().Id;

            _runAsync = config.RunAsync;
            _throwOnError = config.ThrowOnError;

            DefaultReaders.CreateDefaultTypeReaders(_typeReaders);
        }

        /// <summary>
        /// Discover and load <see cref="CommandBase{T}"/> from a given assembly
        /// </summary>
        /// <param name="assembly"><see cref="Assembly"/> the command modules are defined in</param>
        /// <param name="services"><see cref="IServiceProvider"/> to be used when instantiating a command module</param>
        /// <returns>Module information for the <see cref="CommandBase{T}"/> types that are loaded to <see cref="SlashCommandService"/></returns>
        public async Task<IEnumerable<SlashModuleInfo>> AddModules (Assembly assembly, IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            await _lock.WaitAsync().ConfigureAwait(false);

            try
            {
                var types = await ModuleClassBuilder.SearchAsync(assembly, this);
                var moduleDefs = await ModuleClassBuilder.BuildAsync(types, this, services);

                foreach (var info in moduleDefs)
                {
                    _typedModuleDefs[info.Key] = info.Value;
                    LoadModuleInternal(info.Value);
                }
                return moduleDefs.Values;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Register and update the Application Commands from <see cref="SlashCommandService.Commands"/> while deleting the missing commands
        /// </summary>
        /// <param name="guild">Optional guild parameter, if defined, the commands are registered as guild commands for the provide guild, else commands are
        /// registered as global commands</param>
        /// <param name="deleteMissing">If true, delete all of the commands that are not registered in the <see cref="SlashCommandService"/></param>
        /// <returns></returns>
        public async Task SyncCommands (IGuild guild = null, bool deleteMissing = true)
        {
            DiscordRestApiClient restClient = Client.ApiClient;

            var creationParams = new List<CreateApplicationCommandParams>();

            foreach (var module in Modules)
            {
                if (string.IsNullOrEmpty(module.Name))
                {
                    var args = module.Commands.AsEnumerable().GroupParseApplicationCommandParams();
                    creationParams.AddRange(args);
                }
                else
                {
                    if (module.TryParseApplicationCommandParams(out var args))
                        creationParams.Add(args);
                }
            }

            if (deleteMissing)
            {
                var existing = await Rest.SlashCommandHelper.GetApplicationCommands(Client, guild, null);

                var missing = existing.Where(x => !creationParams.Any(y => y.Name == x.Name));

                if (missing != null)
                    foreach (var command in missing)
                    {
                        await Rest.SlashCommandHelper.DeleteApplicationCommand(Client, command.Id, guild, null).ConfigureAwait(false);
                        existing.ToList().Remove(command);
                    }
            }

            foreach (var args in creationParams)
            {
                ApplicationCommand result;

                if (guild != null)
                    result = await restClient.CreateGuildApplicationCommand(_applicationId, guild.Id, args).ConfigureAwait(false);
                else
                    result = await restClient.CreateGlobalApplicationCommand(_applicationId, args).ConfigureAwait(false);

                if (result == null)
                    await _cmdLogger.WarningAsync($"Command could not be registered ({args.Name})").ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Register a set of commands as "Guild Commands" to a guild
        /// </summary>
        /// <remarks>
        /// Commands will be registered as standalone commands, if you want the <see cref="SlashGroupAttribute"/> to take effect,
        /// use <see cref="AddModulesToGuild(IGuild, SlashModuleInfo[])"/>
        /// </remarks>
        /// <param name="guild">Guild the commands will be registered to</param>
        /// <param name="commands">Commands that will be registered</param>
        /// <returns></returns>
        public async Task AddCommandsToGuild (IGuild guild, params SlashCommandInfo[] commands)
        {
            if (guild == null)
                throw new ArgumentException($"{nameof(guild)} cannot be null to call this function.");

            foreach (var com in commands)
            {
                ApplicationCommand result = await Client.ApiClient.CreateGuildApplicationCommand(_applicationId, guild.Id,
                    com.ParseApplicationCommandParams(), null);

                if (result == null)
                    await _cmdLogger.WarningAsync($"Command could not be registered ({com.Name})").ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Register a set of modules as "Guild Commands" to a guild
        /// </summary>
        /// <param name="guild">Guild the commands will be registered to</param>
        /// <param name="modules">Modules that will be registered</param>
        /// <returns></returns>
        public async Task AddModulesToGuild (IGuild guild, params SlashModuleInfo[] modules)
        {
            if (guild == null)
                throw new ArgumentException($"{nameof(guild)} cannot be null to call this function.");

            foreach (var module in Modules)
            {
                if (module.TryParseApplicationCommandParams(out var args))
                {
                    ApplicationCommand result = await Client.ApiClient.CreateGuildApplicationCommand(_applicationId, guild.Id, args, null);

                    if (result == null)
                        await _cmdLogger.WarningAsync($"Module could not be registered ({module.Name})").ConfigureAwait(false);
                }
            }
        }

        private void LoadModuleInternal (SlashModuleInfo module)
        {
            _moduleDefs.Add(module);

            foreach (var command in module.Commands)
                _commandMap.AddCommand(command);

            foreach (var internalCommand in module.Interactions)
                _interactionCommandMap.AddCommand(internalCommand);
        }

        /// <summary>
        /// Remove a loaded module from <see cref="SlashCommandService.Modules"/>
        /// </summary>
        /// <param name="type"><see cref="CommandBase{T}"/> that will be removed</param>
        /// <returns></returns>
        public async Task<bool> RemoveModuleAsync (Type type)
        {
            await _lock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!_typedModuleDefs.TryRemove(type, out var module))
                    return false;

                return RemoveModuleInternal(module);
            }
            finally
            {
                _lock.Release();
            }
        }

        private bool RemoveModuleInternal (SlashModuleInfo moduleInfo)
        {
            if (!_moduleDefs.Remove(moduleInfo))
                return false;


            foreach (var command in moduleInfo.Commands)
            {
                _commandMap.RemoveCommand(command);
            }

            return true;
        }

        /// <summary>
        /// Execute a command from a given <see cref="ISlashCommandContext"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="ISlashCommandContext.Interaction"/>
        /// must be type of <see cref="SocketCommandInteraction"/></param>
        /// <param name="input">Command string that will be used to parse the <see cref="SlashCommandInfo"/></param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns></returns>
        public async Task ExecuteCommandAsync (ISlashCommandContext context, string[] input, IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            var command = _commandMap.GetCommands(string.Join(" ", input)).First();

            if (command == null)
            {
                await _cmdLogger.DebugAsync($"Unknown slash command, skipping execution ({string.Join(" ", input).ToUpper()})");
                return;
            }
            await command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        /// <summary>
        /// Use to execute an Interaction Handler from a <see cref="IDiscordInteractable.CustomId"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="ISlashCommandContext.Interaction"/>
        /// must be type of <see cref="SocketMessageInteraction"/></param>
        /// <param name="input">String that will be used to parse the <see cref="SlashInteractionInfo"/>,
        /// set the <see cref="IDiscordInteractable.CustomId"/> of a message component the same as the <see cref="InteractionAttribute.CustomId"/> to handle
        /// Message Component Interactions automatically</param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns></returns>
        public async Task ExecuteInteractionAsync (ISlashCommandContext context, string input, IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            var command = _interactionCommandMap.GetCommands(input).First();

            if (command == null)
            {
                await _cmdLogger.DebugAsync($"Unknown custom interaction id, skipping execution ({input.ToUpper()})");
                return;
            }
            await command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        /// <summary>
        /// Replace a default type reader
        /// </summary>
        /// <remarks>
        /// Must be used before the module discovery
        /// </remarks>
        /// <param name="discordParamType">The Application Commands API type this reader will be used for</param>
        /// <param name="reader">Type Reader function</param>
        public void ReplaceTypeReader (ApplicationCommandOptionType discordParamType, Func<ISlashCommandContext, InteractionParameter, IServiceProvider, object> reader) =>
            _typeReaders[discordParamType] = reader;

        public void Dispose ( ) => throw new NotImplementedException();
    }
}
