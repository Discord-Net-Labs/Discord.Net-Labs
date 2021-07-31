using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic Stage Channel.
    /// </summary>
    public interface IStageChannel : IVoiceChannel
    {
        /// <summary>
        ///     Gets the topic of the Stage instance.
        /// </summary>
        string Topic { get; }

        /// <summary>
        ///     The <see cref="StagePrivacyLevel"/> of the current stage.
        /// </summary>
        StagePrivacyLevel PrivacyLevel { get; }

        /// <summary>
        ///     <see langword="true"/> if stage discovery is disabled, otherwise <see langword="false"/>. 
        /// </summary>
        bool DiscoverableDisabled { get; }

        /// <summary>
        ///     <see langword="true"/> when the stage is live, otherwise <see langword="false"/>.
        /// </summary>
        bool Live { get; }

        /// <summary>
        ///     Gets a collection of users who are speakers within the stage.
        /// </summary>
        IReadOnlyCollection<IGuildUser> Speakers { get; }


    }
}
