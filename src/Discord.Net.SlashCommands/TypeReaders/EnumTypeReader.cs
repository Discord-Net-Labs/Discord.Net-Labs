using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    internal sealed class EnumTypeReader<T> : TypeReader<T> where T : Enum
    {
        public override ApplicationCommandOptionType GetDiscordType ( ) => ApplicationCommandOptionType.String;
        public override Task<TypeReaderResult> ReadAsync (ISlashCommandContext context, SocketSlashCommandDataOption option, IServiceProvider services)
        {
            try
            {
                var result = Enum.Parse(typeof(T), (string)option.Value, true);
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            }
            catch (Exception ex)
            {
                return Task.FromResult(TypeReaderResult.FromError(ex));
            }
        }

        public override void Write (ApplicationCommandOptionProperties properties)
        {
            var names = Enum.GetNames(typeof(T));
            if(names.Length <= 25)
            {
                var choices = new List<ApplicationCommandOptionChoiceProperties>();

                foreach (var name in names)
                    choices.Add(new ApplicationCommandOptionChoiceProperties
                    {
                        Name = name,
                        Value = name
                    });

                properties.Choices = choices;
            }
        }
    }
}
