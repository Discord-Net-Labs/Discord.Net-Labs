using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents an interface used to specify classes that they are a vaild dataype of a <see cref="IDiscordInteraction"/> class.
    /// </summary>
    public interface IDiscordInteractionData : IEntity<ulong>
    {
        /// <summary>
        ///     Gets the name of the invoked command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the type of the invoked command.
        /// </summary>
        ApplicationCommandType Type { get; }
    }
}
