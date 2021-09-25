using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket
{
    public static class WebSocketExtensions
    {
        /// <summary>
        ///     Get the name of the executed command and its parents in hierarchical order
        /// </summary>
        /// <param name="command"></param>
        /// <returns>
        ///     The name of the executed command and its parents in hierarchical order
        /// </returns>
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
