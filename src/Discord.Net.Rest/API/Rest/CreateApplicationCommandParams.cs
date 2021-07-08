using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateApplicationCommandParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }
        [JsonProperty("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        public CreateApplicationCommandParams (string name, string description)
        {
            Preconditions.SlashCommandName(name, nameof(name));
            Preconditions.SlashCommandDescription(description, nameof(description));

            Name = name;
            Description = description;
        }
    }
}
