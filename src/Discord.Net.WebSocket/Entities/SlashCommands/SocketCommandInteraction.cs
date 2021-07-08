using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    /// Represents a Web-Socket based <see cref="IDiscordInteraction"/> that originated from a Slash Command
    /// </summary>
    public class SocketCommandInteraction : SocketInteraction
    {
        /// <summary>
        /// Name of the nested commands, with the order: Command, Sub-Command Group, Sub-Command
        /// </summary>
        public string[] Command { get; private set; }
        /// <summary>
        /// Parameter values input by the user while invoking the command
        /// </summary>
        public IReadOnlyCollection<InteractionParameter> Data { get; private set; }

        internal SocketCommandInteraction (DiscordSocketClient discord, ClientState state, SocketUser user, ISocketMessageChannel channel, Model model)
            : base(discord, state, user, channel, model)
        {
            Update(state, model);
        }

        internal override void Update (ClientState state, Model model)
        {
            Data = ParseParameters(model, out string[] command).ToImmutableArray();
            Command = command;
        }

        private IEnumerable<InteractionParameter> ParseParameters (Model model, out string[] command, string commandDelimiter = " ")
        {
            if (!model.Data.IsSpecified)
                throw new ArgumentException($"Provided Interaction Command Model is not a type of {nameof(SocketCommandInteraction)}");

            var data = model.Data.Value;

            var optionNames = new List<string>();
            optionNames.Add(data.Name);

            if (!data.Options.IsSpecified)
            {
                command = optionNames.ToArray();
                return Enumerable.Empty<InteractionParameter>();
            }  

            var result = new List<InteractionParameter>();
            var children = data.Options.Value;

            do
            {
                foreach (var option in children)
                {
                    var type = option.Type;
                    if (type == ApplicationCommandOptionType.SubCommand || type == ApplicationCommandOptionType.SubCommandGroup)
                    {
                        optionNames.Add(option.Name);
                        children = option.Options.IsSpecified ? option.Options.Value : null;
                    }
                    else
                    {
                        children = null;
                        result.Add(new InteractionParameter(option.Name, option.Value, type));
                    }
                }
            } while (children != null);

            command = optionNames.ToArray();
            return result;
        }
    }
}
