using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal abstract class DefaultEntityTypeConverter<T> : TypeConverter<T> where T : class
    {
        public override Task<TypeConverterResult> ReadAsync (IInteractionCommandContext context, SocketSlashCommandDataOption option, IServiceProvider services)
        {
            if (option.Value != null)
                return Task.FromResult(TypeConverterResult.FromSuccess(option.Value as T));
            else
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Provided input cannot be read as {nameof(IChannel)}"));
        }
    }

    internal class DefaultRoleConverter<T> : DefaultEntityTypeConverter<T> where T : class, IRole
    {
        public override ApplicationCommandOptionType GetDiscordType () => ApplicationCommandOptionType.Role;
    }

    internal class DefaultUserConverter<T> : DefaultEntityTypeConverter<T> where T : class, IUser
    {
        public override ApplicationCommandOptionType GetDiscordType () => ApplicationCommandOptionType.User;
    }

    internal class DefaultChannelConverter<T> : DefaultEntityTypeConverter<T> where T : class, IChannel
    {
        public override ApplicationCommandOptionType GetDiscordType () => ApplicationCommandOptionType.Channel;
    }

    internal class DefaultMentionableConverter<T> : DefaultEntityTypeConverter<T> where T : class, IMentionable
    {
        public override ApplicationCommandOptionType GetDiscordType () => ApplicationCommandOptionType.Mentionable;
    }
}
