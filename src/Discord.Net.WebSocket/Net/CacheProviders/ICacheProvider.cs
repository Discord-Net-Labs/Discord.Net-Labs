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
        IEnumerable<byte> GetMessage(ulong messageId, ulong channelId);
        IEnumerable<IEnumerable<byte>> GetMessages(ulong from, ulong to, ulong channelId);
        IEnumerable<IEnumerable<byte>> GetMessages(ulong from, Direction dir, ulong channelId, int limit = DiscordConfig.MaxMessagesPerBatch);
        void CreateMessage(ulong messageId, ulong channelId, IEnumerable<byte> entity);
        void UpdateMessage(ulong messageId, ulong channelId, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteMessage(ulong messageId, ulong channelId);
        #endregion
        #region Channels
        IEnumerable<byte> GetChannel(ulong id);
        void CreateChannel(ulong id, IEnumerable<byte> entity);
        void UpdateChannel(ulong id, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteChannel(ulong id);
        #endregion
        #region Users
        IEnumerable<byte> GetUser(ulong id);
        IEnumerable<IEnumerable<byte>> GetUsers();
        void CreateUser(ulong id, IEnumerable<byte> entity);
        void UpdateUser(ulong id, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteUser(ulong id);
        #endregion
        #region Guild Channels
        IEnumerable<byte> GetGuildChannel(ulong id, ulong guildId);
        IEnumerable<IEnumerable<byte>> GetGuildChannels(ulong guildId);
        void CreateGuildChannel(ulong id, ulong guildId, IEnumerable<byte> entity);
        void UpdateGuildChannel(ulong id, ulong guildId, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteGuildChannel(ulong id, ulong guildId);
        #endregion
        #region Guild Users
        IEnumerable<byte> GetGuildUser(ulong id, ulong guildId);
        IEnumerable<IEnumerable<byte>> GetGuildUsers(ulong guildId);
        void CreateGuildUser(ulong id, ulong guildId, IEnumerable<byte> entity);
        void UpdateGuildUser(ulong id, ulong guildId, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteGuildUser(ulong id, ulong guildId);
        #endregion
        #region Thread Members
        IEnumerable<byte> GetThreadMember(ulong id, ulong guildId, ulong threadId);
        IEnumerable<IEnumerable<byte>> GetThreadMember(ulong guildId, ulong threadId);
        void CreateThreadMember(ulong id, ulong guildId, ulong threadId, IEnumerable<byte> entity);
        void UpdateThreadMember(ulong id, ulong guildId, ulong threadId, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteThreadMember(ulong id, ulong guildId, ulong threadId);
        #endregion
        #region Guilds
        IEnumerable<byte> GetGuild(ulong id);
        IEnumerable<IEnumerable<byte>> GetGuilds();
        void CreateGuild(ulong id, IEnumerable<byte> entity);
        void UpdateGuild(ulong id, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteGuild(ulong id);
        #endregion
        #region Roles
        IEnumerable<byte> GetRole(ulong id, ulong guildId);
        IEnumerable<IEnumerable<byte>> GetRoles(ulong guildId);
        void CreateRole(ulong id, ulong guildId, IEnumerable<byte> entity);
        void UpdateRole(ulong id, ulong guildId, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteRole(ulong id, ulong guildId);
        #endregion
        #region Guild Emotes
        IEnumerable<byte> GetEmote(ulong id, ulong guildId);
        IEnumerable<IEnumerable<byte>> GetEmotes(ulong guildId);
        void CreateEmote(ulong id, ulong guildId, IEnumerable<byte> entity);
        void UpdateEmote(ulong id, ulong guildId, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteEmote(ulong id, ulong guildId);
        #endregion
        #region Guild Stickers
        IEnumerable<byte> GetSticker(ulong id, ulong guildId);
        IEnumerable<IEnumerable<byte>> GetStickers(ulong guildId);
        void CreateSticker(ulong id, ulong guildId, IEnumerable<byte> entity);
        void UpdateSticker(ulong id, ulong guildId, IEnumerable<byte> entity);
        IEnumerable<byte> DeleteSticker(ulong id, ulong guildId);
        #endregion
    }
}
