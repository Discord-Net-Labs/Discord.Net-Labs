using Discord.Interactions.Builders;
using Discord.Logging;
using Discord.Rest;
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
    ///     Provides the framework for building and registering Discord Application Commands
    /// </summary>
    public class InteractionService : IDisposable
    {
        /// <summary>
        ///     Occurs when a Slash Command related information is recieved
        /// </summary>
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        /// <summary>
        ///     Occurs when a Slash Command is executed
        /// </summary>
        public event Func<SlashCommandInfo, IInteractionCommandContext, IResult, Task> SlashCommandExecuted { add { _slashCommandExecutedEvent.Add(value); } remove { _slashCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<SlashCommandInfo, IInteractionCommandContext, IResult, Task>> _slashCommandExecutedEvent = new AsyncEvent<Func<SlashCommandInfo, IInteractionCommandContext, IResult, Task>>();

        /// <summary>
        ///     Occurs when a Context Command is executed
        /// </summary>
        public event Func<ContextCommandInfo, IInteractionCommandContext, IResult, Task> ContextCommandExecuted { add { _contextCommandExecutedEvent.Add(value); } remove { _contextCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<ContextCommandInfo, IInteractionCommandContext, IResult, Task>> _contextCommandExecutedEvent = new AsyncEvent<Func<ContextCommandInfo, IInteractionCommandContext, IResult, Task>>();

        /// <summary>
        ///     Occurs when a Message Component command is executed
        /// </summary>
        public event Func<ComponentCommandInfo, IInteractionCommandContext, IResult, Task> ComponentCommandExecuted { add { _componentCommandExecutedEvent.Add(value); } remove { _componentCommandExecutedEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<ComponentCommandInfo, IInteractionCommandContext, IResult, Task>> _componentCommandExecutedEvent = new AsyncEvent<Func<ComponentCommandInfo, IInteractionCommandContext, IResult, Task>>();

        private readonly ConcurrentDictionary<Type, ModuleInfo> _typedModuleDefs;
        private readonly CommandMap<SlashCommandInfo> _slashCommandMap;
        private readonly ConcurrentDictionary<ApplicationCommandType, CommandMap<ContextCommandInfo>> _contextCommandMaps;
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
        ///     Represents all modules loaded within <see cref="InteractionService"/>
        /// </summary>
        public IReadOnlyList<ModuleInfo> Modules => _moduleDefs.ToList();

        /// <summary>
        ///     Represents all Slash Commands loaded within <see cref="InteractionService"/>
        /// </summary>
        public IReadOnlyList<SlashCommandInfo> SlashCommands => _moduleDefs.SelectMany(x => x.SlashCommands).ToList();

        /// <summary>
        ///     Represents all Context Commands loaded within <see cref="InteractionService"/>
        /// </summary>
        public IReadOnlyList<ContextCommandInfo> ContextCommands => _moduleDefs.SelectMany(x => x.ContextCommands).ToList();

        /// <summary>
        ///     Represents all Component Commands loaded within <see cref="InteractionService"/>
        /// </summary>
        public IReadOnlyCollection<ComponentCommandInfo> ComponentCommands => _moduleDefs.SelectMany(x => x.ComponentCommands).ToList();

        /// <summary>
        ///     Underlying Discord Client
        /// </summary>
        public BaseSocketClient Client { get; }

        /// <summary>
        ///     Initialize a <see cref="InteractionService"/> with the default configurations
        /// </summary>
        /// <param name="discord">The discord client</param>
        public InteractionService (BaseSocketClient discord) : this(discord, new InteractionServiceConfig()) { }

        /// <summary>
        ///     Initialize a <see cref="InteractionService"/> with provided configurations
        /// </summary>
        /// <param name="discord">The discord client</param>
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
            _contextCommandMaps = new ConcurrentDictionary<ApplicationCommandType, CommandMap<ContextCommandInfo>>();
            _componentCommandMap = new CommandMap<ComponentCommandInfo>(this, config.InteractionCustomIdDelimiters);

            Client = discord;

            _runMode = config.DefaultRunMode;
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
        ///     Discover and load command modules from an <see cref="Assembly"/>
        /// </summary>
        /// <param name="assembly"><see cref="Assembly"/> the command modules are defined in</param>
        /// <param name="services">The <see cref="IServiceProvider"/> for your dependency injection solution if using one; otherwise, pass <c>null</c>.</param>
        /// <returns>
        ///     A task representing the operation for adding modules. The task result contains a collection of the modules added.
        /// </returns>
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
        ///     Add a command module from a <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">Type of the module</typeparam>
        /// <param name="services">The <see cref="IServiceProvider" /> for your dependency injection solution if using one; otherwise, pass <c>null</c> .</param>
        /// <returns>
        ///     A task representing the operation for adding the module. The task result contains the built module
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if this module has already been added.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the <typeparamref name="T"/> is not a valid module definition
        /// </exception>
        public async Task<ModuleInfo> AddModuleAsync<T> (IServiceProvider services) where T : class
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

                var moduleDef = ( await ModuleClassBuilder.BuildAsync(new List<TypeInfo> { typeof(T).GetTypeInfo() }, this, services).ConfigureAwait(false) ).FirstOrDefault();

                if (moduleDef.Value == default)
                    throw new InvalidOperationException($"Could not build the module {typeInfo.FullName}, did you pass an invalid type?");

                if (!_typedModuleDefs.TryAdd(typeof(T), moduleDef.Value))
                    throw new ArgumentException("Module definition for this type already exists.");

                LoadModuleInternal(moduleDef.Value);

                return moduleDef.Value;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        ///     Register Application Commands from <see cref="ContextCommands"/> and <see cref="SlashCommands"/> to a guild 
        /// </summary>
        /// <param name="guildId">Id of the target guild</param>
        /// <param name="deleteMissing">If <see langword="false"/>, this operation will not delete the commands that are missing from <see cref="InteractionService"/></param>
        /// <returns>
        ///     A task representing the command registration process. The task result contains the active application commands of the target guild
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> RegisterCommandsToGuildAsync (ulong guildId, bool deleteMissing = true)
        {
            EnsureClientReady();

            var props = _typedModuleDefs.Values.SelectMany(x => x.ToApplicationCommandProps()).ToList();

            if (!deleteMissing)
            {

                var existing = await Client.Rest.GetGuildApplicationCommands(guildId).ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await Client.Rest.BulkOverwriteGuildCommands(props.ToArray(), guildId).ConfigureAwait(false);
        }

        /// <summary>
        ///     Register Application Commands from <see cref="ContextCommands"/> and <see cref="SlashCommands"/> to Discord on in global scope
        /// </summary>
        /// <param name="deleteMissing">If <see langword="false"/>, this operation will not delete the commands that are missing from <see cref="InteractionService"/></param>
        /// <returns>
        ///    A task representing the command registration process. The task result contains the active global application commands of bot
        /// </returns>
        public async Task<IReadOnlyCollection<RestGlobalCommand>> RegisterCommandsGloballyAsync (bool deleteMissing = true)
        {
            EnsureClientReady();

            var props = _typedModuleDefs.Values.SelectMany(x => x.ToApplicationCommandProps()).ToList();

            if (!deleteMissing)
            {
                var existing = await Client.Rest.GetGlobalApplicationCommands().ConfigureAwait(false);
                var missing = existing.Where(x => !props.Any(y => y.Name.IsSpecified && y.Name.Value == x.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await Client.Rest.BulkOverwriteGlobalCommands(props.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        ///     Register Application Commands from <paramref name="commands"/> to a guild 
        /// </summary>
        /// <remarks>
        ///     Commands will be registered as standalone commands, if you want the <see cref="GroupAttribute"/> to take effect,
        ///     use <see cref="AddModulesToGuildAsync(IGuild, ModuleInfo[])"/>
        /// </remarks>
        /// <param name="guild">The target guild</param>
        /// <param name="commands">Commands to be registered to Discord</param>
        /// <returns>
        ///     A task representing the command registration process. The task result contains the active application commands of the target guild
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> AddCommandsToGuildAsync (IGuild guild, params IApplicationCommandInfo[] commands)
        {
            EnsureClientReady();

            if (guild is null)
                throw new ArgumentNullException(nameof(guild));

            var existing = await Client.Rest.GetGuildApplicationCommands(guild.Id).ConfigureAwait(false);

            var props = new List<ApplicationCommandProperties>();

            foreach (var command in commands)
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
                var missing = existing.Where(oldCommand => !props.Any(newCommand => newCommand.Name.IsSpecified && newCommand.Name.Value == oldCommand.Name));
                props.AddRange(missing.Select(x => x.ToApplicationCommandProps()));
            }

            return await Client.Rest.BulkOverwriteGuildCommands(props.ToArray(), guild.Id).ConfigureAwait(false);
        }

        /// <summary>
        ///     Register Application Commands from modules provided in <paramref name="modules"/> to a guild 
        /// </summary>
        /// <param name="guild">The target guild</param>
        /// <param name="modules">Modules to be registered to Discord</param>
        /// <returns>
        ///     A task representing the command registration process. The task result contains the active application commands of the target guild
        /// </returns>
        public async Task<IReadOnlyCollection<RestGuildCommand>> AddModulesToGuildAsync (IGuild guild, params ModuleInfo[] modules)
        {
            EnsureClientReady();

            if (guild is null)
                throw new ArgumentNullException(nameof(guild));

            var existing = await Client.Rest.GetGuildApplicationCommands(guild.Id).ConfigureAwait(false);
            var props = modules.SelectMany(x => x.ToApplicationCommandProps(true)).ToList();

            foreach (var command in existing)
                props.Add(command.ToApplicationCommandProps());

            return await Client.Rest.BulkOverwriteGuildCommands(props.ToArray(), guild.Id).ConfigureAwait(false);
        }

        private void LoadModuleInternal (ModuleInfo module)
        {
            _moduleDefs.Add(module);

            foreach (var command in module.SlashCommands)
                _slashCommandMap.AddCommand(command, command.IgnoreGroupNames);

            foreach (var command in module.ContextCommands)
                _contextCommandMaps.GetOrAdd(command.CommandType, new CommandMap<ContextCommandInfo>(this)).AddCommand(command, command.IgnoreGroupNames);

            foreach (var interaction in module.ComponentCommands)
                _componentCommandMap.AddCommand(interaction, interaction.IgnoreGroupNames);

            foreach (var subModule in module.SubModules)
                LoadModuleInternal(subModule);
        }

        /// <summary>
        ///     Remove a command module
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the module.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation. The task result contains a value that
        ///     indicates whether the module is successfully removed.
        /// </returns>
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
        ///     Execute a Command from a given <see cref="IInteractionCommandContext"/>
        /// </summary>
        /// <param name="context">Name context of the command</param>
        /// <param name="services">The service to be used in the command's dependency injection.</param>
        /// <returns>
        ///     A task representing the command execution process. The task result contains the result of the execution
        /// </returns>
        public async Task<IResult> ExecuteCommandAsync (IInteractionCommandContext context, IServiceProvider services)
        {
            var interaction = context.Interaction;

            switch (interaction)
            {
                case SocketSlashCommand slashCommand:
                    return await ExecuteSlashCommandAsync(context, slashCommand.Data, services).ConfigureAwait(false);
                case SocketMessageComponent messageComponent:
                    return await ExecuteComponentCommandAsync(context, messageComponent.Data.CustomId, services).ConfigureAwait(false);
                case SocketUserCommand userCommand:
                    return await ExecuteContextCommandAsync(context, userCommand.CommandName, ApplicationCommandType.User, services).ConfigureAwait(false);
                case SocketMessageCommand messageCommand:
                    return await ExecuteContextCommandAsync(context, messageCommand.CommandName, ApplicationCommandType.Message, services).ConfigureAwait(false);
                default:
                    throw new InvalidOperationException($"{interaction.Type} interaction type cannot be executed by the Interaction service");
            }
        }

        private async Task<IResult> ExecuteSlashCommandAsync (IInteractionCommandContext context, SocketSlashCommandData data, IServiceProvider services)
        {
            var keywords = data.GetCommandKeywords();

            var result = _slashCommandMap.GetCommand(keywords);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown slash command, skipping execution ({string.Join(" ", keywords).ToUpper()})");

                if (_deleteUnkownSlashCommandAck)
                {
                    var response = await context.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                    await response.DeleteAsync().ConfigureAwait(false);
                }

                return result;
            }
            return await result.Command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        private async Task<IResult> ExecuteContextCommandAsync (IInteractionCommandContext context, string input, ApplicationCommandType commandType, IServiceProvider services)
        {
            if (!_contextCommandMaps.TryGetValue(commandType, out var map))
                return SearchResult<ContextCommandInfo>.FromError(input, InteractionCommandError.UnknownCommand, $"No {commandType} command found.");

            var result = map.GetCommand(input);

            if (!result.IsSuccess)
            {
                await _cmdLogger.DebugAsync($"Unknown context command, skipping execution ({result.Text.ToUpper()})");

                return result;
            }
            return await result.Command.ExecuteAsync(context, services).ConfigureAwait(false);
        }

        private async Task<IResult> ExecuteComponentCommandAsync (IInteractionCommandContext context, string input, IServiceProvider services)
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
        ///     Add a concrete type <see cref="TypeConverter"/>
        /// </summary>
        /// <typeparam name="T">Primary target <see cref="Type"/> of the <see cref="TypeConverter"/></typeparam>
        /// <param name="converter">The <see cref="TypeConverter"/> instance</param>
        public void AddTypeConverter<T> (TypeConverter converter) =>
            AddTypeConverter(typeof(T), converter);

        /// <summary>
        ///     Add a concrete type <see cref="TypeConverter"/>
        /// </summary>
        /// <param name="type">Primary target <see cref="Type"/> of the <see cref="TypeConverter"/></param>
        /// <param name="converter">The <see cref="TypeConverter"/> instance</param>
        public void AddTypeConverter (Type type, TypeConverter converter)
        {
            if (!converter.CanConvertTo(type))
                throw new ArgumentException($"This {converter.GetType().FullName} cannot read {type.FullName} and cannot be registered as its {nameof(TypeConverter)}");

            _typeConverters[type] = converter;
        }

        /// <summary>
        ///     Add a generic type <see cref="TypeConverter{T}"/>
        /// </summary>
        /// <typeparam name="T">Generic Type constraint of the <see cref="Type"/> of the <see cref="TypeConverter{T}"/></typeparam>
        /// <typeparam name="TConverter">Type of the <see cref="TypeConverter{T}"/></typeparam>
        public void AddGenericTypeConverter<T, TConverter> ( ) where TConverter : TypeConverter, new() =>
            AddGenericTypeConverter<TConverter>(typeof(T));

        /// <summary>
        ///     Add a generic type <see cref="TypeConverter{T}"/>
        /// </summary>
        /// <param name="type">Generic Type constraint of the <see cref="Type"/> of the <see cref="TypeConverter{T}"/></param>
        /// <typeparam name="TConverter">Type of the <see cref="TypeConverter{T}"/></typeparam>
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
        ///     Modify the command permissions of the matching Discord Slash Command
        /// </summary>
        /// <param name="module">Module representing the top level Slash Command</param>
        /// <param name="guild">Target guild</param>
        /// <param name="permissions">New permission values</param>
        /// <returns>
        ///     The active command permissions after the modification
        /// </returns>
        public async Task<GuildApplicationCommandPermission> ModifySlashCommandPermissionsAsync (ModuleInfo module, IGuild guild,
            params ApplicationCommandPermission[] permissions)
        {
            if (!module.IsSlashGroup)
                throw new InvalidOperationException($"This module does not have a {nameof(GroupAttribute)} and does not represent an Application Command");

            if (!module.IsTopLevelGroup)
                throw new InvalidOperationException("This module is not a top level application command. You cannot change its permissions");

            if (guild is null)
                throw new ArgumentNullException("guild");

            var commands = await Client.Rest.GetGuildApplicationCommands(guild.Id).ConfigureAwait(false);
            var appCommand = commands.First(x => x.Name == module.SlashGroupName);

            return await appCommand.ModifyCommandPermissions(permissions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Modify the command permissions of the matching Discord Slash Command
        /// </summary>
        /// <param name="command">The Slash Command</param>
        /// <param name="guild">Target guild</param>
        /// <param name="permissions">New permission values</param>
        /// <returns>
        ///     The active command permissions after the modification
        /// </returns>
        public async Task<GuildApplicationCommandPermission> ModifySlashCommandPermissionsAsync (SlashCommandInfo command, IGuild guild,
            params ApplicationCommandPermission[] permissions) =>
            await ModifyApplicationCommandPermissionsAsync(command, guild, permissions).ConfigureAwait(false);

        /// <summary>
        ///     Modify the command permissions of the matching Discord Slash Command
        /// </summary>
        /// <param name="command">The Context Command</param>
        /// <param name="guild">Target guild</param>
        /// <param name="permissions">New permission values</param>
        /// <returns>
        ///     The active command permissions after the modification
        /// </returns>
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

            var commands = await Client.Rest.GetGuildApplicationCommands(guild.Id).ConfigureAwait(false);
            var appCommand = commands.First(x => x.Name == ( command as IApplicationCommandInfo ).Name);

            return await appCommand.ModifyCommandPermissions(permissions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Get a <see cref="SlashCommandInfo"/>
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="InteractionModuleBase{T}"/></typeparam>
        /// <param name="methodName">Method name of the handler, use of <see langword="nameof"/> is recommended</param>
        /// <returns>
        ///     <see cref="SlashCommandInfo"/> instance for this command
        /// </returns>
        /// <exception cref="InvalidOperationException">Module or Slash Command couldn't be found</exception>
        public SlashCommandInfo GetSlashCommandInfo<TModule> (string methodName) where TModule : class
        {
            var module = GetModuleInfo<TModule>();

            return module.SlashCommands.First(x => x.MethodName == methodName);
        }

        /// <summary>
        ///     Get a <see cref="ContextCommandInfo"/>
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="InteractionModuleBase{T}"/></typeparam>
        /// <param name="methodName">Method name of the handler, use of <see langword="nameof"/> is recommended</param>
        /// <returns>
        ///     <see cref="ContextCommandInfo"/> instance for this command
        /// </returns>
        /// <exception cref="InvalidOperationException">Module or Context Command couldn't be found</exception>
        public ContextCommandInfo GetContextCommandInfo<TModule> (string methodName) where TModule : class
        {
            var module = GetModuleInfo<TModule>();

            return module.ContextCommands.First(x => x.MethodName == methodName);
        }

        /// <summary>
        ///     Get a <see cref="ComponentCommandInfo"/>
        /// </summary>
        /// <typeparam name="TModule">Declaring module type of this command, must be a type of <see cref="InteractionModuleBase{T}"/></typeparam>
        /// <param name="methodName">Method name of the handler, use of <see langword="nameof"/> is recommended</param>
        /// <returns>
        ///     <see cref="ComponentCommandInfo"/> instance for this command
        /// </returns>
        /// <exception cref="InvalidOperationException">Module or Component Command couldn't be found</exception>
        public ComponentCommandInfo GetInteractionInfo<TModule> (string methodName) where TModule : class
        {
            var module = GetModuleInfo<TModule>();

            return module.ComponentCommands.First(x => x.MethodName == methodName);
        }

        /// <summary>
        ///     Get a built <see cref="ModuleInfo"/>
        /// </summary>
        /// <typeparam name="TModule">Type of the module, must be a type of <see cref="InteractionModuleBase{T}"/></typeparam>
        /// <returns>
        ///     <see cref="ModuleInfo"/> instance for this module
        /// </returns>
        public ModuleInfo GetModuleInfo<TModule> ( ) where TModule : class
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
