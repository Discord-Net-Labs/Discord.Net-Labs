using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket
{
    public static class WebSocketExtensions
    {
        /// <summary>
        /// Get a collection containing all of the names in the Slash Command hierarchy 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string[] GetCommandKeywords (this SocketSlashCommand command)
        {
            var keywords = new List<string> { command.Data.Name };

            var child = command.Data.Options?.ElementAt(0);

            while (child?.Type == ApplicationCommandOptionType.SubCommandGroup || child?.Type == ApplicationCommandOptionType.SubCommand)
            {
                keywords.Add(child.Name);
                child = child.Options?.ElementAt(0);
            }

            return keywords.ToArray();
        }
    }
}
