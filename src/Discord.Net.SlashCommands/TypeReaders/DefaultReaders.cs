using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Default Type Readers that are used to parse the Slash Command Parameter values into objects
    /// </summary>
    public static class DefaultReaders
    {
        /// <summary>
        /// Default type reader that is used for all of the supported value types and strings
        /// </summary>
        /// <param name="ctx">Command context, the parameter belongs to</param>
        /// <param name="parameter">Parameter to be parsed</param>
        /// <param name="services">Service provider for passing dependencies</param>
        /// <returns>The parse result as an object</returns>
        public static object PrimitiveReader (ISlashCommandContext ctx, InteractionParameter parameter, IServiceProvider services)
        {
            var paramType = SlashCommandUtility.GetParameterType(parameter.Type);

            object value;

            if (parameter.Value is Optional<object> optional)
                value = optional.Value;
            else
                value = parameter.Value;

            if (value is IConvertible)
                return Convert.ChangeType(value, paramType);
            else
                return value;
        }

        /// <summary>
        /// Default type reader that is used when parsing a <see cref="IUser"/> type
        /// </summary>
        /// <param name="ctx">Command context, the parameter belongs to</param>
        /// <param name="parameter">Parameter to be parsed</param>
        /// <param name="services">Service provider for passing dependencies</param>
        /// <returns>The parsed <see cref="IUser"/> is successful, else <see langword="null"/></returns>
        /// <exception cref="ArgumentException">Provided parameter cannot be parsed with the provided type reader</exception>
        public static object UserReader (ISlashCommandContext ctx, InteractionParameter parameter, IServiceProvider services)
        {
            if (ulong.TryParse(parameter.Value.ToString(), out var id))
            {
                if (ctx.Guild != null)
                    return ( ctx.Channel as SocketGuildChannel ).GetUser(id);
                else
                    return ( ctx.Channel as SocketChannel ).GetUser(id);
            }
            else
                throw new ArgumentException($"Provided {nameof(parameter)} cannot be parsed with the provided type reader");
        }

        /// <summary>
        /// Default type reader that is used when parsing a <see cref="IRole"/> type
        /// </summary>
        /// <param name="ctx">Command context, the parameter belongs to</param>
        /// <param name="parameter">Parameter to be parsed</param>
        /// <param name="services">Service provider for passing dependencies</param>
        /// <returns>The parsed <see cref="IRole"/> is successful, else <see langword="null"/></returns>
        /// <exception cref="ArgumentException">Provided parameter cannot be parsed with the provided type reader</exception>
        public static object RoleReader (ISlashCommandContext ctx, InteractionParameter parameter, IServiceProvider services)
        {
            if (ulong.TryParse(parameter.Value.ToString(), out var id))
            {
                if (ctx.Guild != null)
                    return ( ctx.Channel as SocketGuildChannel ).Guild.GetRole(id);
                else
                    return null;
            }
            else
                throw new ArgumentException($"Provided {nameof(parameter)} cannot be parsed with the provided type reader");
        }

        /// <summary>
        /// Default type reader that is used when parsing a <see cref="IChannel"/> type
        /// </summary>
        /// <param name="ctx">Command context, the parameter belongs to</param>
        /// <param name="parameter">Parameter to be parsed</param>
        /// <param name="services">Service provider for passing dependencies</param>
        /// <returns>The parsed <see cref="IChannel"/> is successful, else <see langword="null"/></returns>
        /// <exception cref="ArgumentException">Provided parameter cannot be parsed with the provided type reader</exception>
        public static object ChannelReader (ISlashCommandContext ctx, InteractionParameter parameter, IServiceProvider services)
        {
            if (ulong.TryParse(parameter.Value.ToString(), out var id))
            {
                if (ctx.Guild != null)
                    return ( ctx.Guild as SocketGuild )?.GetChannel(id);
                else
                    return ( ctx.Channel as SocketChannel );
            }
            else
                throw new ArgumentException($"Provided {nameof(parameter)} cannot be parsed with the provided type reader");
        }

        /// <summary>
        /// Default type reader that is used when parsing a <see cref="IMentionable"/> type
        /// </summary>
        /// <remarks>
        /// Since all of the complex object types supported by Application Commands API are <see cref="IMentionable"/>s,
        /// this type of parameter is only registered either when it is explicitly declared, or as fallback
        /// </remarks>
        /// <param name="ctx">Command context, the parameter belongs to</param>
        /// <param name="parameter">Parameter to be parsed</param>
        /// <param name="services">Service provider for passing dependencies</param>
        /// <returns>The parsed <see cref="IMentionable"/> is successful, else <see langword="null"/></returns>
        /// <exception cref="ArgumentException">Provided parameter cannot be parsed with the provided type reader</exception>
        public static object MentionableReader (ISlashCommandContext ctx, InteractionParameter parameter, IServiceProvider services)
        {
            if (ulong.TryParse(parameter.Value.ToString(), out var id))
            {
                if (ctx.Guild == null)
                {
                    return ( ctx.Channel as SocketChannel )?.GetUser(id);
                }
                else
                {
                    var channel = ctx.Channel as SocketGuildChannel;

                    var user = channel?.GetUser(id);
                    if (user != null)
                        return user;
                    var role = channel?.Guild?.GetRole(id);
                    return role;
                }
            }
            else
                throw new ArgumentException($"Provided {nameof(parameter)} cannot be parsed with the provided type reader");
        }

        /// <summary>
        /// Use to insert the default TypeReaders into <see cref="SlashCommandService"/>
        /// </summary>
        /// <param name="map"></param>
        internal static void CreateDefaultTypeReaders (IDictionary<ApplicationCommandOptionType, Func<ISlashCommandContext, InteractionParameter,
            IServiceProvider, object>> map)
        {
            foreach (var type in PrimitiveTypes)
                map.Add(SlashCommandUtility.GetDiscordOptionType(type), PrimitiveReader);

            map.Add(ApplicationCommandOptionType.User, UserReader);
            map.Add(ApplicationCommandOptionType.Role, RoleReader);
            map.Add(ApplicationCommandOptionType.Channel, ChannelReader);
            map.Add(ApplicationCommandOptionType.Mentionable, MentionableReader);
        }

        private static readonly Type[] PrimitiveTypes =
        {
            typeof(string),
            typeof(int),
            typeof(bool)
        };
    }
}
