using Discord.Logging;
using Discord.Rest;
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

        internal readonly bool _runAsync, _throwOnError, _deleteUnkownSlashCommandAck;
        internal readonly string _wildCardExp;

        /// <summary>
        /// Represents all of the modules that are loaded in the <see cref="SlashCommandService"/>
        /// </summary>
        public IReadOnlyList<ModuleInfo> Modules => _moduleDefs.ToList();

        /// <summary>
        /// Get all of the executeable Slash Commands that are loaded in the <see cref="SlashCommandService"/> modules
        /// </summary>
        public IReadOnlyList<SlashCommandInfo> SlashCommands => _moduleDefs.SelectMany(x => x.SlashCommands).ToList();

        /// <summary>
        /// Get all of the executeable Context Commands that are loaded in the <see cref="SlashCommandService"/> modules
        /// </summary>
        public IReadOnlyList<ContextCommandInfo> ContextCommands => _moduleDefs.SelectMany(x => x.ContextCommands).ToList();

        /// <summary>
        /// Get all of the Interaction handlers that are loaded in the <see cref="SlashCommandService"/>
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
            _cmdLogger = _logManager.CreateLogger("App Commands");

            _slashCommandMap = new SlashCommandMap<SlashCommandInfo>(this);
            _contextCommandMap = new SlashCommandMap<ContextCommandInfo>(this);
            _interactionCommandMap = new SlashCommandMap<InteractionInfo>(this, config.InteractionCustomIdDelimiters);

            Client = discord;

            _runAsync = config.RunAsync;
            _throwOnError = config.ThrowOnError;
            _deleteUnkownSlashCommandAck = config.DeleteUnknownSlashCommandAck;
            _wildCardExp = config.WildCardExpression;

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
        /// Discover and load all of the <see cref="SlashModuleBase{T}"/>s from a given assembly
        /// </summary>
        /// <param name="assembly"><see cref="Assembly"/> the command modules are defined in</param>
        /// <param name="services"><see cref="IServiceProvider"/> to be used when instantiating a command module</param>
        /// <returns>Module information for the <see cref="SlashModuleBase{T}"/> types that are loaded to <see cref="SlashCommandService"/></returns>
        public async Task<IEnumerable<ModuleInfo>> AddModulesAsync (Assembly assembly, IServiceProvider services)
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
        /// Add a command module manually to the command service
        /// </summary>
        /// <typeparam name="T">Type of the module</typeparam>
        /// <param name="services">Service provider that will be used to build this module</param>
        /// <returns>A task representing the module loading process</returns>
        /// <exception cref="ArgumentException">Thrown when a module that is already present in the command service is trying to be added</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <typeparamref name="T"/> is not a valid module definition</exception>
        public async Task AddModuleAsync<T> (IServiceProvider services)
        {
            if (!typeof(ISlashModuleBase).IsAssignableFrom(typeof(T)))
                throw new ArgumentException("Type parameter must be a type of Slash Module", "T");

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
                    throw new ArgumentException("Module definition for this type already exists.");

                LoadModuleInternal(moduleDef[typeof(T)]);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Register/update the Application Commands to Discord
        /// </summary>
        /// <param name="guildId">The Id that belongs to the guild, the commands will be registered to</param>
        /// <param name="deleteMissing">If true, delete all of the commands that are not registered in the <see cref="SlashCommandService"/></param>
        /// <returns>A task representing the command registration process, with a collection of <see cref="RestGuildCommand"/> containing the
        /// commands that are currently registered to the provided guild as its result.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> RegisterCommandsToGuildAsync (ulong guildId, bool deleteMissing = true)
        {
            CheckApplicationId();

            IEnumerable<IApplicationCommand> existing = null;

            if (deleteMissing)
                existing = await ClientHelper.GetGuildApplicationCommands(Client, guildId).ConfigureAwait(false);

            var props = _typedModuleDefs.Values.SelectMany(x => x.ToApplicationCommandProps()).ToList();

            if (existing != null)
            {
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await ClientHelper.BulkOverwriteGuildApplicationCommand(Client, guildId, props.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Register/update the Application Commands to Discord
        /// </summary>
        /// <param name="deleteMissing">If true, delete all of the commands that are not registered in the <see cref="SlashCommandService"/></param>
        /// <returns>A task representing the command registration process, with a collection of <see cref="RestGlobalCommand"/> containing the
        /// global commands that are currently registered to the Discord
        /// </returns>
        public async Task<IReadOnlyCollection<RestGlobalCommand>> RegisterCommandGloballyAsync (bool deleteMissing = true)
        {
            CheckApplicationId();

            IEnumerable<IApplicationCommand> existing = null;

            if (deleteMissing)
                existing = await ClientHelper.GetGlobalApplicationCommands(Client).ConfigureAwait(false);

            var props = _typedModuleDefs.Values.SelectMany(x => x.ToApplicationCommandProps()).ToList();

            if (existing != null)
            {
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await ClientHelper.BulkOverwriteGlobalApplicationCommand(Client, props.ToArray()).ConfigureAwait(false);
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
        /// <returns>A task representing the command registration process, with a collection of <see cref="RestGuildCommand"/> containing the
        /// commands that are currently registered to the provided guild as its result.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> AddCommandsToGuildAsync (IGuild guild, params SlashCommandInfo[] commands)
        {
            CheckApplicationId();

            if (guild == null)
                throw new ArgumentException($"{nameof(guild)} cannot be null to call this function.");

            var existing = await ClientHelper.GetGuildApplicationCommands(Client, guild.Id).ConfigureAwait(false);
            var props = commands.Select(x => x.ToApplicationCommandProps()).ToList();

            if (existing != null)
            {
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await ClientHelper.BulkOverwriteGuildApplicationCommand(Client, guild.Id, props.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Register a set of modules as "Guild Commands" to a guild
        /// </summary>
        /// <param name="guild">Guild the commands will be registered to</param>
        /// <param name="modules">Modules that will be registered</param>
        /// <returns>A task representing the command registration process, with a collection of <see cref="RestGuildCommand"/> containing the
        /// commands that are currently registered to the provided guild as its result.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> AddModulesToGuildAsync (IGuild guild, params ModuleInfo[] modules)
        {
            CheckApplicationId();

            if (guild == null)
                throw new ArgumentException($"{nameof(guild)} cannot be null to call this function.");

            var existing = await ClientHelper.GetGuildApplicationCommands(Client, guild.Id).ConfigureAwait(false);
            var props = _typedModuleDefs.Values.SelectMany(x => x.ToApplicationCommandProps()).ToList();

            foreach (var command in existing)
                props.Add(command.ToApplicationCommandProps());

            return await ClientHelper.BulkOverwriteGuildApplicationCommand(Client, guild.Id, props.ToArray()).ConfigureAwait(false);
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
        /// Execute a slash command from a given <see cref="ISlashCommandContext"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="ISlashCommandContext.Interaction"/>
        /// must be type of <see cref="SocketSlashCommand"/></param>
        /// <param name="input">Command string that will be used to parse the <see cref="SlashCommandInfo"/>. Use the
        /// <see cref="WebSocketExtensions.GetCommandKeywords(SocketSlashCommand)"/> to get the input </param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns>A task representing the command execution process, with an <see cref="IResult"/> containg the execution information as it result.</returns>
        public async Task<IResult> ExecuteSlashCommandAsync (ISlashCommandContext context, string[] input, IServiceProvider services)
        {
            var result = _slashCommandMap.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown slash command, skipping execution ({string.Join(" ", input).ToUpper()})");

                if (_deleteUnkownSlashCommandAck)
                {
                    var response = await context.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                    await response.DeleteAsync().ConfigureAwait(false);
                }

                return result;
            }
            return await result.Command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a context command from a given <see cref="ISlashCommandContext"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="ISlashCommandContext.Interaction"/>
        /// must be type of <see cref="SocketUserCommand"/> or <see cref="SocketMessageCommand"/></param>
        /// <param name="input">Command string that will be used to parse the <see cref="SlashCommandInfo"/>.( In normal use, this should be equal to
        /// <see cref="IDiscordInteractionData.Name"/> )</param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns>A task representing the command execution process, with an <see cref="IResult"/> containg the execution information as it result.</returns>
        public async Task<IResult> ExecuteContextCommandAsync (ISlashCommandContext context, string input, IServiceProvider services)
            => await ExecuteContextCommandAsync(context, new string[] { input }, services).ConfigureAwait(false);

        /// <summary>
        /// Execute a context command from a given <see cref="ISlashCommandContext"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="ISlashCommandContext.Interaction"/>
        /// must be type of <see cref="SocketUserCommand"/> or <see cref="SocketMessageCommand"/></param>
        /// <param name="input">A collection of keywords that will be used for command traversel</param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns>A task representing the command execution process, with an <see cref="IResult"/> containg the execution information as it result.</returns>
        public async Task<IResult> ExecuteContextCommandAsync (ISlashCommandContext context, string[] input, IServiceProvider services)
        {
            var result = _contextCommandMap.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown context command, skipping execution ({string.Join(" ", input).ToUpper()})");

                return result;
            }
            return await result.Command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a Message Component Interaction handler from a given <see cref="ISlashCommandContext"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="ISlashCommandContext.Interaction"/>
        /// must be type of <see cref="SocketMessageComponent"/></param>
        /// <param name="input">Command string that will be used to parse the <see cref="SlashCommandInfo"/>.( In normal use, this should be equal to
        /// <see cref="SocketMessageComponentData.CustomId"/> )</param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns>A task representing the command execution process, with an <see cref="IResult"/> containg the execution information as it result.</returns>
        public async Task<IResult> ExecuteInteractionAsync (ISlashCommandContext context, string input, IServiceProvider services)
        {
            var result = _interactionCommandMap.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown custom interaction id, skipping execution ({input.ToUpper()})");

                return result;
            }
            return await result.Command.ExecuteAsync(context, services, result.RegexCaptureGroups).ConfigureAwait(false);
        }

        internal TypeReader GetTypeReader (Type type)
        {
            if (_typeReaders.TryGetValue(type, out var specific))
                return specific;

            else if (_typeReaders.Any(x => x.Value.CanConvertTo(type)))
                return _typeReaders.First(x => x.Value.CanConvertTo(type)).Value;

            else if (_genericTypeReaders.Any(x => x.Key.IsAssignableFrom(type)))
            {
                var readerType = GetMostSpecificTypeReader(type);
                var reader = readerType.MakeGenericType(type).GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>()) as TypeReader;
                _typeReaders[type] = reader;
                return reader;
            }

            throw new ArgumentException($"No type reader is defined for this {nameof(Type)}", "type");
        }

        /// <summary>
        /// Add a concrete type <see cref="TypeReader"/> to the command service
        /// </summary>
        /// <typeparam name="T">The type this <see cref="TypeReader"/> will be used to handle></typeparam>
        /// <param name="reader">The <see cref="TypeReader"/> instance</param>
        public void AddTypeReader<T> (TypeReader reader) =>
            AddTypeReader(typeof(T), reader);

        /// <summary>
        /// Add a concrete type <see cref="TypeReader"/> to the command service
        /// </summary>
        /// <param name="type">The type this <see cref="TypeReader"/> will be used to handle</param>
        /// <param name="reader">The <see cref="TypeReader"/> instance</param>
        public void AddTypeReader (Type type, TypeReader reader)
        {
            if (!reader.CanConvertTo(type))
                throw new ArgumentException($"This {nameof(TypeReader)} cannot read {type.FullName} and cannot be registered as its type readers");

            _typeReaders[type] = reader;
        }

        /// <summary>
        /// Add a generic type <see cref="TypeReader{T}"/> to the command service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeReader"></param>
        public void AddGenericTypeReader<T> (Type typeReader) =>
            AddGenericTypeReader(typeof(T), typeReader);

        /// <summary>
        /// Add a generic type <see cref="TypeReader{T}"/> to the command service
        /// </summary>
        /// <param name="type"></param>
        /// <param name="readerType"></param>
        public void AddGenericTypeReader (Type type, Type readerType)
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

        /// <summary>
        /// Modify the command permissions of the matching Discord Slash Command
        /// </summary>
        /// <param name="module">Module representing the top level Slash Command</param>
        /// <param name="guild">Target guild</param>
        /// <param name="permissions">Set of permissions to be modified</param>
        /// <returns>The active command permissions after the modification</returns>
        public async Task<GuildApplicationCommandPermission> ModifySlashCommandPermissionsAsync (ModuleInfo module, IGuild guild,
            params ApplicationCommandPermission[] permissions)
        {
            if (!module.IsSlashGroup)
                throw new InvalidOperationException($"This module does not have a {nameof(SlashGroupAttribute)} and does not represent an Application Command");

            if (!module.IsTopLevel)
                throw new InvalidOperationException("This module is not a top level application command. You cannot change its permissions");

            if (guild == null)
                throw new ArgumentNullException("guild");

            var commands = await ClientHelper.GetGuildApplicationCommands(Client, guild.Id).ConfigureAwait(false);
            var appCommand = commands.First(x => x.Name == module.SlashGroupName);

            return await appCommand.ModifyCommandPermissions(permissions).ConfigureAwait(false);
        }

        /// <summary>
        /// Modify the command permissions of the matching Discord Slash Command
        /// </summary>
        /// <param name="command">Command representing the top level Slash Command</param>
        /// <param name="guild">Target guild</param>
        /// <param name="permissions">Set of permissions to be modified</param>
        /// <returns>The active command permissions after the modification</returns>
        public async Task<GuildApplicationCommandPermission> ModifySlashCommandPermissionsAsync (SlashCommandInfo command, IGuild guild,
            params ApplicationCommandPermission[] permissions) =>
            await ModifyApplicationCommandPermissionsAsync(command, guild, permissions).ConfigureAwait(false);

        /// <summary>
        /// Modify the command permissions of the matching Discord Context Command
        /// </summary>
        /// <param name="command">Command representing the top level Context Command</param>
        /// <param name="guild">Target guild</param>
        /// <param name="permissions">Set of permissions to be modified</param>
        /// <returns>The active command permissions after the modification</returns>
        public async Task<GuildApplicationCommandPermission> ModifyContextCommandPermissionsAsync (ContextCommandInfo command, IGuild guild,
            params ApplicationCommandPermission[] permissions) =>
            await ModifyApplicationCommandPermissionsAsync(command, guild, permissions).ConfigureAwait(false);

        private async Task<GuildApplicationCommandPermission> ModifyApplicationCommandPermissionsAsync (IExecutableInfo command, IGuild guild,
            params ApplicationCommandPermission[] permissions)
        {
            if (!command.IsTopLevel)
                throw new InvalidOperationException("This command is not a top level application command. You cannot change its permissions");

            if (guild == null)
                throw new ArgumentNullException("guild");

            var commands = await ClientHelper.GetGuildApplicationCommands(Client, guild.Id).ConfigureAwait(false);
            var appCommand = commands.First(x => x.Name == command.Name);

            return await appCommand.ModifyCommandPermissions(permissions).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the created <see cref="SlashCommandInfo"/> instance for a Slash Command handler
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="SlashModuleBase{T}"/></typeparam>
        /// <param name="name">Method name of the handler, use of <see langword="nameof"/> is recommended</param>
        /// <returns>The loaded <see cref="SlashCommandInfo"/> instance for this method</returns>
        /// <exception cref="InvalidOperationException">The module is not registered to the command service or the slash command could not be found</exception>
        public SlashCommandInfo GetSlashCommandInfo<TModule> (string name) where TModule : SlashModuleBase
        {
            var module = GetModuleInfo<TModule>();

            return module.SlashCommands.First(x => x.MethodName == name);
        }

        /// <summary>
        /// Get the created <see cref="ContextCommandInfo"/> instance for a Context Command handler
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="SlashModuleBase{T}"/></typeparam>
        /// <param name="name">Method name of the handler, use of <see langword="nameof"/> is recommended</param>
        /// <returns>The loaded <see cref="ContextCommandInfo"/> instance for this method</returns>
        /// <exception cref="InvalidOperationException">The module is not registered to the command service or the context command could not be found</exception>
        public ContextCommandInfo GetContextCommandInfo<TModule> (string name) where TModule : SlashModuleBase
        {
            var module = GetModuleInfo<TModule>();

            return module.ContextCommands.First(x => x.MethodName == name);
        }

        /// <summary>
        /// Get the created <see cref="InteractionInfo"/> instance for a Message Component interaction handler
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="SlashModuleBase{T}"/></typeparam>
        /// <param name="name">Method name of the handler, use of <see langword="nameof"/> is recommended</param>
        /// <returns>The loaded <see cref="InteractionInfo"/> instance for this method</returns>
        /// <exception cref="InvalidOperationException">The module is not registered to the command service or the interaction could not be found</exception>
        public InteractionInfo GetInteractionInfo<TModule> (string name) where TModule : SlashModuleBase
        {
            var module = GetModuleInfo<TModule>();

            return module.Interactions.First(x => x.MethodName == name);
        }

        /// <summary>
        /// Get the created <see cref="ModuleInfo"/> instance for a module type
        /// </summary>
        /// <typeparam name="TModule">Type of the module, must be a type of <see cref="SlashModuleBase{T}"/></typeparam>
        /// <returns>The loaded <see cref="ModuleInfo"/> instance for this method</returns>
        public ModuleInfo GetModuleInfo<TModule> ( ) where TModule : SlashModuleBase
        {
            if (!typeof(ISlashModuleBase).IsAssignableFrom(typeof(TModule)))
                throw new ArgumentException("Type parameter must be a type of Slash Module", "TModule");

            var module = _typedModuleDefs[typeof(TModule)];

            if (module == null)
                throw new InvalidOperationException($"{typeof(TModule).FullName} is not loaded to the Slash Command Service");

            return module;
        }

        public void Dispose ( )
        {
            _lock.Dispose();
        }

        private Type GetMostSpecificTypeReader (Type type)
        {
            var scorePairs = new Dictionary<Type, int>();
            var validReaders = _genericTypeReaders.Where(x => x.Key.IsAssignableFrom(type));

            foreach (var typeReaderPair in validReaders)
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
