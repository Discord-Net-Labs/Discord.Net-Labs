using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents the different type of application commands.
    /// </summary>
    public enum ApplicationCommandType
    {
        /// <summary>
        ///     Slash commands; a text-based command that shows up when a user types /
        /// </summary>
        ChatInput = 1,

        /// <summary>
        ///     A UI-based command that shows up when you right click or tap on a user
        /// </summary>
        User = 2,

        /// <summary>
        ///     A UI-based command that shows up when you right click or tap on a messages
        /// </summary>
        Message = 3,
    }
}
