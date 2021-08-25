using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    internal abstract class DefaultEntityTypeReader<T> : TypeReader<T> where T : class
    {
        public override Task<TypeReaderResult> ReadAsync (ISlashCommandContext context, SocketSlashCommandDataOption option, IServiceProvider services)
        {
            if (option.Value != null)
                return Task.FromResult(TypeReaderResult.FromSuccess(option.Value as T));
            else
                return Task.FromResult(TypeReaderResult.FromError(SlashCommandError.ParseFailed, $"Provided input cannot be read as {nameof(IChannel)}"));
        }
    }

    internal class DefaultRoleReader<T> : DefaultEntityTypeReader<T> where T : class, IRole
    {
        public override ApplicationCommandOptionType GetDiscordType () => ApplicationCommandOptionType.Role;
    }

    internal class DefaultUserReader<T> : DefaultEntityTypeReader<T> where T : class, IUser
    {
        public override ApplicationCommandOptionType GetDiscordType () => ApplicationCommandOptionType.User;
    }

    internal class DefaultChannelReader<T> : DefaultEntityTypeReader<T> where T : class, IChannel
    {
        public override ApplicationCommandOptionType GetDiscordType () => ApplicationCommandOptionType.Channel;
    }

    internal class DefaultMentionableReader<T> : DefaultEntityTypeReader<T> where T : class, IMentionable
    {
        public override ApplicationCommandOptionType GetDiscordType () => ApplicationCommandOptionType.Mentionable;
    }
}
