using Discord.API;
using Discord.API.Rest;
using Discord.Logging;
using Discord.Rest;
using Discord.SlashCommands.Builders;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        /// Occurs when a Slash Command related information is recieved
        /// </summary>
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        /// <summary>
        /// Occurs when a Slash Command is executed
        /// </summary>
        public event Func<SlashCommandInfo, ISlashCommandContext, IResult, Task> SlashCommandExecuted { add { _slashCommandExecutedEvent.Add(value); } remove { _slashCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<SlashCommandInfo, ISlashCommandContext, IResult, Task>> _slashCommandExecutedEvent = new AsyncEvent<Func<SlashCommandInfo, ISlashCommandContext, IResult, Task>>();

        /// <summary>
        /// Occurs when a Context Command is executed
        /// </summary>
        public event Func<ContextCommandInfo, ISlashCommandContext, IResult, Task> ContextCommandExecuted { add { _contextCommandExecutedEvent.Add(value); } remove { _contextCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<ContextCommandInfo, ISlashCommandContext, IResult, Task>> _contextCommandExecutedEvent = new AsyncEvent<Func<ContextCommandInfo, ISlashCommandContext, IResult, Task>>();

        /// <summary>
        /// Occurs when a Message Component command is executed
        /// </summary>
        public event Func<InteractionInfo, ISlashCommandContext, IResult, Task> InteractionExecuted { add { _interactionExecutedEvent.Add(value); } remove { _interactionExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<InteractionInfo, ISlashCommandContext, IResult, Task>> _interactionExecutedEvent = new AsyncEvent<Func<InteractionInfo, ISlashCommandContext, IResult, Task>>();

        private readonly ConcurrentDictionary<Type, ModuleInfo> _typedModuleDefs;
        private readonly SlashCommandMap<SlashCommandInfo> _slashCommandMap;
        private readonly SlashCommandMap<ContextCommandInfo> _contextCommandMap;
        private readonly SlashCommandMap<InteractionInfo> _interactionCommandMap;
        private readonly HashSet<ModuleInfo> _moduleDefs;
        private readonly ConcurrentDictionary<Type, TypeReader> _typeReaders;
        private readonly ConcurrentDictionary<Type, Type> _genericTypeReaders;
        private readonly SemaphoreSlim _lock;
        internal readonly Logger _cmdLogger;
        internal readonly LogManager _logManager;

        internal readonly bool _runAsync, _throwOnError, _deleteUnkownCommandAck;

        /// <summary>
        /// Represents all of the modules that are loaded in the <see cref="SlashCommandService"/>
        /// </summary>
        public IReadOnlyList<ModuleInfo> Modules => _moduleDefs.ToList();

        /// <summary>
        /// Represents all of the executeable commands that are loaded in the <see cref="SlashCommandService"/> modules
        /// </summary>
        public IReadOnlyList<SlashCommandInfo> SlashCommands => _moduleDefs.SelectMany(x => x.SlashCommands).ToList();

        public IReadOnlyList<ContextCommandInfo> ContextCommands => _moduleDefs.SelectMany(x => x.ContextCommands).ToList();

        /// <summary>
        /// Represents all of the Interaction handlers that are loaded in the <see cref="SlashCommandService"/>
        /// </summary>
        public IReadOnlyCollection<InteractionInfo> Interacions => _moduleDefs.SelectMany(x => x.Interactions).ToList();

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
            _typedModuleDefs = new ConcurrentDictionary<Type, ModuleInfo>();
            _moduleDefs = new HashSet<ModuleInfo>();

            _logManager = new LogManager(config.LogLevel);
            _logManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
            _cmdLogger = _logManager.CreateLogger("Application Commands");

            _slashCommandMap = new SlashCommandMap<SlashCommandInfo>(this);
            _contextCommandMap = new SlashCommandMap<ContextCommandInfo>(this);
            _interactionCommandMap = new SlashCommandMap<InteractionInfo>(this, config.InteractionCustomIdDelimiters);

            Client = discord;

            _runAsync = config.RunAsync;
            _throwOnError = config.ThrowOnError;
            _deleteUnkownCommandAck = config.DeleteUnknownSlashCommandAck;

            _genericTypeReaders = new ConcurrentDictionary<Type, Type>();
            _genericTypeReaders[typeof(IChannel)] = typeof(DefaultChannelReader<>);
            _genericTypeReaders[typeof(IRole)] = typeof(DefaultRoleReader<>);
            _genericTypeReaders[typeof(IUser)] = typeof(DefaultUserReader<>);
            _genericTypeReaders[typeof(IMentionable)] = typeof(DefaultMentionableReader<>);
            _genericTypeReaders[typeof(IConvertible)] = typeof(DefaultValueTypeReader<>);
            _genericTypeReaders[typeof(Enum)] = typeof(EnumTypeReader<>);

            _typeReaders = new ConcurrentDictionary<Type, TypeReader>();
            _typeReaders[typeof(TimeSpan)] = new TimeSpanTypeReader();
        }

        /// <summary>
        /// Discover and load <see cref="SlashModuleBase{T}"/> from a given assembly
        /// </summary>
        /// <param name="assembly"><see cref="Assembly"/> the command modules are defined in</param>
        /// <param name="services"><see cref="IServiceProvider"/> to be used when instantiating a command module</param>
        /// <returns>Module information for the <see cref="SlashModuleBase{T}"/> types that are loaded to <see cref="SlashCommandService"/></returns>
        public async Task<IEnumerable<ModuleInfo>> AddModules (Assembly assembly, IServiceProvider services)
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

        public async Task AddModule<T> (IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            await _lock.WaitAsync().ConfigureAwait(false);

            try
            {
                var typeInfo = typeof(T).GetTypeInfo();

                if (_typedModuleDefs.ContainsKey(typeInfo))
                    throw new ArgumentException("Module definition for this type already exists.");

                var moduleDef = await ModuleClassBuilder.BuildAsync(new List<TypeInfo> { typeof(T).GetTypeInfo() }, this, services).ConfigureAwait(false);

                if (moduleDef[typeof(T)] == default(ModuleInfo))
                    throw new InvalidOperationException($"Could not build the module {typeInfo.FullName}, did you pass an invalid type?");

                if (!_typedModuleDefs.TryAdd(typeof(T), moduleDef[typeof(T)]))
                    throw new Exception("Cannot initialize this module.");

                LoadModuleInternal(moduleDef[typeof(T)]);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Register and update the Application Commands from <see cref="SlashCommandService.SlashCommands"/> while deleting the missing commands
        /// </summary>
        /// <param name="guild">Optional guild parameter, if defined, the commands are registered as guild commands for the provide guild, else commands are
        /// registered as global commands</param>
        /// <param name="deleteMissing">If true, delete all of the commands that are not registered in the <see cref="SlashCommandService"/></param>
        /// <returns></returns>
        public async Task SyncCommands (ulong? guildId = null, bool deleteMissing = true)
        {
            CheckApplicationId();

            var props = _typedModuleDefs.Values.SelectMany(x => x.ToModel()).ToList();

            IEnumerable<IApplicationCommand> existing;

            if (guildId.HasValue)
                existing = await ClientHelper.GetGuildApplicationCommands(Client, guildId.Value).ConfigureAwait(false);
            else
                existing = await ClientHelper.GetGlobalApplicationCommands(Client).ConfigureAwait(false);

            if (!deleteMissing)
            {
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToCreationProps()));
            }

            if (guildId.HasValue)
                await ClientHelper.BulkOverwriteGuildApplicationCommand(Client, guildId.Value, props.ToArray()).ConfigureAwait(false);
            else
                await ClientHelper.BulkOverwriteGlobalApplicationCommand(Client, props.ToArray()).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<ApplicationCommandProperties>> GetProps (ulong? guildId = null)
        {
            var props = _typedModuleDefs.Values.SelectMany(x => x.ToModel()).ToList();

            IEnumerable<IApplicationCommand> existing;

            if (guildId.HasValue)
                existing = await ClientHelper.GetGuildApplicationCommands(Client, guildId.Value).ConfigureAwait(false);
            else
                existing = await ClientHelper.GetGlobalApplicationCommands(Client).ConfigureAwait(false);

            var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
            props.AddRange(missing.Select(x => x.ToCreationProps()));

            return props;
        }

        /// <summary>
        /// Register a set of commands as "Guild Commands" to a guild
        /// </summary>
        /// <remarks>
        /// Commands will be registered as standalone commands, if you want the <see cref="SlashGroupAttribute"/> to take effect,
        /// use <see cref="AddModulesToGuild(IGuild, ModuleInfo[])"/>
        /// </remarks>
        /// <param name="guild">Guild the commands will be registered to</param>
        /// <param name="commands">Commands that will be registered</param>
        /// <returns></returns>
        public async Task AddCommandsToGuild (IGuild guild, params SlashCommandInfo[] commands)
        {
            CheckApplicationId();

            if (guild == null)
                throw new ArgumentException($"{nameof(guild)} cannot be null to call this function.");

            foreach (var com in commands)
            {
            }
        }

        /// <summary>
        /// Register a set of modules as "Guild Commands" to a guild
        /// </summary>
        /// <param name="guild">Guild the commands will be registered to</param>
        /// <param name="modules">Modules that will be registered</param>
        /// <returns></returns>
        public async Task AddModulesToGuild (IGuild guild, params ModuleInfo[] modules)
        {
            CheckApplicationId();

            if (guild == null)
                throw new ArgumentException($"{nameof(guild)} cannot be null to call this function.");

            var args = new List<CreateApplicationCommandParams>();

            foreach(var commandArg in args)
            {
                ApplicationCommand result = await Client.ApiClient.CreateGuildApplicationCommandAsync(commandArg, guild.Id);

                if (result == null)
                    await _cmdLogger.WarningAsync($"Module could not be registered ({commandArg.Name})").ConfigureAwait(false);
            }
        }

        private void LoadModuleInternal (ModuleInfo module)
        {
            _moduleDefs.Add(module);

            foreach (var command in module.SlashCommands)
                _slashCommandMap.AddCommand(command, command.IgnoreGroupNames);

            foreach (var command in module.ContextCommands)
                _contextCommandMap.AddCommand(command, command.IgnoreGroupNames);

            foreach (var interaction in module.Interactions)
                _interactionCommandMap.AddCommand(interaction, interaction.IgnoreGroupNames);

            foreach (var subModule in module.SubModules)
                LoadModuleInternal(subModule);
        }

        /// <summary>
        /// Remove a loaded module from <see cref="SlashCommandService.Modules"/>
        /// </summary>
        /// <param name="type"><see cref="SlashModuleBase{T}"/> that will be removed</param>
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

        private bool RemoveModuleInternal (ModuleInfo moduleInfo)
        {
            if (!_moduleDefs.Remove(moduleInfo))
                return false;


            foreach (var command in moduleInfo.SlashCommands)
            {
                _slashCommandMap.RemoveCommand(command);
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
        public async Task<IResult> ExecuteCommandAsync (ISlashCommandContext context, string[] input, IServiceProvider services)
        {
            var result = _slashCommandMap.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown slash command, skipping execution ({string.Join(" ", input).ToUpper()})");

                if(_deleteUnkownCommandAck)
                {
                    var response = await context.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                    await response.DeleteAsync().ConfigureAwait(false);
                }

                return result;
            }
            return await result.Command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        public async Task<IResult> ExecuteContextCommandAsync (ISlashCommandContext context, string input, IServiceProvider services)
            => await ExecuteCommandAsync(context, new string[] { input }, services).ConfigureAwait(false);

        public async Task<IResult> ExecuteContextCommandAsync (ISlashCommandContext context, string[] input, IServiceProvider services)
        {
            var result = _contextCommandMap.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown context command, skipping execution ({string.Join(" ", input).ToUpper()})");

                if (_deleteUnkownCommandAck)
                {
                    var response = await context.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                    await response.DeleteAsync().ConfigureAwait(false);
                }

                return result;
            }
            return await result.Command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        /// <summary>
        /// Use to execute an Interaction Handler from a <see cref="IDiscordInteractable.CustomId"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="ISlashCommandContext.Interaction"/>
        /// must be type of <see cref="SocketMessageInteraction"/></param>
        /// <param name="input">String that will be used to parse the <see cref="InteractionInfo"/>,
        /// set the <see cref="IDiscordInteractable.CustomId"/> of a message component the same as the <see cref="InteractionAttribute.CustomId"/> to handle
        /// Message Component Interactions automatically</param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns></returns>
        public async Task<IResult> ExecuteInteractionAsync (ISlashCommandContext context, string input, IServiceProvider services)
        {
            var result = _interactionCommandMap.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown custom interaction id, skipping execution ({input.ToUpper()})");

                if (_deleteUnkownCommandAck)
                {
                    var response = await context.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                    await response.DeleteAsync().ConfigureAwait(false);
                }

                return result;
            }
            return await result.Command.ExecuteAsync(context, services, result.WilCardMatch).ConfigureAwait(false);
        }

        internal TypeReader GetTypeReader (Type type)
        {
            if (_typeReaders.TryGetValue(type, out var specific))
                return specific;

            else if(_typeReaders.Any(x => x.Value.CanConvertTo(type)))
                return _typeReaders.First(x => x.Value.CanConvertTo(type)).Value;

            else if(_genericTypeReaders.Any(x => x.Key.IsAssignableFrom(type)))
            {
                var readerType = GetMostSpecificTypeReader(type);
                var reader = readerType.MakeGenericType(type).GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>()) as TypeReader;
                _typeReaders[type] = reader;
                return reader;
            }

            throw new ArgumentException($"No type reader is defined for this {nameof(Type)}", "type");
        }

        public void AddTypeReader(Type type, TypeReader reader)
        {
            if (!reader.CanConvertTo(type))
                throw new ArgumentException($"This {nameof(TypeReader)} cannot read {type.FullName} and cannot be registered as its type readers");

            _typeReaders[type] = reader;
        }

        public bool RemoveTypeReader<T> ( ) =>
            RemoveTypeReader(typeof(T));

        public bool RemoveTypeReader (Type type) =>
            _typeReaders.TryRemove(type, out var _);

        public void AddGenericTypeReader<T> (Type typeReader) =>
            AddGenericTypeReader(typeof(T), typeReader);

        public void AddGenericTypeReader (Type type, Type readerType )
        {
            if (!readerType.IsGenericTypeDefinition)
                throw new ArgumentException($"{nameof(TypeReader)} is not generic.");

            var genericArguments = readerType.GetGenericArguments();

            if (genericArguments.Count() > 1)
                throw new InvalidOperationException($"Valid generic {nameof(TypeReader)}s cannot have more than 1 generic type parameter");

            var constraints = genericArguments.SelectMany(x => x.GetGenericParameterConstraints());

            if (!constraints.Any(x => x.IsAssignableFrom(type)))
                throw new InvalidOperationException($"This generic class does not support type {type.FullName}");

            _genericTypeReaders[type] = readerType;
        }

        public bool RemoveGenericTypeReader<T> ( ) =>
            RemoveGenericTypeReader(typeof(T));

        public bool RemoveGenericTypeReader (Type type) =>
            _genericTypeReaders.TryRemove(type, out var _);

        public async Task<ParseResult> ParseGuildCommand (SlashCommandInfo command, IGuild guild )
        {
            var registered = await guild.GetApplicationCommandsAsync().ConfigureAwait(false);

            return await ParseCommands(command, registered).ConfigureAwait(false);
        }

        public async Task<ParseResult> ParseGlobalCommands (SlashCommandInfo command )
        {
            var registered = await Client.Rest.GetGlobalApplicationCommands().ConfigureAwait(false);

            return await ParseCommands(command, registered).ConfigureAwait(false);
        }

        private Task<ParseResult> ParseCommands (SlashCommandInfo command, IEnumerable<IApplicationCommand> applicationCommands)
        {
            var keywords = new List<string>() { command.Name };

            ModuleInfo parent = command.Module;
            while (parent != null)
            {
                if (parent.SlashGroupName != null)
                    keywords.Add(parent.SlashGroupName);

                parent = parent.Parent;
            }

            try
            {
                var index = keywords.Count - 1;
                var restCommand = applicationCommands.First(x => x.Name == keywords[index--]);
                var options = restCommand.Options;

                while (index >= 0)
                {
                    var option = options.First(x => x.Name == keywords[index--]);
                    options = option.Options;
                }

                return Task.FromResult(ParseResult.FromSuccess(restCommand));
            }
            catch (InvalidOperationException ex)
            {
                keywords.Reverse();
                var result = ParseResult.FromError(SlashCommandError.ParseFailed, $"No Slash command was found with the name {string.Join(" ", keywords)}");
                return Task.FromResult(result);
            }
        }

        public void Dispose ( )
        {
            _lock.Dispose();
        }

        private Type GetMostSpecificTypeReader ( Type type )
        {
            var scorePairs = new Dictionary<Type, int>();
            var validReaders = _genericTypeReaders.Where(x => x.Key.IsAssignableFrom(type));

            foreach(var typeReaderPair in validReaders)
            {
                var score = validReaders.Count(x => typeReaderPair.Key.IsAssignableFrom(x.Key));
                scorePairs.Add(typeReaderPair.Value, score);
            }

            return scorePairs.OrderBy(x => x.Value).ElementAt(0).Key;
        }

        private void CheckApplicationId ( )
        {
            if (Client.CurrentUser == null || Client.CurrentUser?.Id == 0)
                throw new InvalidOperationException($"Provided client is not ready to execute this operation, invoke this operation after a `Client Ready` event");
        }
    }
}
