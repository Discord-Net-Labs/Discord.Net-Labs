using System;
using System.Diagnostics;
using Model = Discord.API.VoiceState;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket user's voice connection status.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketVoiceState : SocketCacheableEntity<Cache.VoiceState, string>, IVoiceState
    {
        private ulong _channelId { get; set; }

        [Flags]
        private enum Flags : byte
        {
            Normal = 0x00,
            Suppressed = 0x01,
            Muted = 0x02,
            Deafened = 0x04,
            SelfMuted = 0x08,
            SelfDeafened = 0x10,
            SelfStream = 0x20,
            SelfVideo = 0x40,
        }

        private Flags _voiceStates;

        /// <summary>
        ///     Gets the voice channel that the user is currently in; or <c>null</c> if none.
        /// </summary>
        public SocketVoiceChannel VoiceChannel => (SocketVoiceChannel)Discord.State.GetChannel(_channelId);
        /// <inheritdoc />
        public string VoiceSessionId { get; }
        /// <inheritdoc/>
        public DateTimeOffset? RequestToSpeakTimestamp { get; private set; }

        /// <inheritdoc />
        public bool IsMuted => (_voiceStates & Flags.Muted) != 0;
        /// <inheritdoc />
        public bool IsDeafened => (_voiceStates & Flags.Deafened) != 0;
        /// <inheritdoc />
        public bool IsSuppressed => (_voiceStates & Flags.Suppressed) != 0;
        /// <inheritdoc />
        public bool IsSelfMuted => (_voiceStates & Flags.SelfMuted) != 0;
        /// <inheritdoc />
        public bool IsSelfDeafened => (_voiceStates & Flags.SelfDeafened) != 0;
        /// <inheritdoc />
        public bool IsStreaming => (_voiceStates & Flags.SelfStream) != 0;
        /// <inheritdoc />
        public bool IsVideo => (_voiceStates & Flags.SelfVideo) != 0;

        internal SocketVoiceState(DiscordSocketClient client, string session)
            : base(client, session)
        {

        }

        internal SocketVoiceState(DiscordSocketClient client, DateTimeOffset? requestToSpeak, string sessionId, bool isSelfMuted, bool isSelfDeafened, bool isMuted, bool isDeafened, bool isSuppressed, bool isStream, bool isVideo)
            : base(client, sessionId)
        {
            VoiceSessionId = sessionId;
            RequestToSpeakTimestamp = requestToSpeak;

            Flags voiceStates = Flags.Normal;
            if (isSelfMuted)
                voiceStates |= Flags.SelfMuted;
            if (isSelfDeafened)
                voiceStates |= Flags.SelfDeafened;
            if (isMuted)
                voiceStates |= Flags.Muted;
            if (isDeafened)
                voiceStates |= Flags.Deafened;
            if (isSuppressed)
                voiceStates |= Flags.Suppressed;
            if (isStream)
                voiceStates |= Flags.SelfStream;
            if (isVideo)
                voiceStates |= Flags.SelfVideo;
            _voiceStates = voiceStates;
        }

        internal static SocketVoiceState Create(DiscordSocketClient client, Model model)
        {
            return new SocketVoiceState(client, model.RequestToSpeakTimestamp.IsSpecified ? model.RequestToSpeakTimestamp.Value : null, model.SessionId, model.SelfMute, model.SelfDeaf, model.Mute, model.Deaf, model.Suppress, model.SelfStream, model.SelfVideo);
        }

        internal override void Update(DiscordSocketClient discord, Cache.VoiceState model)
        {
            _channelId = model.ChannelId;

            Flags voiceStates = Flags.Normal;
            if (model.SelfMute)
                voiceStates |= Flags.SelfMuted;
            if (model.SelfDeaf)
                voiceStates |= Flags.SelfDeafened;
            if (model.Mute)
                voiceStates |= Flags.Muted;
            if (model.Deaf)
                voiceStates |= Flags.Deafened;
            if (model.Suppress)
                voiceStates |= Flags.Suppressed;
            if (model.SelfStream ?? false)
                voiceStates |= Flags.SelfStream;
            if (model.SelfVideo)
                voiceStates |= Flags.SelfVideo;
            _voiceStates = voiceStates;

            RequestToSpeakTimestamp = DateTimeUtils.FromTicks(model.RequestToSpeak);
        }

        internal override Cache.VoiceState ToCacheModel()
        {
            return new Cache.VoiceState()
            {
                ChannelId = _channelId,
                Deaf = IsDeafened,
                Mute = IsMuted,
                RequestToSpeak = RequestToSpeakTimestamp.HasValue ? RequestToSpeakTimestamp.Value.UtcTicks : null,
                SelfDeaf = IsSelfDeafened,
                SelfMute = IsSelfMuted,
                SelfStream = IsStreaming,
                SelfVideo = IsVideo,
                SessionId = Id,
                Suppress = IsSuppressed,
            };
        }

        /// <summary>
        ///     Gets the name of this voice channel.
        /// </summary>
        /// <returns>
        ///     A string that resolves to name of this voice channel; otherwise "Unknown".
        /// </returns>
        public override string ToString() => VoiceChannel?.Name ?? "Unknown";
        private string DebuggerDisplay => $"{VoiceChannel?.Name ?? "Unknown"} ({_voiceStates})";
        internal SocketVoiceState Clone() => this;

        /// <inheritdoc />
        IVoiceChannel IVoiceState.VoiceChannel => VoiceChannel;
    }
}
