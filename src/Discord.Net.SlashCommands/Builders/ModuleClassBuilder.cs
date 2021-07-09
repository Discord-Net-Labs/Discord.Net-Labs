using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.SlashCommands.Builders
{
    internal static class ModuleClassBuilder
    {
        private static readonly TypeInfo ModuleTypeInfo = typeof(ISlashModuleBase).GetTypeInfo();

        public static async Task<IEnumerable<TypeInfo>> SearchAsync (Assembly assembly, SlashCommandService commandService)
        {
            bool IsLoadableModule (TypeInfo info)
            {
                return info.DeclaredMethods.Any(x => x.GetCustomAttribute<SlashCommandAttribute>() != null);
            }

            var result = new List<TypeInfo>();

            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsPublic || type.IsNestedPublic)
                {
                    if (IsValidModuleDefinition(type))
                    {
                        result.Add(type);
                    }
                }
                else if (IsLoadableModule(type))
                {
                    await commandService._cmdLogger.WarningAsync($"Class {type.FullName} is not public and cannot be loaded.").ConfigureAwait(false);
                }
            }
            return result;
        }

        public static async Task<Dictionary<Type, SlashModuleInfo>> BuildAsync (IEnumerable<TypeInfo> validTypes, SlashCommandService commandService,
            IServiceProvider services)
        {
            var topLevelGroups = validTypes.Where(x => x.DeclaringType == null || !IsValidModuleDefinition(x.DeclaringType.GetTypeInfo()));
            var built = new List<TypeInfo>();

            var result = new Dictionary<Type, SlashModuleInfo>();

            foreach (var type in validTypes)
            {
                var builder = new SlashModuleBuilder(commandService);

                BuildModule(builder, type, commandService, services);
                built.Add(type);

                result.Add(type.AsType(), builder.Build());
            }

            await commandService._cmdLogger.DebugAsync($"Successfully built {built.Count} slash command modules.").ConfigureAwait(false);

            return result;
        }

        private static void BuildModule (SlashModuleBuilder builder, TypeInfo typeInfo, SlashCommandService commandService,
            IServiceProvider services)
        {
            var attributes = typeInfo.GetCustomAttributes();
            builder.TypeInfo = typeInfo;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case SlashGroupAttribute group:
                        {
                            if (!string.IsNullOrEmpty(group.Name))
                                builder.Name = group.Name;
                            if (!string.IsNullOrEmpty(group.Description))
                                builder.Description = group.Description;
                        }
                        break;
                    case DefaultPermissionAttribute defPermission:
                        {
                            builder.DefaultPermission = defPermission.Allow;
                        }
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }

            var methods = typeInfo.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var validCommands = methods.Where(IsValidCommandDefinition);
            var validInternalCommands = methods.Where(IsValidInteractionDefinition);

            foreach (var method in validCommands)
                builder.AddCommand(x => BuildCommand(x, typeInfo, method, commandService, services));

            foreach (var method in validInternalCommands)
                builder.AddInteraction(x => BuildInteraction(x, typeInfo, method, commandService, services));
        }

        private static void BuildCommand (SlashCommandBuilder builder, TypeInfo typeInfo, MethodInfo methodInfo,
            SlashCommandService commandService, IServiceProvider services)
        {
            var attributes = methodInfo.GetCustomAttributes();

            builder.Name = methodInfo.Name;
            builder.Description = methodInfo.Name;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case SlashGroupAttribute group:
                        {
                            builder.WithGroup(group.Name, group.Description);
                        }
                        break;
                    case SlashCommandAttribute command:
                        {
                            if (!string.IsNullOrEmpty(command.Name))
                                builder.Name = command.Name;

                            if (!string.IsNullOrEmpty(command.Description))
                                builder.Description = command.Description;
                        }
                        break;
                    case DefaultPermissionAttribute defaultPermission:
                        {
                            builder.DefaultPermission = defaultPermission.Allow;
                        }
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }

            var parameters = methodInfo.GetParameters();

            foreach (var parameter in parameters)
                builder.AddParameter(x => BuildParameter(x, parameter, commandService, services));

            var createInstance = ReflectionUtils.CreateBuilder<ISlashModuleBase>(typeInfo, commandService);

            async Task<IResult> ExecuteCallback (ISlashCommandContext context, object[] args, IServiceProvider serviceProvider, SlashCommandInfo commandInfo)
            {
                var instance = createInstance(serviceProvider);
                instance.SetContext(context);

                try
                {
                    instance.BeforeExecute(commandInfo);
                    var task = methodInfo.Invoke(instance, args) as Task ?? Task.Delay(0);
                    await task.ConfigureAwait(false);
                    return ExecuteResult.FromSuccess();
                }
                catch (Exception ex)
                {
                    await commandService._cmdLogger.ErrorAsync(ex);
                    return ExecuteResult.FromError(ex);
                }
                finally
                {
                    instance.AfterExecute(commandInfo);
                    ( instance as IDisposable )?.Dispose();
                }
            }

            builder.Callback = ExecuteCallback;
        }

        private static void BuildInteraction (SlashInteractionBuilder builder, TypeInfo typeInfo, MethodInfo methodInfo,
            SlashCommandService commandService, IServiceProvider services)
        {
            var attributes = methodInfo.GetCustomAttributes();

            builder.Name = methodInfo.Name;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case InteractionAttribute interaction:
                        builder.Name = interaction.CustomId;
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }

            var parameters = methodInfo.GetParameters();

            foreach (var parameter in parameters)
                builder.AddParameter(parameter);

            var createInstance = ReflectionUtils.CreateBuilder<ISlashModuleBase>(typeInfo, commandService);

            async Task<IResult> ExecuteCallback (ISlashCommandContext context, object[] args, IServiceProvider serviceProvider, SlashInteractionInfo commandInfo)
            {
                var instance = createInstance(serviceProvider);
                instance.SetContext(context);

                try
                {
                    instance.BeforeExecute(null);
                    var task = methodInfo.Invoke(instance, args) as Task ?? Task.Delay(0);
                    await task.ConfigureAwait(false);
                    return ExecuteResult.FromSuccess();
                }
                catch (Exception ex)
                {
                    await commandService._cmdLogger.ErrorAsync(ex);
                    return ExecuteResult.FromError(ex);
                }
                finally
                {
                    instance.AfterExecute(null);
                    ( instance as IDisposable )?.Dispose();
                }
            }

            builder.Callback = ExecuteCallback;
        }

        private static void BuildParameter (SlashParameterBuilder builder, ParameterInfo paramInfo, SlashCommandService commandService,
            IServiceProvider services)
        {
            var attributes = paramInfo.GetCustomAttributes();
            var paramType = paramInfo.ParameterType;

            builder.Name = paramInfo.Name;
            builder.Description = paramInfo.Name;
            builder.IsRequired = !paramInfo.IsOptional;
            builder.DefaultValue = paramInfo.DefaultValue;
            builder.ParameterType = paramType;
            builder.TypeReader = GetTypeReader(paramType, commandService);

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case DescriptionAttribute description:
                        {
                            if (!string.IsNullOrEmpty(description.Name))
                                builder.Name = description.Name;

                            if (!string.IsNullOrEmpty(description.Description))
                                builder.Description = description.Description;
                        }
                        break;
                    case ChoiceAttribute choice:
                        {
                            if (choice.Value is string str)
                                builder.AddOptions(new ParameterChoice(choice.Name, str));
                            else if (choice.Value is int integer)
                                builder.AddOptions(new ParameterChoice(choice.Name, integer));
                            else
                                throw new InvalidOperationException("Parameter choice must either be an integer or a string");
                        }
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }
        }

        private static Func<ISlashCommandContext, InteractionParameter, IServiceProvider, object> GetTypeReader ( Type type, SlashCommandService commandService )
        {
            var discordType = SlashCommandUtility.GetDiscordOptionType(type);

            var reader = commandService.TypeReaders[discordType];

            if (reader == null)
                throw new InvalidOperationException($"There is no registered type reader for type {nameof(type)}");
            return reader;
        }

        private static bool IsValidModuleDefinition (TypeInfo typeInfo)
        {
            return ModuleTypeInfo.IsAssignableFrom(typeInfo) &&
                   !typeInfo.IsAbstract &&
                   !typeInfo.ContainsGenericParameters;
        }

        private static bool IsValidCommandDefinition (MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(SlashCommandAttribute)) &&
                   ( methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>) ) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }

        private static bool IsValidInteractionDefinition (MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(InteractionAttribute)) &&
                   ( methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>) ) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }
    }
}
