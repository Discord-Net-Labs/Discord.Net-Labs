using Discord.Logging;
using Discord.Rest;
using Discord.Interactions.Builders;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    /// Provides the framework for self registering and self-executing Discord Application Commands
    /// </summary>
    public class InteractionService : IDisposable
    {
        /// <summary>
        /// Occurs when a Slash Command related information is recieved
        /// </summary>
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        /// <summary>
        /// Occurs when a Slash Command is executed
        /// </summary>
        public event Func<SlashCommandInfo, IInteractionCommandContext, IResult, Task> SlashCommandExecuted { add { _slashCommandExecutedEvent.Add(value); } remove { _slashCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<SlashCommandInfo, IInteractionCommandContext, IResult, Task>> _slashCommandExecutedEvent = new AsyncEvent<Func<SlashCommandInfo, IInteractionCommandContext, IResult, Task>>();

        /// <summary>
        /// Occurs when a Context Command is executed
        /// </summary>
        public event Func<ContextCommandInfo, IInteractionCommandContext, IResult, Task> ContextCommandExecuted { add { _contextCommandExecutedEvent.Add(value); } remove { _contextCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<ContextCommandInfo, IInteractionCommandContext, IResult, Task>> _contextCommandExecutedEvent = new AsyncEvent<Func<ContextCommandInfo, IInteractionCommandContext, IResult, Task>>();

        /// <summary>
        /// Occurs when a Message Component command is executed
        /// </summary>
        public event Func<ComponentCommandInfo, IInteractionCommandContext, IResult, Task> ComponentCommandExecuted { add { _interactionExecutedEvent.Add(value); } remove { _interactionExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<ComponentCommandInfo, IInteractionCommandContext, IResult, Task>> _interactionExecutedEvent = new AsyncEvent<Func<ComponentCommandInfo, IInteractionCommandContext, IResult, Task>>();

        private readonly ConcurrentDictionary<Type, ModuleInfo> _typedModuleDefs;
        private readonly CommandMap<SlashCommandInfo> _slashCommandMap;
        private readonly CommandMap<ContextCommandInfo> _contextCommandMap;
        private readonly CommandMap<ComponentCommandInfo> _componentCommandMap;
        private readonly HashSet<ModuleInfo> _moduleDefs;
        private readonly ConcurrentDictionary<Type, TypeConverter> _typeConverters;
        private readonly ConcurrentDictionary<Type, Type> _genericTypeConverters;
        private readonly SemaphoreSlim _lock;
        internal readonly Logger _cmdLogger;
        internal readonly LogManager _logManager;

        internal readonly bool _throwOnError, _deleteUnkownSlashCommandAck, _useCompiledLambda;
        internal readonly string _wildCardExp;
        internal readonly RunMode _runMode;

        /// <summary>
        /// Represents all of the modules that are loaded in the <see cref="InteractionService"/>
        /// </summary>
        public IReadOnlyList<ModuleInfo> Modules => _moduleDefs.ToList();

        /// <summary>
        /// Get all of the executeable Slash Commands that are loaded in the <see cref="InteractionService"/> modules
        /// </summary>
        public IReadOnlyList<SlashCommandInfo> SlashCommands => _moduleDefs.SelectMany(x => x.SlashCommands).ToList();

        /// <summary>
        /// Get all of the executeable Context Commands that are loaded in the <see cref="InteractionService"/> modules
        /// </summary>
        public IReadOnlyList<ContextCommandInfo> ContextCommands => _moduleDefs.SelectMany(x => x.ContextCommands).ToList();

        /// <summary>
        /// Get all of the Interaction handlers that are loaded in the <see cref="InteractionService"/>
        /// </summary>
        public IReadOnlyCollection<ComponentCommandInfo> ComponentCommands => _moduleDefs.SelectMany(x => x.ComponentCommands).ToList();

        /// <summary>
        /// Client that the Application Commands will be registered for
        /// </summary>
        public BaseSocketClient Client { get; }

        /// <summary>
        /// Initialize a <see cref="InteractionService"/> with the default configurations
        /// </summary>
        /// <param name="discord">The client that will be used to register commands</param>
        public InteractionService (BaseSocketClient discord) : this(discord, new InteractionServiceConfig()) { }

        /// <summary>
        /// Initialize a <see cref="InteractionService"/> with configurations from a provided <see cref="InteractionServiceConfig"/>
        /// </summary>
        /// <param name="discord">The client that will be used to register commands</param>
        /// <param name="config">The configuration class</param>
        public InteractionService (BaseSocketClient discord, InteractionServiceConfig config)
        {
            _lock = new SemaphoreSlim(1, 1);
            _typedModuleDefs = new ConcurrentDictionary<Type, ModuleInfo>();
            _moduleDefs = new HashSet<ModuleInfo>();

            _logManager = new LogManager(config.LogLevel);
            _logManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
            _cmdLogger = _logManager.CreateLogger("App Commands");

            _slashCommandMap = new CommandMap<SlashCommandInfo>(this);
            _contextCommandMap = new CommandMap<ContextCommandInfo>(this);
            _componentCommandMap = new CommandMap<ComponentCommandInfo>(this, config.InteractionCustomIdDelimiters);

            Client = discord;

            _runMode = config.RunMode;
            if (_runMode == RunMode.Default)
                throw new InvalidOperationException($"RunMode cannot be set to {RunMode.Default}");

            _throwOnError = config.ThrowOnError;
            _deleteUnkownSlashCommandAck = config.DeleteUnknownSlashCommandAck;
            _wildCardExp = config.WildCardExpression;
            _useCompiledLambda = config.UseCompiledLambda;

            _genericTypeConverters = new ConcurrentDictionary<Type, Type>
            {
                [typeof(IChannel)] = typeof(DefaultChannelConverter<>),
                [typeof(IRole)] = typeof(DefaultRoleConverter<>),
                [typeof(IUser)] = typeof(DefaultUserConverter<>),
                [typeof(IMentionable)] = typeof(DefaultMentionableConverter<>),
                [typeof(IConvertible)] = typeof(DefaultValueConverter<>),
                [typeof(Enum)] = typeof(EnumConverter<>)
            };

            _typeConverters = new ConcurrentDictionary<Type, TypeConverter>
            {
                [typeof(TimeSpan)] = new TimeSpanConverter()
            };
        }

        /// <summary>
        /// Discover and load all of the <see cref="InteractionModuleBase{T}"/>s from a given assembly
        /// </summary>
        /// <param name="assembly"><see cref="Assembly"/> the command modules are defined in</param>
        /// <param name="services"><see cref="IServiceProvider"/> to be used when instantiating a command module</param>
        /// <returns>Module information for the <see cref="InteractionModuleBase{T}"/> types that are loaded to <see cref="InteractionService"/></returns>
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
            if (!typeof(IInteractionModuleBase).IsAssignableFrom(typeof(T)))
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
        /// <param name="deleteMissing">If true, delete all of the commands that are not registered in the <see cref="InteractionService"/></param>
        /// <returns>A task representing the command registration process, with a collection of <see cref="RestGuildCommand"/> containing the
        /// commands that are currently registered to the provided guild as its result.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> RegisterCommandsToGuildAsync (ulong guildId, bool deleteMissing = true)
        {
            EnsureClientReady();

            var props = _typedModuleDefs.Values.SelectMany(x => x.ToApplicationCommandProps()).ToList();

            if (!deleteMissing)
            {
                var existing = await ClientHelper.GetGuildApplicationCommands(Client, guildId).ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await ClientHelper.BulkOverwriteGuildApplicationCommand(Client, guildId, props.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Register/update the Application Commands to Discord
        /// </summary>
        /// <param name="deleteMissing">If true, delete all of the commands that are not registered in the <see cref="InteractionService"/></param>
        /// <returns>A task representing the command registration process, with a collection of <see cref="RestGlobalCommand"/> containing the
        /// global commands that are currently registered to the Discord
        /// </returns>
        public async Task<IReadOnlyCollection<RestGlobalCommand>> RegisterCommandsGloballyAsync (bool deleteMissing = true)
        {
            EnsureClientReady();

            var props = _typedModuleDefs.Values.SelectMany(x => x.ToApplicationCommandProps()).ToList();
            
            if (!deleteMissing)
            {
                var existing = await ClientHelper.GetGlobalApplicationCommands(Client).ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await ClientHelper.BulkOverwriteGlobalApplicationCommand(Client, props.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Register a set of commands as "Guild Commands" to a guild
        /// </summary>
        /// <remarks>
        /// Commands will be registered as standalone commands, if you want the <see cref="GroupAttribute"/> to take effect,
        /// use <see cref="AddModulesToGuild(IGuild, ModuleInfo[])"/>
        /// </remarks>
        /// <param name="guild">Guild the commands will be registered to</param>
        /// <param name="commands">Commands that will be registered</param>
        /// <returns>A task representing the command registration process, with a collection of <see cref="RestGuildCommand"/> containing the
        /// commands that are currently registered to the provided guild as its result.
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> AddCommandsToGuildAsync (IGuild guild, params IApplicationCommandInfo[] commands)
        {
            EnsureClientReady();

            if (guild is null)
                throw new ArgumentNullException(nameof(guild));

            var existing = await ClientHelper.GetGuildApplicationCommands(Client, guild.Id).ConfigureAwait(false);

            var props = new List<ApplicationCommandProperties>();

            foreach(var command in commands)
            {
                switch (command)
                {
                    case SlashCommandInfo slashCommand:
                        props.Add(slashCommand.ToApplicationCommandProps());
                        break;
                    case ContextCommandInfo contextCommand:
                        props.Add(contextCommand.ToApplicationCommandProps());
                        break;
                    default:
                        throw new InvalidOperationException($"Command type {command.GetType().FullName} isn't supported yet");
                }
            }

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
            EnsureClientReady();

            if (guild is null)
                throw new ArgumentNullException(nameof(guild));

            var existing = await ClientHelper.GetGuildApplicationCommands(Client, guild.Id).ConfigureAwait(false);
            var props = modules.SelectMany(x => x.ToApplicationCommandProps(true)).ToList();

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

            foreach (var interaction in module.ComponentCommands)
                _componentCommandMap.AddCommand(interaction, interaction.IgnoreGroupNames);

            foreach (var subModule in module.SubModules)
                LoadModuleInternal(subModule);
        }

        /// <summary>
        /// Remove a loaded module from <see cref="InteractionService.Modules"/>
        /// </summary>
        /// <param name="type"><see cref="InteractionModuleBase{T}"/> that will be removed</param>
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
        /// Execute a slash command from a given <see cref="IInteractionCommandContext"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="IInteractionCommandContext.Interaction"/>
        /// must be type of <see cref="SocketSlashCommand"/></param>
        /// <param name="input">Command string that will be used to parse the <see cref="SlashCommandInfo"/>. Use the
        /// <see cref="WebSocketExtensions.GetCommandKeywords(SocketSlashCommand)"/> to get the input </param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns>A task representing the command execution process, with an <see cref="IResult"/> containg the execution information as it result.</returns>
        public async Task<IResult> ExecuteSlashCommandAsync (IInteractionCommandContext context, string[] input, IServiceProvider services)
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
        /// Execute a context command from a given <see cref="IInteractionCommandContext"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="IInteractionCommandContext.Interaction"/>
        /// must be type of <see cref="SocketUserCommand"/> or <see cref="SocketMessageCommand"/></param>
        /// <param name="input">Command string that will be used to parse the <see cref="SlashCommandInfo"/>.( In normal use, this should be equal to
        /// <see cref="IDiscordInteractionData.Name"/> )</param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns>A task representing the command execution process, with an <see cref="IResult"/> containg the execution information as it result.</returns>
        public async Task<IResult> ExecuteContextCommandAsync (IInteractionCommandContext context, string input, IServiceProvider services)
            => await ExecuteContextCommandAsync(context, new string[] { input }, services).ConfigureAwait(false);

        /// <summary>
        /// Execute a context command from a given <see cref="IInteractionCommandContext"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="IInteractionCommandContext.Interaction"/>
        /// must be type of <see cref="SocketUserCommand"/> or <see cref="SocketMessageCommand"/></param>
        /// <param name="input">A collection of keywords that will be used for command traversel</param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns>A task representing the command execution process, with an <see cref="IResult"/> containg the execution information as it result.</returns>
        public async Task<IResult> ExecuteContextCommandAsync (IInteractionCommandContext context, string[] input, IServiceProvider services)
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
        /// Execute a Message Component Interaction handler from a given <see cref="IInteractionCommandContext"/>
        /// </summary>
        /// <param name="context">A command context that will be used to execute the command, <see cref="IInteractionCommandContext.Interaction"/>
        /// must be type of <see cref="SocketMessageComponent"/></param>
        /// <param name="input">Command string that will be used to parse the <see cref="SlashCommandInfo"/>.( In normal use, this should be equal to
        /// <see cref="SocketMessageComponentData.CustomId"/> )</param>
        /// <param name="services">Services that will be injected into the declaring type</param>
        /// <returns>A task representing the command execution process, with an <see cref="IResult"/> containg the execution information as it result.</returns>
        public async Task<IResult> ExecuteInteractionAsync (IInteractionCommandContext context, string input, IServiceProvider services)
        {
            var result = _componentCommandMap.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown custom interaction id, skipping execution ({input.ToUpper()})");

                return result;
            }
            return await result.Command.ExecuteAsync(context, services, result.RegexCaptureGroups).ConfigureAwait(false);
        }

        internal TypeConverter GetTypeConverter (Type type)
        {
            if (_typeConverters.TryGetValue(type, out var specific))
                return specific;

            else if (_typeConverters.Any(x => x.Value.CanConvertTo(type)))
                return _typeConverters.First(x => x.Value.CanConvertTo(type)).Value;

            else if (_genericTypeConverters.Any(x => x.Key.IsAssignableFrom(type)))
            {
                var converterType = GetMostSpecificTypeConverter(type);
                var converter = converterType.MakeGenericType(type).GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>()) as TypeConverter;
                _typeConverters[type] = converter;
                return converter;
            }

            throw new ArgumentException($"No type {nameof(TypeConverter)} is defined for this {type.FullName}", "type");
        }

        /// <summary>
        /// Add a concrete type <see cref="TypeConverter"/> to the command service
        /// </summary>
        /// <typeparam name="T">The type this <see cref="TypeConverter"/> will be used to handle</typeparam>
        /// <param name="converter">The <see cref="TypeConverter"/> instance</param>
        public void AddTypeConverter<T> (TypeConverter converter) =>
            AddTypeConverter(typeof(T), converter);

        /// <summary>
        /// Add a concrete type <see cref="TypeConverter"/> to the command service
        /// </summary>
        /// <param name="type">The type this <see cref="TypeConverter"/> will be used to handle</param>
        /// <param name="converter">The <see cref="TypeConverter"/> instance</param>
        public void AddTypeConverter (Type type, TypeConverter converter)
        {
            if (!converter.CanConvertTo(type))
                throw new ArgumentException($"This {converter.GetType().FullName} cannot read {type.FullName} and cannot be registered as its {nameof(TypeConverter)}");

            _typeConverters[type] = converter;
        }

        public void AddGenericTypeConverter<T, TConverter> ( ) where TConverter : TypeConverter, new() =>
            AddGenericTypeConverter<TConverter>(typeof(T));

        /// <summary>
        /// Add a generic type <see cref="TypeConverter{T}"/> to the command service
        /// </summary>
        /// <param name="type"></param>
        /// <typeparam name="TConverter"></typeparam>
        public void AddGenericTypeConverter<TConverter> (Type type) where TConverter : TypeConverter, new()
        {
            var converterType = typeof(TConverter);

            if (!converterType.IsGenericTypeDefinition)
                throw new ArgumentException($"{converterType.FullName} is not generic.");

            var genericArguments = converterType.GetGenericArguments();

            if (genericArguments.Count() > 1)
                throw new InvalidOperationException($"Valid generic {converterType.FullName}s cannot have more than 1 generic type parameter");

            var constraints = genericArguments.SelectMany(x => x.GetGenericParameterConstraints());

            if (!constraints.Any(x => x.IsAssignableFrom(type)))
                throw new InvalidOperationException($"This generic class does not support type {type.FullName}");

            _genericTypeConverters[type] = converterType;
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
                throw new InvalidOperationException($"This module does not have a {nameof(GroupAttribute)} and does not represent an Application Command");

            if (!module.IsTopLevelGroup)
                throw new InvalidOperationException("This module is not a top level application command. You cannot change its permissions");

            if (guild is null)
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

        private async Task<GuildApplicationCommandPermission> ModifyApplicationCommandPermissionsAsync<T> (T command, IGuild guild,
            params ApplicationCommandPermission[] permissions) where T : class, IApplicationCommandInfo, ICommandInfo
        {
            if (!command.IsTopLevelCommand)
                throw new InvalidOperationException("This command is not a top level application command. You cannot change its permissions");

            if (guild is null)
                throw new ArgumentNullException("guild");

            var commands = await ClientHelper.GetGuildApplicationCommands(Client, guild.Id).ConfigureAwait(false);
            var appCommand = commands.First(x => x.Name == (command as IApplicationCommandInfo).Name);

            return await appCommand.ModifyCommandPermissions(permissions).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the created <see cref="SlashCommandInfo"/> instance for a Slash Command handler
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="InteractionModuleBase{T}"/></typeparam>
        /// <param name="methodName">Method name of the handler, use of <see langword="nameof"/> is recommended</param>
        /// <returns>The loaded <see cref="SlashCommandInfo"/> instance for this method</returns>
        /// <exception cref="InvalidOperationException">The module is not registered to the command service or the slash command could not be found</exception>
        public SlashCommandInfo GetSlashCommandInfo<TModule> (string methodName) where TModule : class, IInteractionModuleBase
        {
            var module = GetModuleInfo<TModule>();

            return module.SlashCommands.First(x => x.MethodName == methodName);
        }

        /// <summary>
        /// Get the created <see cref="ContextCommandInfo"/> instance for a Context Command handler
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="InteractionModuleBase{T}"/></typeparam>
        /// <param name="methodName">Method name of the handler, use of <see langword="nameof"/> is recommended</param>
        /// <returns>The loaded <see cref="ContextCommandInfo"/> instance for this method</returns>
        /// <exception cref="InvalidOperationException">The module is not registered to the command service or the context command could not be found</exception>
        public ContextCommandInfo GetContextCommandInfo<TModule> (string methodName) where TModule : class, IInteractionModuleBase
        {
            var module = GetModuleInfo<TModule>();

            return module.ContextCommands.First(x => x.MethodName == methodName);
        }

        /// <summary>
        /// Get the created <see cref="ComponentCommandInfo"/> instance for a Message Component interaction handler
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="InteractionModuleBase{T}"/></typeparam>
        /// <param name="methodName">Method name of the handler, use of <see langword="nameof"/> is recommended</param>
        /// <returns>The loaded <see cref="ComponentCommandInfo"/> instance for this method</returns>
        /// <exception cref="InvalidOperationException">The module is not registered to the command service or the interaction could not be found</exception>
        public ComponentCommandInfo GetInteractionInfo<TModule> (string methodName) where TModule : class, IInteractionModuleBase
        {
            var module = GetModuleInfo<TModule>();

            return module.ComponentCommands.First(x => x.MethodName == methodName);
        }

        /// <summary>
        /// Get the created <see cref="ModuleInfo"/> instance for a module type
        /// </summary>
        /// <typeparam name="TModule">Type of the module, must be a type of <see cref="InteractionModuleBase{T}"/></typeparam>
        /// <returns>The loaded <see cref="ModuleInfo"/> instance for this method</returns>
        public ModuleInfo GetModuleInfo<TModule> ( ) where TModule : class, IInteractionModuleBase
        {
            if (!typeof(IInteractionModuleBase).IsAssignableFrom(typeof(TModule)))
                throw new ArgumentException("Type parameter must be a type of Slash Module", "TModule");

            var module = _typedModuleDefs[typeof(TModule)];

            if (module is null)
                throw new InvalidOperationException($"{typeof(TModule).FullName} is not loaded to the Slash Command Service");

            return module;
        }

        public void Dispose ( )
        {
            _lock.Dispose();
        }

        private Type GetMostSpecificTypeConverter (Type type)
        {
            var scorePairs = new Dictionary<Type, int>();
            var validConverters = _genericTypeConverters.Where(x => x.Key.IsAssignableFrom(type));

            foreach (var typeConverterPair in validConverters)
            {
                var score = validConverters.Count(x => typeConverterPair.Key.IsAssignableFrom(x.Key));
                scorePairs.Add(typeConverterPair.Value, score);
            }

            return scorePairs.OrderBy(x => x.Value).ElementAt(0).Key;
        }

        private void EnsureClientReady ( )
        {
            if (Client.CurrentUser is null || Client.CurrentUser?.Id == 0)
                throw new InvalidOperationException($"Provided client is not ready to execute this operation, invoke this operation after a `Client Ready` event");
        }
    }
}
