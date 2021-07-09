using Discord.API;
using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class SlashCommandHelper
    {
        #region Commands
        public static async Task<IEnumerable<RestApplicationCommand>> GetApplicationCommands (BaseDiscordClient discord, IGuild guild,
            RequestOptions options)
        {
            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);

            IEnumerable<API.ApplicationCommand> commands;

            if (guild != null)
                commands = await discord.ApiClient.GetGuildApplicationCommands(appInfo.Id, guild.Id, options).ConfigureAwait(false);
            else
                commands = await discord.ApiClient.GetGlobalApplicationCommands(appInfo.Id, options).ConfigureAwait(false);

            return commands.Select(x =>
            {
                var restGuild = x.GuildId.IsSpecified ? new RestGuild(discord, x.GuildId.Value) : null;
                return new RestApplicationCommand(discord, x.Id, restGuild, x);
            });
        }

        public static async Task<RestApplicationCommand> GetApplicationCommand (BaseDiscordClient discord, IGuild guild,
            ulong commandId, RequestOptions options)
        {
            ApplicationCommand command;

            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);

            if (guild != null)
                command = await discord.ApiClient.GetGuildApplicationCommand(appInfo.Id, commandId, commandId, options).ConfigureAwait(false);
            else
                command = await discord.ApiClient.GetGlobalApplicationCommand(appInfo.Id, commandId, options).ConfigureAwait(false);

            var restGuild = command.GuildId.IsSpecified ? new RestGuild(discord, command.GuildId.Value) : null;
            return new RestApplicationCommand(discord, command.Id, restGuild, command);
        }

        public static async Task DeleteApplicationCommand (BaseDiscordClient discord, ulong commandId, IGuild guild,
            RequestOptions options)
        {
            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);

            if (guild != null)
                await discord.ApiClient.DeleteGuildApplicationCommand(appInfo.Id, guild.Id, commandId, options).ConfigureAwait(false);
            else
                await discord.ApiClient.DeleteGlobalApplicationCommand(appInfo.Id, commandId, options).ConfigureAwait(false);
        }

        public static async Task<RestApplicationCommand> ModifyApplicationCommand (BaseDiscordClient discord, ulong commandId,
            IGuild guild, string name, string description, bool defaultPermission, IEnumerable<IApplicationCommandOption> commandOptions, RequestOptions options)
        {
            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);

            var args = new ModifyApplicationCommandParams(name, description)
            {
                DefaultPermission = defaultPermission,
                Options = commandOptions.Select(x => x.ToModel()).ToArray()
            };

            API.ApplicationCommand command;

            if (guild != null)
                command = await discord.ApiClient.ModifyGuildApplicationCommand(appInfo.Id, guild.Id, commandId, args, options).ConfigureAwait(false);
            else
                command = await discord.ApiClient.ModifyGlobalApplicationCommand(appInfo.Id, commandId, args, options).ConfigureAwait(false);

            var restGuild = command.GuildId.IsSpecified ? new RestGuild(discord, command.GuildId.Value) : null;
            return new RestApplicationCommand(discord, command.Id, restGuild, command);
        }

        public static async Task<ulong> CreateApplicationCommand (BaseDiscordClient discord, string name,
            string description, bool defaultPermission, IEnumerable<ApplicationCommandOption> commandOptions, IGuild guild, RequestOptions options)
        {
            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);

            var args = new CreateApplicationCommandParams(name, description)
            {
                DefaultPermission = defaultPermission,
                Options = commandOptions.Select(x => x.ToModel()).ToArray()
            };

            ApplicationCommand command;
            if (guild != null)
                command = await discord.ApiClient.CreateGuildApplicationCommand(appInfo.Id, guild.Id, args, options).ConfigureAwait(false);
            else
                command = await discord.ApiClient.CreateGlobalApplicationCommand(appInfo.Id, args, options).ConfigureAwait(false);

            return command.Id;
        }

        public static async Task<IEnumerable<ulong>> BulkOverwriteApplicationCommands (BaseDiscordClient discord, IEnumerable<IApplicationCommand> commands,
            IGuild guild, RequestOptions options)
        {
            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);

            var models = commands.Select(x => x.ToModel());

            IEnumerable<ApplicationCommand> echoCommands;

            if (guild != null)
                echoCommands = await discord.ApiClient.BulkOverwriteGuildApplicationCommands(appInfo.Id, guild.Id, models, options).ConfigureAwait(false);
            else
                echoCommands = await discord.ApiClient.BulkOverwriteGlobalApplicationCommands(appInfo.Id, models, options).ConfigureAwait(false);

            return echoCommands.Select(x => x.Id);
        }
        #endregion

        #region InteractionResponse
        public static async Task SendInteractionResponse (BaseDiscordClient discord, IDiscordInteraction interaction,
            string text, bool isTTS, IEnumerable<Embed> embeds, AllowedMentions allowedMentions, IEnumerable<MessageComponent> messageComponents,
            InteractionApplicationCommandCallbackFlags flags,
            RequestOptions options)
        {
            Preconditions.AtMost(embeds?.Count() ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            CheckAllowedMentions(allowedMentions);

            if (!interaction.Token.IsValid)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token.Token;

            var data = new InteractionApplicationCommandCallbackData
            {
                TTS = isTTS,
                Content = text,
                Embeds = embeds?.Select(x => x?.ToModel()).ToArray(),
                AllowedMentions = allowedMentions?.ToModel(),
                Flags = flags,
                Components = messageComponents?.Select(x => x?.ToModel()).ToArray()
            };

            InteractionCallbackType callbackType;
            switch (interaction.InteractionType)
            {
                case InteractionType.Ping:
                    throw new InvalidOperationException($"Interaction type {nameof(InteractionType.Ping)} only supports acknowledgement as a response.");
                case InteractionType.ApplicationCommand:
                    callbackType = InteractionCallbackType.ChannelMessageWithSource;
                    break;
                case InteractionType.MessageComponent:
                    callbackType = InteractionCallbackType.UpdateMessage;
                    break;
                default:
                    throw new ArgumentException($"Interaction type {nameof(interaction.InteractionType)} is not supported for sending a response");
            }

            var args = new CreateInteractionResponseParams(callbackType)
            {
                Data = data
            };
            await discord.ApiClient.CreateInteractionResponse(interaction.Id, token, args, options).ConfigureAwait(false);
        }

        public static async Task SendAcknowledgement(BaseDiscordClient discord, IDiscordInteraction interaction, RequestOptions options)
        {
            if (!interaction.Token.IsValid)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            CreateInteractionResponseParams args;
            switch (interaction.InteractionType)
            {
                case InteractionType.Ping:
                    args = new CreateInteractionResponseParams(InteractionCallbackType.Pong);
                    break;
                case InteractionType.ApplicationCommand:
                    args = new CreateInteractionResponseParams(InteractionCallbackType.DeferredChannelMessageWithSource);
                    break;
                case InteractionType.MessageComponent:
                    args = new CreateInteractionResponseParams(InteractionCallbackType.DeferredUpdateMessage);
                    break;
                default:
                    throw new InvalidOperationException("This interaction type is not supported for sending an acknowledgement.");
            }

            await discord.ApiClient.CreateInteractionResponse(interaction.Id, interaction.Token.Token, args, options).ConfigureAwait(false);
        }

        public static async Task<InteractionResponse> GetInteractionResponse (BaseDiscordClient discord, IDiscordInteraction interaction, RequestOptions options)
        {
            if (!interaction.Token.IsValid)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token.Token;
            ulong appId = interaction.ApplicationId;

            var response = await discord.ApiClient.GetOriginalInteractionResponse(appId, token, options);

            if (!response.Data.IsSpecified)
                return new InteractionResponse(response.Type);
            else
                return response.ToEntity();
        }

        public static async Task ModifyInteractionResponse (BaseDiscordClient discord, IDiscordInteraction interaction, string text,
            AllowedMentions allowedMentions, IEnumerable<Embed> embeds, IEnumerable<MessageComponent> messageComponents, RequestOptions options)
        {
            Preconditions.AtMost(embeds?.Count() ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            CheckAllowedMentions(allowedMentions);

            if (!interaction.Token.IsValid)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token.Token;
            ulong appId = interaction.ApplicationId;

            var args = new ModifyInteractionResponseParams()
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel(),
                Embeds = embeds?.Select(x => x?.ToModel()).ToArray(),
                Components = messageComponents?.Select(x => x?.ToModel()).ToArray()
            };

            if (args.Components.GetValueOrDefault(null) != null && args.Content.GetValueOrDefault(null) == null)
                throw new InvalidOperationException("Message components must be provided alongside a string content.");

            await discord.ApiClient.ModifyOriginalInteraction(appId, token, args, options).ConfigureAwait(false);
        }

        public static async Task DeleteInteractionResponse (BaseDiscordClient discord, IDiscordInteraction interaction, RequestOptions options)
        {
            if (!interaction.Token.IsValid)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token.Token;
            ulong appId = interaction.ApplicationId;

            await discord.ApiClient.DeleteOriginalInteractionResponse(appId, token, options).ConfigureAwait(false);
        }
        #endregion

        #region Followup
        public static async Task<RestMessage> SendInteractionFollowup (BaseDiscordClient discord, IDiscordInteraction interaction, string text,
            bool isTTS, IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, RequestOptions options)
        {
            Preconditions.AtMost(embeds?.Count() ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            CheckAllowedMentions(allowedMentions);

            if (!interaction.Token.IsValid)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token.Token;
            ulong appId = interaction.ApplicationId;

            var args = new CreateWebhookMessageParams(text)
            {
                IsTTS = isTTS,
                Embeds = embeds?.Select(x => x?.ToModel()).ToArray(),
                Username = username,
                AvatarUrl = avatarUrl,
                AllowedMentions = allowedMentions?.ToModel()
            };

            var message = await discord.ApiClient.CreateFollowupMessage(appId, token, args, options).ConfigureAwait(false);
            var user = RestUser.Create(discord, message.Author.GetValueOrDefault(null));
            return RestMessage.Create(discord, interaction.Channel, user, message);
        }

        public static async Task ModifyFollowupMessage (BaseDiscordClient discord, IDiscordInteraction interaction, ulong messageId,
            string text, AllowedMentions allowedMentions, IEnumerable<Embed> embeds, RequestOptions options)
        {
            Preconditions.AtMost(embeds?.Count() ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            CheckAllowedMentions(allowedMentions);

            if (!interaction.Token.IsValid)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token.Token;
            ulong appId = interaction.ApplicationId;

            var args = new ModifyWebhookMessageParams()
            {
                Content = text,
                Embeds = embeds?.Select(x => x?.ToModel()).ToArray(),
                AllowedMentions = allowedMentions?.ToModel()
            };
            await discord.ApiClient.ModifyFollowupMessage(appId, token, messageId, args, options).ConfigureAwait(false);
        }

        public static async Task DeleteFollowupMessage (BaseDiscordClient discord, IDiscordInteraction interaction, ulong messageId,
            RequestOptions options)
        {
            if (!interaction.Token.IsValid)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token.Token;
            ulong appId = interaction.ApplicationId;

            await discord.ApiClient.DeleteFollowupMessage(appId, token, messageId, options).ConfigureAwait(false);
        }
        #endregion

        #region Permissions

        public static async Task<IEnumerable<ApplicationCommandPermissions>> GetGuildCommandPermissions (BaseDiscordClient discord, IGuild guild,
            RequestOptions options = null)
        {
            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);
            var commands = await discord.ApiClient.GetGuildApplicationCommandPermissions(appInfo.Id, guild.Id, options).ConfigureAwait(false);

            var result = new List<ApplicationCommandPermissions>();

            foreach (var command in commands)
            {
                var restCommand = await GetApplicationCommand(discord, guild, command.CommandId, options).ConfigureAwait(false);
                var users = new Dictionary<IUser, bool>();
                var roles = new Dictionary<IRole, bool>();

                var restGuild = await ClientHelper.GetGuildAsync(discord, guild.Id, false, options).ConfigureAwait(false);

                foreach (var permission in command.Permissions)
                {
                    if (permission.Type == ApplicationCommandPermissionType.Role)
                        roles.Add(restGuild.GetRole(permission.Id), permission.Allow);
                    else if (permission.Type == ApplicationCommandPermissionType.User)
                        users.Add(await restGuild.GetUserAsync(permission.Id, options), permission.Allow);
                }

                result.Add(new ApplicationCommandPermissions(restCommand, users, roles));
            }
            return result;
        }

        public static async Task<ApplicationCommandPermissions> GetCommandPermission(BaseDiscordClient discord, IApplicationCommand command,
            RequestOptions options = null )
        {
            var commandPermission = await discord.ApiClient.GetApplicationCommandPermissions(command.ApplicationId, command.Guild.Id, command.Id, options)
                .ConfigureAwait(false);

            var users = new Dictionary<IUser, bool>();
            var roles = new Dictionary<IRole, bool>();

            var restGuild = await ClientHelper.GetGuildAsync(discord, command.Guild.Id, false, options).ConfigureAwait(false);

            foreach(var permission in commandPermission.Permissions)
            {
                if (permission.Type == ApplicationCommandPermissionType.Role)
                    roles.Add(restGuild.GetRole(permission.Id), permission.Allow);
                else if (permission.Type == ApplicationCommandPermissionType.User)
                    users.Add(await restGuild.GetUserAsync(permission.Id, options), permission.Allow);
            }

            return new ApplicationCommandPermissions(command, users, roles); 
        }

        public static async Task<ApplicationCommandPermissions> ModifyCommandPermissions (BaseDiscordClient discord, IApplicationCommand command,
            IDictionary<IUser, bool> userPerms, IDictionary<IRole, bool> rolesPerms, RequestOptions options = null)
        {
            if (rolesPerms.Count + userPerms.Count > 10)
                throw new InvalidOperationException("You cannot edit more than 10 permissions at the same time");

            var args = new List<ApplicationCommandPermission>();

            foreach (var userPerm in userPerms)
                args.Add(new ApplicationCommandPermission
                {
                    Id = userPerm.Key.Id,
                    Type = ApplicationCommandPermissionType.User,
                    Allow = userPerm.Value
                });

            foreach (var rolePerm in rolesPerms)
                args.Add(new ApplicationCommandPermission
                {
                    Id = rolePerm.Key.Id,
                    Type = ApplicationCommandPermissionType.Role,
                    Allow = rolePerm.Value
                });

            var modified = await discord.ApiClient.ModifyApplicationCommandPermissions(command.ApplicationId, command.Guild.Id, command.Id, args, options)
                .ConfigureAwait(false);

            var guild = await ClientHelper.GetGuildAsync(discord, modified.GuildId, false, options).ConfigureAwait(false);

            var users = modified.Permissions.Where(x => x.Type == ApplicationCommandPermissionType.User)
                .ToDictionary( x => guild.GetUserAsync(x.Id).GetAwaiter().GetResult() as IUser, x => x.Allow);

            var roles = modified.Permissions.Where(x => x.Type == ApplicationCommandPermissionType.Role)
                .ToDictionary(x => guild.GetRole(x.Id) as IRole, x => x.Allow);

            return new ApplicationCommandPermissions(command, users, roles);
        }
        #endregion

        #region Utils
        private static void CheckAllowedMentions (AllowedMentions allowedMentions)
        {
            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");

            if (allowedMentions != null && allowedMentions.AllowedTypes.HasValue)
            {
                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Users) &&
                    allowedMentions.UserIds != null && allowedMentions.UserIds.Count > 0)
                {
                    throw new ArgumentException("The Users flag is mutually exclusive with the list of User Ids.", nameof(allowedMentions));
                }

                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Roles) &&
                    allowedMentions.RoleIds != null && allowedMentions.RoleIds.Count > 0)
                {
                    throw new ArgumentException("The Roles flag is mutually exclusive with the list of Role Ids.", nameof(allowedMentions));
                }
            }
        }
        #endregion
    }
}
