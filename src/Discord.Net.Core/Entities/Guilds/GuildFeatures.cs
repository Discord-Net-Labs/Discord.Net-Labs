using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public class GuildFeatures
    {
        /// <summary>
        ///     Gets the flags of recognized features for this guild.
        /// </summary>
        public GuildFeature Value { get; }

        /// <summary>
        ///     Gets a collection of experimental features for this guild. Features that are not contained in <see cref="GuildFeature"/> are put in here.
        /// </summary>
        public IReadOnlyCollection<string> Experimental { get; }

        public bool HasThreads
            => HasFeature(GuildFeature.ThreadsEnabled | GuildFeature.ThreadsEnabledTesting);

        public bool HasTextInVoice
            => HasFeature(GuildFeature.TextInVoiceEnabled);

        public bool IsStaffServer
            => HasFeature(GuildFeature.InternalEmployeeOnly);

        public bool IsHub
            => HasFeature(GuildFeature.Hub);

        public bool IsLinkedToHub
            => HasFeature(GuildFeature.LinkedToHub);

        public bool IsPartnered
            => HasFeature(GuildFeature.Partnered);

        public bool IsVerified
            => HasFeature(GuildFeature.Verified);

        public bool HasVanityUrl
            => HasFeature(GuildFeature.VanityUrl);

        public bool HasRoleSubscriptions
            => HasFeature(GuildFeature.RoleSubscriptionsEnabled | GuildFeature.RoleSubscriptionsAvailableForPurchase);

        public bool HasRoleIcons
            => HasFeature(GuildFeature.RoleIcons);

        public bool HasPrivateThreads
            => HasFeature(GuildFeature.PrivateThreads);

        internal GuildFeatures(GuildFeature value, string[] experimental)
        {
            Value = value;
            Experimental = experimental.ToImmutableArray();
        }

        public bool HasFeature(GuildFeature feature)
            => Value.HasFlag(feature);
        public bool HasFeature(string feature)
            => Experimental.Contains(feature);

        internal void EnsureFeature(GuildFeature feature)
        {
            if (!HasFeature(feature))
            {
                var vals = Enum.GetValues(typeof(GuildFeature)).Cast<GuildFeature>();

                var missingValues = vals.Where(x => feature.HasFlag(x) && !Value.HasFlag(x));

                throw new InvalidOperationException($"Missing required guild feature{(missingValues.Count() > 1 ? "s" : "")} {string.Join(", ", missingValues.Select(x => x.ToString()))} in order to execute this operation.");
            }
        }
    }
}
