using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Discord.SlashCommands
{
    internal static class ReflectionUtils
    {
        private static readonly TypeInfo ObjectTypeInfo = typeof(object).GetTypeInfo();

        internal static T CreateObject<T> (TypeInfo typeInfo, SlashCommandService commandService, IServiceProvider services = null) =>
            CreateBuilder<T>(typeInfo, commandService)(services);

        internal static Func<IServiceProvider, T> CreateBuilder<T>(TypeInfo typeInfo, SlashCommandService commandService)
        {
            var constructor = GetConstructor(typeInfo);
            var parameters = constructor.GetParameters();
            var properties = GetProperties(typeInfo);

            return (services) =>
            {
                var args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    args[i] = GetMember(commandService, services, parameters[i].ParameterType, typeInfo);

                var obj = InvokeConstructor<T>(constructor, args, typeInfo);
                foreach (var property in properties)
                    property.SetValue(obj, GetMember(commandService, services, property.PropertyType, typeInfo));
                return obj;
            };
        }

        private static T InvokeConstructor<T> (ConstructorInfo constructor, object[] args, TypeInfo ownerType)
        {
            try
            {
                return (T)constructor.Invoke(args);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create \"{ownerType.FullName}\".", ex);
            }
        }
        private static ConstructorInfo GetConstructor (TypeInfo ownerType)
        {
            var constructors = ownerType.DeclaredConstructors.Where(x => !x.IsStatic).ToArray();
            if (constructors.Length == 0)
                throw new InvalidOperationException($"No constructor found for \"{ownerType.FullName}\".");
            else if (constructors.Length > 1)
                throw new InvalidOperationException($"Multiple constructors found for \"{ownerType.FullName}\".");
            return constructors[0];
        }
        private static PropertyInfo[] GetProperties (TypeInfo ownerType)
        {
            var result = new List<PropertyInfo>();
            while (ownerType != ObjectTypeInfo)
            {
                foreach (var prop in ownerType.DeclaredProperties)
                {
                    if (prop.SetMethod?.IsStatic == false && prop.SetMethod?.IsPublic == true)
                        result.Add(prop);
                }
                ownerType = ownerType.BaseType.GetTypeInfo();
            }
            return result.ToArray();
        }
        private static object GetMember (SlashCommandService commandService, IServiceProvider services, Type memberType, TypeInfo ownerType)
        {
            if (memberType == typeof(SlashCommandService))
                return commandService;
            if (memberType == typeof(IServiceProvider) || memberType == services.GetType())
                return services;
            var service = services.GetService(memberType);
            if (service != null)
                return service;
            throw new InvalidOperationException($"Failed to create \"{ownerType.FullName}\", dependency \"{memberType.Name}\" was not found.");
        }

        public static T Create<T> (params object[] args)
        {
            var types = args.Select(p => p.GetType()).ToArray();
            var ctor = typeof(T).GetConstructor(types);

            var exnew = Expression.New(ctor);
            var lambda = Expression.Lambda<T>(exnew);
            var compiled = lambda.Compile();
            return compiled;
        }

        public static object Create(Type type, params object[] args)
        {
            var types = args.Select(p => p.GetType()).ToArray();
            var ctor = type.GetConstructor(types);

            var exnew = Expression.New(ctor);
            var lambda = Expression.Lambda(exnew);
            var compiled = lambda.Compile();
            return compiled;
        }

        public delegate object ObjectActivator (params object[] args);

        public static object New (this Type input, params object[] args )
        {
            var types = args.Select(x => x?.GetType());
            var constructor = input.GetConstructor(types.ToArray());

            var paramInfo = constructor.GetParameters();
            var paramex = Expression.Parameter(typeof(object[]), "args");

            var argex = new Expression[paramInfo.Length];
            for(var i = 0; i < paramInfo.Length; i++)
            {
                var index = Expression.Constant(i);
                var paramType = paramInfo[i].ParameterType;
                var accessor = Expression.ArrayIndex(paramex, index);
                var cast = Expression.Convert(accessor, paramType);
                argex[i] = cast;
            }

            var newex = Expression.New(constructor, argex);
            var lambda = Expression.Lambda(typeof(ObjectActivator), newex, paramex);
            var result = (ObjectActivator)lambda.Compile();
            return result(args);
        }
        public class Type<T>
        {
            public static T New (params object[] args)
            {
                return (T)typeof(T).New(args);
            }
        }
    }
}
