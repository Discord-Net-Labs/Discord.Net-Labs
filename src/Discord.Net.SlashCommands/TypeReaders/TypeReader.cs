using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    public abstract class TypeReader
    {
        public abstract bool CanConvertTo (Type type);
        public abstract ApplicationCommandOptionType GetDiscordType ();
        public abstract Task<TypeReaderResult> ReadAsync (ISlashCommandContext context, SocketSlashCommandDataOption option, IServiceProvider services);
        public virtual void Write(ApplicationCommandOptionProperties properties) { }
    }

    public abstract class TypeReader<T> : TypeReader
    {
        public sealed override bool CanConvertTo (Type type) =>
            typeof(T).IsAssignableFrom(type);
    }
}
