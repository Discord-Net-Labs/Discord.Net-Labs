using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public enum GuildScheduledEventPrivacyLevel
    {
        /// <summary>
        ///     The scheduled event is public and available in discovery.
        /// </summary>
        Public = 1,

        /// <summary>
        ///     The scheduled event is only accessible to guild members.
        /// </summary>
        Private = 2,
    }
}
