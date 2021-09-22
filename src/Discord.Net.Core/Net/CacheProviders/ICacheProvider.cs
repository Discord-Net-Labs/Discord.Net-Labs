using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic cache provider.
    /// </summary>
    public interface ICacheProvider
    {
        #region Messages
        ValueTask<IEnumerable<byte>> GetMessageAsync(ulong messageId, ulong channelId);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetMessagesAsync(ulong from, ulong to, ulong channelId);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetMessagesAsync(ulong from, Direction dir, ulong channelId, int limit = DiscordConfig.MaxMessagesPerBatch);
        ValueTask CreateMessageAsync(ulong messageId, ulong channelId, IEnumerable<byte> entity);
        ValueTask UpdateMessageAsync(ulong messageId, ulong channelId, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteMessageAsync(ulong messageId, ulong channelId);
        #endregion
        #region Channels
        ValueTask<IEnumerable<byte>> GetChannelAsync(ulong id);
        ValueTask CreateChannelAsync(ulong id, IEnumerable<byte> entity);
        ValueTask UpdateChannelAsync(ulong id, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteChannelAsync(ulong id);
        #endregion
        #region Users
        ValueTask<IEnumerable<byte>> GetUserAsync(ulong id);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetUsersAsync();
        ValueTask CreateUserAsync(ulong id, IEnumerable<byte> entity);
        ValueTask UpdateUserAsync(ulong id, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteUserAsync(ulong id);
        #endregion
        #region Guild Channels
        ValueTask<IEnumerable<byte>> GetGuildChannelAsync(ulong id, ulong guildId);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetGuildChannelsAsync(ulong guildId);
        ValueTask CreateGuildChannelAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask UpdateGuildChannelAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteGuildChannelAsync(ulong id, ulong guildId);
        #endregion
        #region Guild Users
        ValueTask<IEnumerable<byte>> GetGuildUserAsync(ulong id, ulong guildId);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetGuildUsersAsync(ulong guildId);
        ValueTask CreateGuildUserAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask UpdateGuildUserAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteGuildUserAsync(ulong id, ulong guildId);
        #endregion
        #region Thread Members
        ValueTask<IEnumerable<byte>> GetThreadMemberAsync(ulong id, ulong guildId, ulong threadId);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetThreadMemberAsync(ulong guildId, ulong threadId);
        ValueTask CreateThreadMemberAsync(ulong id, ulong guildId, ulong threadId, IEnumerable<byte> entity);
        ValueTask UpdateThreadMemberAsync(ulong id, ulong guildId, ulong threadId, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteThreadMemberAsync(ulong id, ulong guildId, ulong threadId);
        #endregion
        #region Guilds
        ValueTask<IEnumerable<byte>> GetGuildAsync(ulong id);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetGuildsAsync();
        ValueTask CreateGuildAsync(ulong id, IEnumerable<byte> entity);
        ValueTask UpdateGuildAsync(ulong id, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteGuildAsync(ulong id);
        #endregion
        #region Roles
        ValueTask<IEnumerable<byte>> GetRoleAsync(ulong id, ulong guildId);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetRolesAsync(ulong guildId);
        ValueTask CreateRoleAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask UpdateRoleAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteRoleAsync(ulong id, ulong guildId);
        #endregion
        #region Guild Emotes
        ValueTask<IEnumerable<byte>> GetEmoteAsync(ulong id, ulong guildId);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetEmotesAsync(ulong guildId);
        ValueTask CreateEmoteAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask UpdateEmoteAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteEmoteAsync(ulong id, ulong guildId);
        #endregion
        #region Guild Stickers
        ValueTask<IEnumerable<byte>> GetStickerAsync(ulong id, ulong guildId);
        ValueTask<IEnumerable<IEnumerable<byte>>> GetStickersAsync(ulong guildId);
        ValueTask CreateStickerAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask UpdateStickerAsync(ulong id, ulong guildId, IEnumerable<byte> entity);
        ValueTask<IEnumerable<byte>> DeleteStickerAsync(ulong id, ulong guildId);
        #endregion
    }
}
