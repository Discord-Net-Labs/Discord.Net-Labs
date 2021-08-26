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

        public static async Task<Dictionary<Type, ModuleInfo>> BuildAsync (IEnumerable<TypeInfo> validTypes, SlashCommandService commandService,
            IServiceProvider services)
        {
            var topLevelGroups = validTypes.Where(x => x.DeclaringType == null || !IsValidModuleDefinition(x.DeclaringType.GetTypeInfo()));
            var built = new List<TypeInfo>();

            var result = new Dictionary<Type, ModuleInfo>();

            foreach (var type in topLevelGroups)
            {
                var builder = new ModuleBuilder(commandService);

                BuildModule(builder, type, commandService, services);
                BuildSubModules(builder, type.DeclaredNestedTypes, built, commandService, services);
                built.Add(type);

                result.Add(type.AsType(), builder.Build());
            }

            await commandService._cmdLogger.DebugAsync($"Successfully built {built.Count} Slash Command modules.").ConfigureAwait(false);

            return result;
        }

        private static void BuildModule (ModuleBuilder builder, TypeInfo typeInfo, SlashCommandService commandService,
            IServiceProvider services)
        {
            builder.Name = typeInfo.Name;
            var attributes = typeInfo.GetCustomAttributes();
            builder.TypeInfo = typeInfo;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case SlashGroupAttribute group:
                        {
                            if (!string.IsNullOrEmpty(group.Name))
                                builder.SlashGroupName = group.Name;
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

            var validSlashCommands = methods.Where(IsValidSlashCommandDefinition);
            var validContextCommands = methods.Where(IsValidContextCommandDefinition);
            var validInteractions = methods.Where(IsValidInteractionDefinition);

            foreach (var method in validSlashCommands)
                builder.AddSlashCommand(x => BuildSlashCommand(x, typeInfo, method, commandService, services));

            foreach (var method in validContextCommands)
                builder.AddContextCommand(x => BuildContextCommand(x, typeInfo, method, commandService, services));

            foreach (var method in validInteractions)
                builder.AddInteraction(x => BuildInteraction(x, typeInfo, method, commandService, services));
        }

        private static void BuildSubModules (ModuleBuilder parent, IEnumerable<TypeInfo> subModules, IList<TypeInfo> builtTypes, SlashCommandService commandService,
            IServiceProvider services, int slashGroupDepth = 0)
        {
            foreach (var submodule in subModules.Where(IsValidModuleDefinition))
            {
                if (builtTypes.Contains(submodule))
                    continue;

                parent.AddModule((builder) =>
                {
                    BuildModule(builder, submodule, commandService, services);

                    if (parent.IsSlashGroup)
                        slashGroupDepth++;

                    if (slashGroupDepth >= 2)
                        throw new InvalidOperationException("Slash Commands only support 2 command prefixes for sub-commands");

                    BuildSubModules(builder, submodule.DeclaredNestedTypes, builtTypes, commandService, services, builder.Name != null ? ++slashGroupDepth : slashGroupDepth);
                });
                builtTypes.Add(submodule);
            }
        }

        private static void BuildSlashCommand (SlashCommandBuilder builder, TypeInfo typeInfo, MethodInfo methodInfo,
            SlashCommandService commandService, IServiceProvider services)
        {
            var attributes = methodInfo.GetCustomAttributes();

            builder.Name = methodInfo.Name;
            builder.Description = methodInfo.Name;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
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
                builder.AddParameter(x => BuildSlashParameter(x, parameter, commandService, services));

            builder.Callback = CreateCallback(typeInfo, methodInfo, commandService, services);
        }

        private static void BuildContextCommand (ContextCommandBuilder builder, TypeInfo typeInfo, MethodInfo methodInfo,
            SlashCommandService commandService, IServiceProvider services)
        {
            var attributes = methodInfo.GetCustomAttributes();

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ContextCommandAttribute command:
                        {
                            if (!string.IsNullOrEmpty(command.Name))
                                builder.Name = command.Name;

                            builder.CommandType = command.CommandType;

                            command.CheckMethodDefinition(methodInfo);
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
                builder.AddParameter(parameter);

            builder.Callback = CreateCallback(typeInfo, methodInfo, commandService, services);
        }

        private static void BuildInteraction (InteractionBuilder builder, TypeInfo typeInfo, MethodInfo methodInfo,
            SlashCommandService commandService, IServiceProvider services)
        {
            if (!methodInfo.GetParameters().All(x => x.ParameterType == typeof(string) || x.ParameterType == typeof(string[])))
                throw new InvalidOperationException($"Interaction method parameters all must be types of {typeof(string).Name} or {typeof(string[]).Name}");

            var attributes = methodInfo.GetCustomAttributes();

            builder.Name = methodInfo.Name;

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case InteractionAttribute interaction:
                        {
                            builder.Name = interaction.CustomId;
                        }
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }

            var parameters = methodInfo.GetParameters();

            foreach (var parameter in parameters)
                builder.AddParameter(parameter);

            builder.Callback = CreateCallback(typeInfo, methodInfo, commandService, services);
        }

        private static Func<ISlashCommandContext, object[], IServiceProvider, ExecutableInfo, Task<IResult>> CreateCallback (TypeInfo typeInfo, MethodInfo methodInfo,
            SlashCommandService commandService, IServiceProvider services)
        {
            var createInstance = ReflectionUtils.CreateBuilder<ISlashModuleBase>(typeInfo, commandService);

            async Task<IResult> ExecuteCallback (ISlashCommandContext context, object[] args, IServiceProvider serviceProvider, ExecutableInfo commandInfo)
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

            return ExecuteCallback;
        }

        private static void BuildSlashParameter (SlashParameterBuilder builder, ParameterInfo paramInfo, SlashCommandService commandService,
            IServiceProvider services)
        {
            var attributes = paramInfo.GetCustomAttributes();
            var paramType = paramInfo.ParameterType;

            builder.Name = paramInfo.Name;
            builder.Description = paramInfo.Name;
            builder.IsRequired = !paramInfo.IsOptional;
            builder.DefaultValue = paramInfo.DefaultValue;
            builder.WithType(paramType);

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case SummaryAttribute description:
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
                                throw new InvalidOperationException("Parameter choice must either be an integer, a string or a double");
                        }
                        break;
                    default:
                        builder.AddAttributes(attribute);
                        break;
                }
            }
        }

        internal static bool IsValidModuleDefinition (TypeInfo typeInfo)
        {
            return ModuleTypeInfo.IsAssignableFrom(typeInfo) &&
                   !typeInfo.IsAbstract &&
                   !typeInfo.ContainsGenericParameters;
        }

        private static bool IsValidSlashCommandDefinition (MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(SlashCommandAttribute)) &&
                   ( methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>) ) &&
                   !methodInfo.IsStatic &&
                   !methodInfo.IsGenericMethod;
        }

        private static bool IsValidContextCommandDefinition (MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(ContextCommandAttribute)) &&
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
