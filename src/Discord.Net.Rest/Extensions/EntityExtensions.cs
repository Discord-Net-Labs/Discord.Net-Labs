using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Rest
{
    internal static class EntityExtensions
    {
        public static IEmote ToIEmote(this API.Emoji model)
        {
            if (model.Id.HasValue)
                return model.ToEntity();
            return new Emoji(model.Name);
        }

        public static GuildEmote ToEntity(this API.Emoji model)
            => new GuildEmote(model.Id.Value,
                model.Name,
                model.Animated.GetValueOrDefault(),
                model.Managed,
                model.RequireColons,
                ImmutableArray.Create(model.Roles),
                model.User.IsSpecified ? model.User.Value.Id : (ulong?)null);

        public static Embed ToEntity(this API.Embed model)
        {
            return new Embed(model.Type, model.Title, model.Description, model.Url, model.Timestamp,
                model.Color.HasValue ? new Color(model.Color.Value) : (Color?)null,
                model.Image.IsSpecified ? model.Image.Value.ToEntity() : (EmbedImage?)null,
                model.Video.IsSpecified ? model.Video.Value.ToEntity() : (EmbedVideo?)null,
                model.Author.IsSpecified ? model.Author.Value.ToEntity() : (EmbedAuthor?)null,
                model.Footer.IsSpecified ? model.Footer.Value.ToEntity() : (EmbedFooter?)null,
                model.Provider.IsSpecified ? model.Provider.Value.ToEntity() : (EmbedProvider?)null,
                model.Thumbnail.IsSpecified ? model.Thumbnail.Value.ToEntity() : (EmbedThumbnail?)null,
                model.Fields.IsSpecified ? model.Fields.Value.Select(x => x.ToEntity()).ToImmutableArray() : ImmutableArray.Create<EmbedField>());
        }
        public static RoleTags ToEntity(this API.RoleTags model)
        {
            return new RoleTags(
                model.BotId.IsSpecified ? model.BotId.Value : null,
                model.IntegrationId.IsSpecified ? model.IntegrationId.Value : null,
                model.IsPremiumSubscriber.IsSpecified ? true : false);
        }
        public static API.Embed ToModel(this Embed entity)
        {
            if (entity == null) return null;

            var model = new API.Embed
            {
                Type = entity.Type,
                Title = entity.Title,
                Description = entity.Description,
                Url = entity.Url,
                Timestamp = entity.Timestamp,
                Color = entity.Color?.RawValue
            };
            if (entity.Author != null)
                model.Author = entity.Author.Value.ToModel();
            model.Fields = entity.Fields.Select(x => x.ToModel()).ToArray();
            if (entity.Footer != null)
                model.Footer = entity.Footer.Value.ToModel();
            if (entity.Image != null)
                model.Image = entity.Image.Value.ToModel();
            if (entity.Provider != null)
                model.Provider = entity.Provider.Value.ToModel();
            if (entity.Thumbnail != null)
                model.Thumbnail = entity.Thumbnail.Value.ToModel();
            if (entity.Video != null)
                model.Video = entity.Video.Value.ToModel();
            return model;
        }
        public static API.AllowedMentions ToModel(this AllowedMentions entity)
        {
            return new API.AllowedMentions()
            {
                Parse = entity.AllowedTypes?.EnumerateMentionTypes().ToArray(),
                Roles = entity.RoleIds?.ToArray(),
                Users = entity.UserIds?.ToArray(),
                RepliedUser = entity.MentionRepliedUser ?? Optional.Create<bool>(),
            };
        }
        public static AllowedMentions ToEntity (this API.AllowedMentions model)
        {
            return new AllowedMentions()
            {
                MentionRepliedUser = model.RepliedUser.IsSpecified ? model.RepliedUser.Value : null,
                RoleIds = model.Roles.IsSpecified ? model.Roles.Value.ToList() : null,
                UserIds = model.Users.IsSpecified ? model.Users.Value.ToList() : null,
            };
        }
        public static API.MessageReference ToModel (this MessageReference entity)
        {
            return new API.MessageReference()
            {
                ChannelId = entity.InternalChannelId,
                GuildId = entity.GuildId,
                MessageId = entity.MessageId,
            };
        }
        public static IEnumerable<string> EnumerateMentionTypes(this AllowedMentionTypes mentionTypes)
        {
            if (mentionTypes.HasFlag(AllowedMentionTypes.Everyone))
                yield return "everyone";
            if (mentionTypes.HasFlag(AllowedMentionTypes.Roles))
                yield return "roles";
            if (mentionTypes.HasFlag(AllowedMentionTypes.Users))
                yield return "users";
        }
        public static EmbedAuthor ToEntity(this API.EmbedAuthor model)
        {
            return new EmbedAuthor(model.Name, model.Url, model.IconUrl, model.ProxyIconUrl);
        }
        public static API.EmbedAuthor ToModel(this EmbedAuthor entity)
        {
            return new API.EmbedAuthor { Name = entity.Name, Url = entity.Url, IconUrl = entity.IconUrl };
        }
        public static EmbedField ToEntity(this API.EmbedField model)
        {
            return new EmbedField(model.Name, model.Value, model.Inline);
        }
        public static API.EmbedField ToModel(this EmbedField entity)
        {
            return new API.EmbedField { Name = entity.Name, Value = entity.Value, Inline = entity.Inline };
        }
        public static EmbedFooter ToEntity (this API.EmbedFooter model)
        {
            return new EmbedFooter(model.Text, model.IconUrl, model.ProxyIconUrl);
        }
        public static API.EmbedFooter ToModel(this EmbedFooter entity)
        {
            return new API.EmbedFooter { Text = entity.Text, IconUrl = entity.IconUrl };
        }
        public static EmbedImage ToEntity(this API.EmbedImage model)
        {
            return new EmbedImage(model.Url, model.ProxyUrl,
                  model.Height.IsSpecified ? model.Height.Value : (int?)null,
                  model.Width.IsSpecified ? model.Width.Value : (int?)null);
        }
        public static API.EmbedImage ToModel(this EmbedImage entity)
        {
            return new API.EmbedImage { Url = entity.Url };
        }
        public static EmbedProvider ToEntity(this API.EmbedProvider model)
        {
            return new EmbedProvider(model.Name, model.Url);
        }
        public static API.EmbedProvider ToModel(this EmbedProvider entity)
        {
            return new API.EmbedProvider { Name = entity.Name, Url = entity.Url };
        }
        public static EmbedThumbnail ToEntity(this API.EmbedThumbnail model)
        {
            return new EmbedThumbnail(model.Url, model.ProxyUrl,
                  model.Height.IsSpecified ? model.Height.Value : (int?)null,
                  model.Width.IsSpecified ? model.Width.Value : (int?)null);
        }
        public static API.EmbedThumbnail ToModel(this EmbedThumbnail entity)
        {
            return new API.EmbedThumbnail { Url = entity.Url };
        }
        public static EmbedVideo ToEntity(this API.EmbedVideo model)
        {
            return new EmbedVideo(model.Url,
                  model.Height.IsSpecified ? model.Height.Value : (int?)null,
                  model.Width.IsSpecified ? model.Width.Value : (int?)null);
        }
        public static API.EmbedVideo ToModel(this EmbedVideo entity)
        {
            return new API.EmbedVideo { Url = entity.Url };
        }

        public static API.Image ToModel(this Image entity)
        {
            return new API.Image(entity.Stream);
        }

        public static Overwrite ToEntity (this API.Overwrite model)
        {
            return new Overwrite(model.TargetId, model.TargetType, new OverwritePermissions(model.Allow, model.Deny));
        }

        public static API.ApplicationCommandOption ToModel (this IApplicationCommandOption entity)
        {
            return new API.ApplicationCommandOption()
            {
                Name = entity.Name,
                Description = entity.Description,
                Type = entity.Type,
                Required = (bool)entity.Required,
                Options = entity.Options?.Select(x => x?.ToModel()).ToArray(),
                Choices = entity.Choices?.Select(x => new API.ApplicationCommandOptionChoice()
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToArray()
            };
        }

        public static API.ApplicationCommand ToModel (this IApplicationCommand entity)
        {
            var model = new API.ApplicationCommand
            {
                Name = entity.Name,
                Description = entity.Description,
                Id = entity.Id,
                ApplicationId = entity.ApplicationId,
                DefaultPermission = entity.DefaultPermission,
                Options = entity.Options.Select(x => x.ToModel()).ToArray()
            };

            if (entity.Guild != null)
                model.GuildId = entity.Guild.Id;

            return model;
        }

        public static API.ApplicationCommandPermission ToModel(this ApplicationCommandPermission entity)
        {
            return new API.ApplicationCommandPermission
            {
                Id = entity.TargetId,
                Permission = entity.Permission,
                Type = entity.TargetType
            };
        }

        public static ApplicationCommandPermission ToEntity(this API.ApplicationCommandPermission model)
        {
            return new ApplicationCommandPermission(model.Id, model.Type, model.Permission);
        }

        public static API.SelectMenuComponent ToModel (this SelectMenu entity) =>
            new API.SelectMenuComponent
            {
                CustomId = entity.CustomId,
                MaxValues = entity.MaxValues,
                MinValues = entity.MinValues,
                Placeholder = entity.Placeholder,
                Options = entity.Options.Select(x => x.ToModel()).ToArray(),
                Type = entity.Type
            };

        public static API.SelectMenuOption ToModel(this SelectMenuOption entity)
        {
            var model = new API.SelectMenuOption
            {
                Label = entity.Label,
                Value = entity.Value,
                Description = entity.Description,
                Default = entity.Default.HasValue ? entity.Default.Value : Optional<bool>.Unspecified
            };

            if (entity.Emote != null)
            {
                if (entity.Emote is Emote e)
                {
                    model.Emoji = new API.Emoji()
                    {
                        Name = e.Name,
                        Animated = e.Animated,
                        Id = e.Id,
                    };
                }
                else
                {
                    model.Emoji = new API.Emoji()
                    {
                        Name = entity.Emote.Name
                    };
                }
            }
            return model;
        }

        public static API.ButtonComponent ToModel(this ButtonComponent entity)
        {
            var model = new API.ButtonComponent
            {
                Type = entity.Type,
                Style = entity.Style,
                Label = entity.Label,
                CustomId = entity.CustomId,
                Url = entity.Url,
                Disabled = entity.Disabled
            };

            if (entity.Emote != null)
            {
                if (entity.Emote is Emote e)
                {
                    model.Emote = new API.Emoji()
                    {
                        Name = e.Name,
                        Animated = e.Animated,
                        Id = e.Id,
                    };
                }
                else
                {
                    model.Emote = new API.Emoji()
                    {
                        Name = entity.Emote.Name
                    };
                }
            }
            return model;
        }

        public static API.ActionRowComponent ToModel(this ActionRowComponent entity) =>
            new API.ActionRowComponent
            {
                Components = entity.Components?.Select<IMessageComponent, IMessageComponent>(x =>
                {
                    switch (x.Type)
                    {
                        case ComponentType.SelectMenu:
                            return ( x as SelectMenu )?.ToModel();
                        case ComponentType.Button:
                            return ( x as ButtonComponent )?.ToModel();
                        default:
                            return null;
                    }
                }).ToArray(),
                Type = entity.Type
            };

        public static API.ApplicationCommandOption ToModel (this ApplicationCommandOptionProperties props)
        {
            return new API.ApplicationCommandOption
            {
                Name = props.Name,
                Description = props.Description,
                Required = (bool)props.Required,
                Type = props.Type,
                Choices = props.Choices.Select(x => new API.ApplicationCommandOptionChoice
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToArray(),
                Options = props.Options.Select(x => x.ToModel()).ToArray()
            };
        }
    }
}
