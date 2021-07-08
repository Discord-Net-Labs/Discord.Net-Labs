using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyApplicationCommandParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("options")]
        public ApplicationCommandOption[]? Options { get; set; }
        [JsonProperty("default_permission")]
        public bool DefaultPermission { get; set; }

        public ModifyApplicationCommandParams (string name, string description)
        {
            Preconditions.SlashCommandName(name, nameof(name));
            Preconditions.SlashCommandDescription(description, nameof(description));

            Name = name;
            Description = description;
        }
    }
}
