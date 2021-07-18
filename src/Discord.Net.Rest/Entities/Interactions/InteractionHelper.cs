using Discord.API;
using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class InteractionHelper
    {
        #region Commands
        public static async Task<IReadOnlyCollection<RestApplicationCommand>> GetApplicationCommands (BaseDiscordClient discord, IGuild guild,
            RequestOptions options)
        {
            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);

            IEnumerable<API.ApplicationCommand> commands;

            if (guild != null)
                commands = await discord.ApiClient.GetGuildApplicationCommands( guild.Id, options).ConfigureAwait(false);
            else
                commands = await discord.ApiClient.GetGlobalApplicationCommands( options).ConfigureAwait(false);

            return commands.Select(x =>
            {
                var restGuild = x.GuildId.IsSpecified ? new RestGuild(discord, x.GuildId.Value) : null;
                return new RestApplicationCommand(discord, x.Id, restGuild, x);
            }).ToImmutableArray();
        }
        
        public static async Task<RestApplicationCommand> GetApplicationCommand (BaseDiscordClient discord, ulong commandId,
            IGuild guild = null, RequestOptions options = null)
        {
            ApplicationCommand command;

            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);

            if (guild != null)
                command = await discord.ApiClient.GetGuildApplicationCommand(commandId, commandId, options).ConfigureAwait(false);
            else
                command = await discord.ApiClient.GetGlobalApplicationCommand(commandId, options).ConfigureAwait(false);

            var restGuild = command.GuildId.IsSpecified ? new RestGuild(discord, command.GuildId.Value) : null;
            return new RestApplicationCommand(discord, command.Id, restGuild, command);
        }

        public static async Task DeleteApplicationCommand (BaseDiscordClient discord, ulong commandId, IGuild guild,
            RequestOptions options)
        {
            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);

            if (guild != null)
                await discord.ApiClient.DeleteGuildApplicationCommand( guild.Id, commandId, options).ConfigureAwait(false);
            else
                await discord.ApiClient.DeleteGlobalApplicationCommand( commandId, options).ConfigureAwait(false);
        }

        public static async Task<RestApplicationCommand> ModifyApplicationCommand (BaseDiscordClient discord, ulong commandId,
            IGuild guild, string name, string description, bool defaultPermission, IEnumerable<IApplicationCommandOption> commandOptions, RequestOptions options)
        {
            var args = new ModifyApplicationCommandParams(name, description)
            {
                DefaultPermission = defaultPermission,
                Options = commandOptions.Select(x => x.ToModel()).ToArray()
            };

            API.ApplicationCommand command;

            if (guild != null)
                command = await discord.ApiClient.ModifyGuildApplicationCommand( guild.Id, commandId, args, options).ConfigureAwait(false);
            else
                command = await discord.ApiClient.ModifyGlobalApplicationCommand( commandId, args, options).ConfigureAwait(false);

            var restGuild = command.GuildId.IsSpecified ? new RestGuild(discord, command.GuildId.Value) : null;
            return new RestApplicationCommand(discord, command.Id, restGuild, command);
        }

        public static async Task<ulong> CreateApplicationCommand (BaseDiscordClient discord, string name,
            string description, bool defaultPermission, IEnumerable<ApplicationCommandOption> commandOptions, IGuild guild, RequestOptions options)
        {
            var args = new CreateApplicationCommandParams(name, description)
            {
                DefaultPermission = defaultPermission,
                Options = commandOptions.Select(x => x.ToModel()).ToArray()
            };

            ApplicationCommand command;
            if (guild != null)
                command = await discord.ApiClient.CreateGuildApplicationCommand( guild.Id, args, options).ConfigureAwait(false);
            else
                command = await discord.ApiClient.CreateGlobalApplicationCommand( args, options).ConfigureAwait(false);

            return command.Id;
        }

        public static async Task<IEnumerable<ulong>> BulkOverwriteApplicationCommands (BaseDiscordClient discord, IEnumerable<IApplicationCommand> commands,
            IGuild guild, RequestOptions options)
        {
            var models = commands.Select(x => x.ToModel());

            IEnumerable<ApplicationCommand> echoCommands;

            if (guild != null)
                echoCommands = await discord.ApiClient.BulkOverwriteGuildApplicationCommands( guild.Id, models, options).ConfigureAwait(false);
            else
                echoCommands = await discord.ApiClient.BulkOverwriteGlobalApplicationCommands( models, options).ConfigureAwait(false);

            return echoCommands.Select(x => x.Id);
        }

        public static Task DeleteAllGuildCommandsAsync (BaseDiscordClient client, ulong guildId, RequestOptions options = null)
        {
            return client.ApiClient.BulkOverwriteGuildApplicationCommands(guildId, new CreateApplicationCommandParams[0], options);
        }

        public static Task DeleteAllGlobalCommandsAsync (BaseDiscordClient client, RequestOptions options = null)
        {
            return client.ApiClient.BulkOverwriteGlobalApplicationCommands(new CreateApplicationCommandParams[0], options);
        }

        #endregion

        #region SlashCommandProps
        public static async Task<RestApplicationCommand> CreateGlobalCommand (BaseDiscordClient client,
            Action<SlashCommandCreationProperties> func, RequestOptions options = null)
        {
            var args = new SlashCommandCreationProperties();
            func(args);
            return await CreateApplicationCommand(client, args, options: options).ConfigureAwait(false);
        }
        public static async Task<RestApplicationCommand> CreateGuildCommand(BaseDiscordClient client, IGuild guild,
            Action<SlashCommandCreationProperties> func, RequestOptions options = null)
        {
            var args = new SlashCommandCreationProperties();
            func(args);
            return await CreateApplicationCommand(client, args, guild, options).ConfigureAwait(false);
        }
        public static async Task<RestApplicationCommand> CreateApplicationCommand (BaseDiscordClient client,
            SlashCommandCreationProperties arg, IGuild guild = null, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));
            Preconditions.NotNullOrEmpty(arg.Description, nameof(arg.Description));

            if (arg.Options.IsSpecified)
                Preconditions.AtMost(arg.Options.Value.Count, 25, nameof(arg.Options));

            var model = new CreateApplicationCommandParams(arg.Name, arg.Description)
            {
                Options = arg.Options.IsSpecified
                    ? arg.Options.Value.Select(x => x.ToModel()).ToArray()
                    : Optional<API.ApplicationCommandOption[]>.Unspecified,
                DefaultPermission = arg.DefaultPermission.IsSpecified
                    ? arg.DefaultPermission.Value
                    : Optional<bool>.Unspecified
            };

            ApplicationCommand cmd;

            if (guild == null)
                cmd = await client.ApiClient.CreateGlobalApplicationCommandAsync(model, options).ConfigureAwait(false);
            else
                cmd = await client.ApiClient.CreateGuildApplicationCommand(guild.Id, model, options).ConfigureAwait(false);

            return new RestApplicationCommand(client, cmd.Id, guild, cmd);
        }
        public static async Task<IReadOnlyCollection<RestApplicationCommand>> BulkOverwriteApplicationCommands (BaseDiscordClient client,
            SlashCommandCreationProperties[] args, IGuild guild = null, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));

            var models = new List<CreateApplicationCommandParams>();

            foreach (var arg in args)
            {
                Preconditions.NotNullOrEmpty(arg.Name, nameof(arg.Name));
                Preconditions.NotNullOrEmpty(arg.Description, nameof(arg.Description));

                if (arg.Options.IsSpecified)
                    Preconditions.AtMost(arg.Options.Value.Count, 25, nameof(arg.Options));

                var model = new CreateApplicationCommandParams(arg.Name, arg.Description)
                {
                    Options = arg.Options.IsSpecified
                    ? arg.Options.Value.Select(x => x.ToModel()).ToArray()
                    : Optional<API.ApplicationCommandOption[]>.Unspecified,
                    DefaultPermission = arg.DefaultPermission.IsSpecified
                    ? arg.DefaultPermission.Value
                    : Optional<bool>.Unspecified
                };

                models.Add(model);
            }

            API.ApplicationCommand[] apiModels;

            if (guild != null)
                apiModels = await client.ApiClient.BulkOverwriteGuildApplicationCommands(guild.Id, models.ToArray(), options).ConfigureAwait(false);
            else
                apiModels = await client.ApiClient.BulkOverwriteGlobalApplicationCommands(models.ToArray(), options).ConfigureAwait(false);

            return apiModels.Select(x => new RestApplicationCommand(client, x.Id, guild, x)).ToArray();
        }

        #endregion

        #region InteractionResponse
        public static async Task SendInteractionResponse (BaseDiscordClient discord, IDiscordInteraction interaction,
            string text, bool isTTS, IEnumerable<Embed> embeds, AllowedMentions allowedMentions, MessageComponent messageComponent,
            InteractionApplicationCommandCallbackFlags flags,
            RequestOptions options)
        {
            Preconditions.AtMost(embeds?.Count() ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            CheckAllowedMentions(allowedMentions);

            if (!interaction.IsValidToken)
                throw new InvalidOperationException("Callback token for this interaction has exprired.");

            string token = interaction.Token;
            var test = new MessageProperties();
            var data = new InteractionApplicationCommandCallbackData
            {
                TTS = isTTS,
                Content = text,
                Embeds = embeds?.Select(x => x?.ToModel()).ToArray(),
                AllowedMentions = allowedMentions?.ToModel(),
                Flags = flags,
                Components = messageComponent?.Components.Select(x => x.ToModel()).ToArray()
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
            if (!interaction.IsValidToken)
                throw new InvalidOperationException("Callback token for this interaction has exprired.");

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

            await discord.ApiClient.CreateInteractionResponse(interaction.Id, interaction.Token, args, options).ConfigureAwait(false);
        }

        public static async Task<RestInteractionMessage> GetInteractionResponse (BaseDiscordClient discord, IDiscordInteraction interaction, RequestOptions options)
        {
            var response = await discord.ApiClient.GetOriginalInteractionResponse( interaction.Token, options);
            var channel = await ClientHelper.GetChannelAsync(discord, response.ChannelId, options).ConfigureAwait(false) as IMessageChannel;

            return RestInteractionMessage.Create(discord, response, interaction, channel);
        }

        public static async Task<RestInteractionMessage> ModifyInteractionResponse (BaseDiscordClient discord, IDiscordInteraction interaction, string text,
            AllowedMentions allowedMentions, IEnumerable<Embed> embeds, MessageComponent messageComponent, RequestOptions options)
        {
            Preconditions.AtMost(embeds?.Count() ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            CheckAllowedMentions(allowedMentions);

            if (!interaction.IsValidToken)
                throw new InvalidOperationException("Callback token for this interaction has exprired.");

            string token = interaction.Token;

            var args = new ModifyInteractionResponseParams()
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel(),
                Embeds = embeds?.Select(x => x?.ToModel()).ToArray(),
                Components = messageComponent?.Components.Select(x => x.ToModel()).ToArray()
            };

            if (args.Components.GetValueOrDefault(null) != null && args.Content.GetValueOrDefault(null) == null)
                throw new InvalidOperationException("Message components must be provided alongside a string content.");

            var msg = await discord.ApiClient.ModifyOriginalInteraction( token, args, options).ConfigureAwait(false);
            return RestInteractionMessage.Create(discord, msg, interaction, interaction.Channel);
        }

        public static async Task DeleteInteractionResponse (BaseDiscordClient discord, IDiscordInteraction interaction, RequestOptions options)
        {
            if (!interaction.IsValidToken)
                throw new InvalidOperationException("Callback token for this interaction has exprired.");

            string token = interaction.Token;

            await discord.ApiClient.DeleteOriginalInteractionResponse( token, options).ConfigureAwait(false);
        }
        #endregion

        #region Followup
        public static async Task<RestFollowupMessage> SendInteractionFollowup (BaseDiscordClient discord, IDiscordInteraction interaction, string text,
            bool isTTS, IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, RequestOptions options)
        {
            Preconditions.AtMost(embeds?.Count() ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            CheckAllowedMentions(allowedMentions);

            if (!interaction.IsValidToken)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token;

            var args = new CreateWebhookMessageParams(text)
            {
                IsTTS = isTTS,
                Embeds = embeds?.Select(x => x?.ToModel()).ToArray(),
                Username = username,
                AvatarUrl = avatarUrl,
                AllowedMentions = allowedMentions?.ToModel()
            };

            var message = await discord.ApiClient.CreateFollowupMessage( token, args, options).ConfigureAwait(false);
            return RestFollowupMessage.Create(discord, message, interaction, interaction.Channel);
        }

        public static async Task<RestFollowupMessage> ModifyFollowupMessage (BaseDiscordClient discord, IDiscordInteraction interaction, ulong messageId,
            string text, AllowedMentions allowedMentions, IEnumerable<Embed> embeds, RequestOptions options)
        {
            Preconditions.AtMost(embeds?.Count() ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            CheckAllowedMentions(allowedMentions);

            if (!interaction.IsValidToken)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token;

            var args = new ModifyWebhookMessageParams()
            {
                Content = text,
                Embeds = embeds?.Select(x => x?.ToModel()).ToArray(),
                AllowedMentions = allowedMentions?.ToModel()
            };
            var message = await discord.ApiClient.ModifyFollowupMessage( token, messageId, args, options).ConfigureAwait(false);
            return RestFollowupMessage.Create(discord, message, interaction, interaction.Channel);
        }

        public static async Task DeleteFollowupMessage (BaseDiscordClient discord, IDiscordInteraction interaction, ulong messageId,
            RequestOptions options)
        {
            if (!interaction.IsValidToken)
                throw new InvalidOperationException("Callback token for this interaction is exprired.");

            string token = interaction.Token;

            await discord.ApiClient.DeleteFollowupMessage( token, messageId, options).ConfigureAwait(false);
        }
        #endregion

        #region Permissions

        public static async Task<IReadOnlyCollection<GuildApplicationCommandPermission>> GetGuildCommandPermissions (BaseDiscordClient discord, IGuild guild,
            RequestOptions options = null)
        {
            var appInfo = await ClientHelper.GetApplicationInfoAsync(discord, options).ConfigureAwait(false);
            var commands = await discord.ApiClient.GetGuildApplicationCommandPermissions(appInfo.Id, guild.Id, options).ConfigureAwait(false);

            var result = new List<GuildApplicationCommandPermission>();

            foreach (var command in commands)
            {
                var perms = new List<ApplicationCommandPermission>();

                foreach (var permission in command.Permissions)
                    perms.Add(new ApplicationCommandPermission(permission.Id, permission.Type, permission.Permission));

                var restCommand = await GetApplicationCommand(discord, command.CommandId, guild, options).ConfigureAwait(false);

                result.Add(new GuildApplicationCommandPermission(restCommand, command.GuildId, perms.ToArray()));
            }
            return result;
        }

        public static async Task<GuildApplicationCommandPermission> GetCommandPermission(BaseDiscordClient discord, IApplicationCommand command,
            RequestOptions options = null )
        {
            var commandPermissions = await discord.ApiClient.GetApplicationCommandPermissions( command.Guild.Id, command.Id, options)
                .ConfigureAwait(false);

            var parsed = new List<ApplicationCommandPermission>();

            foreach(var permission in commandPermissions.Permissions)
                parsed.Add(permission.ToEntity());

            return new GuildApplicationCommandPermission(command, commandPermissions.GuildId, parsed.ToArray());
        }

        public static async Task<GuildApplicationCommandPermission> ModifyCommandPermissions (BaseDiscordClient discord, IApplicationCommand command,
            IEnumerable<ApplicationCommandPermission> permissions, RequestOptions options = null)
        {
            if (permissions.Count() > 10)
                throw new InvalidOperationException("You cannot edit more than 10 permissions at the same time");

            var args = new List<API.ApplicationCommandPermission>();

            foreach(var permission in permissions)
                args.Add(permission.ToModel());

            var modified = await discord.ApiClient.ModifyApplicationCommandPermissions( command.Guild.Id, command.Id, args, options)
                .ConfigureAwait(false);

            return new GuildApplicationCommandPermission(command, modified.GuildId, modified.Permissions.Select(x => x.ToEntity()).ToArray());
        }

        public static async Task<IReadOnlyCollection<GuildApplicationCommandPermission>> BatchModifyGuildCommandPermissions (BaseDiscordClient client, IGuild guild,
            IDictionary<IApplicationCommand, ApplicationCommandPermission[]> args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.Count, 0, nameof(args));

            List<ModifyGuildApplicationCommandPermissions> models = new List<ModifyGuildApplicationCommandPermissions>();

            foreach (var arg in args)
            {
                Preconditions.AtMost(arg.Value.Length, 10, nameof(args));

                var model = new ModifyGuildApplicationCommandPermissions()
                {
                    Id = arg.Key.Id,
                    Permissions = arg.Value.Select(x => new API.ApplicationCommandPermission
                    {
                        Id = x.TargetId,
                        Permission = x.Permission,
                        Type = x.TargetType
                    }).ToArray()
                };

                models.Add(model);
            }

            var apiModels = await client.ApiClient.BatchModifyApplicationCommandPermissions(models.ToArray(), guild.Id, options);
            var commands = await GetApplicationCommands(client, guild, options).ConfigureAwait(false);

            return apiModels.Select(x =>
            {
                var cmd = commands.First(y => y.Id == x.CommandId);
                return new GuildApplicationCommandPermission(cmd, guild.Id,
                    x.Permissions.Select(y => new ApplicationCommandPermission(y.Id, y.Type, y.Permission)).ToArray());
            }).ToImmutableArray();
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
