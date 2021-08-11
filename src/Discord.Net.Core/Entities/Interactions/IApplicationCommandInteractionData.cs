using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents data of an Interaction Command, see <see href="https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-interaction-data-structure"/>.
    /// </summary>
    public interface IApplicationCommandInteractionData : IDiscordInteractionData
    {
        /// <summary>
        ///     The params + values from the user.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; }
    }
}
