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

        public static InteractionResponse ToEntity (this API.InteractionResponse response)
        {
            if (!response.Data.IsSpecified)
                return new InteractionResponse(response.Type);

            var data = response.Data.Value;

            bool isTTS = false;
            AllowedMentions allowedMentions = null;
            Embed[] embeds = null;
            string content = null;
            MessageComponent[] messageComponents = null;

            if (data.Content.IsSpecified)
                content = data.Content.Value;
            if (data.TTS.IsSpecified)
                isTTS = data.TTS.Value;
            if (data.Embeds.IsSpecified)
                embeds = data.Embeds.Value.Select(x => x.ToEntity()).ToArray();
            if (data.AllowedMentions.IsSpecified)
                allowedMentions = data.AllowedMentions.Value.ToEntity();
            if (data.Components.IsSpecified)
                messageComponents = data.Components.Value.Select(x => x.ToEntity()).ToArray();

            return new InteractionResponse(response.Type)
            {
                IsTTS = isTTS,
                Content = content,
                Embeds = embeds,
                AllowedMentions = allowedMentions,
                MessageComponents = messageComponents
            };
        }

        public static API.ApplicationCommandOption ToModel (this IApplicationCommandOption entity)
        {
            return new API.ApplicationCommandOption()
            {
                Name = entity.Name,
                Description = entity.Description,
                Type = entity.OptionType,
                Required = entity.IsRequired,
                Options = entity.Options?.Select(x => x?.ToModel()).ToArray(),
                Choices = entity.Choices?.Select(x => new API.ApplicationCommandOptionChoice()
                {
                    Name = x.Key,
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
        public static API.MessageComponent ToModel (this MessageComponent entity)
        {
            switch (entity)
            {
                case MessageActionRowComponent actionRow:
                    {
                        return new API.MessageComponent
                        {
                            Type = MessageComponentType.ActionRow,
                            Components = actionRow.MessageComponents.Select(x => x.ToModel()).ToArray()
                        };
                    }
                case MessageButtonComponent button:
                    {
                        return new API.MessageComponent
                        {
                            Type = MessageComponentType.Button,
                            Label = button.Label,
                            CustomId = button.CustomId,
                            Url = button.Url,
                            Emoji = button.Emoji != null ? new API.Emoji
                            {
                                Name = button.Emoji?.Name,
                                Id = button.Emoji?.Id,
                                Animated = button.Emoji?.Animated
                            } : null,
                            Style = button.Style,
                            Disabled = button.IsDisabled
                        };
                    }
                case MessageSelectMenuComponent select:
                    {
                        return new API.MessageComponent
                        {
                            Type = MessageComponentType.SelectMenu,
                            Placeholder = select.Placeholder,
                            MaxValues = select.MaxValues,
                            MinValues = select.MinValues,
                            Options = select.Options.Select(x => x.ToModel()).ToArray(),
                            CustomId = select.CustomId
                        };
                    }
                default:
                    throw new ArgumentException("Not supported message component type.");
            }
        }

        public static API.SelectOption ToModel (this SelectOption entity) =>
            new API.SelectOption
            {
                Label = entity.Label,
                Value = entity.Value,
                Description = entity.Description,
                Default = entity.IsDefault,
                Emoji = entity.Emoji != null ? new API.Emoji
                {
                    Name = entity.Emoji?.Name,
                    Id = entity.Emoji?.Id,
                    Animated = entity.Emoji?.Animated
                } : null
            };

        public static MessageComponent ToEntity (this API.MessageComponent component)
        {
            switch (component.Type)
            {
                case MessageComponentType.ActionRow:
                    {
                        IEnumerable<MessageComponent> children = null;
                        if (component.Components.IsSpecified)
                            children = component.Components.Value.Select(x => x.ToEntity());

                        return new MessageActionRowComponent(children);
                    }
                case MessageComponentType.Button:
                    {
                        string label = component.Label.GetValueOrDefault(null);
                        string customId = component.CustomId.GetValueOrDefault(null);
                        var emoji = component.Emoji.GetValueOrDefault(null).ToEntity();
                        var style = component.Style.GetValueOrDefault();
                        string url = component.Url.GetValueOrDefault(null);
                        bool isDisabled = component.Disabled.GetValueOrDefault();
                        return new MessageButtonComponent(label, customId, url, emoji, style, isDisabled);
                    }
                case MessageComponentType.SelectMenu:
                default:
                    throw new ArgumentException("Unknown component type");
            }
        }
    }
}
