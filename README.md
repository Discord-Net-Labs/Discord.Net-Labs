# Discord.Net Labs
[![NuGet](https://img.shields.io/nuget/vpre/Discord.Net.Labs.svg?maxAge=2592000?style=plastic)](https://www.nuget.org/packages/Discord.Net.Labs)
[![Discord](https://discord.com/api/guilds/848176216011046962/widget.png)](https://discord.gg/dvSfUTet3K)

This repo is a custom fork of Discord.Net that introduces the newest features of discord for testing and experimenting. Nothing here is guaranteed to work but you are more than welcome to submit bugs in the issues tabs

## Known issues
Labs will not work with normal package of Playwo's [InteractivityAddon](https://www.nuget.org/packages/Discord.InteractivityAddon). The reason is that his package depends on the base discord.net lib. You can instead use the [InteractivityAddon.Labs](https://www.nuget.org/packages/Discord.InteractivityAddon.Labs) package which implements some of the features added in Discord.Net-Labs.

## How to use
Setting up labs in your project is really simple, here's how to do it:
1) Remove Discord.Net from your project
2) Add Discord.Net Labs nuget to your project
3) Enjoy!

## Branches
### Dev
The main branch we pull off of to introduce new features into, the dev branch is the same as Discord.Nets dev branch

### Interactions
This branch is for anything todo with Discord Interactions, such as [Slash commands](https://discord.com/developers/docs/interactions/slash-commands) and [Message Components](https://discord.com/developers/docs/interactions/message-components). This branch is stable enough to use but does not contain all the features of interactions. 

### SlashCommandService
This branch is on pause and does not work currently, Once everything is stable with the Interaction branch we will continue working on a slash command service for it.

### web/SlashCommandService
webmilio's spin on the SlashCommandService branch, again the state of this is unknown. 

## Listening for interactions
```cs
// Subscribe to the InteractionCreated event
client.InteractionCreated += Client_InteractionCreated;

...
private async Task Client_InteractionCreated(SocketInteraction interaction)
{
  switch (interaction) // We want to check the type of this interaction
  {
    // Slash commands
    case SocketSlashCommand commandInteraction:
      await MySlashCommandHandler(commandInteraction);
      break;
      
    // Button clicks/selection dropdowns
    case SocketMessageComponent componentInteraction:
      await MyMessageComponentHandler(componentInteraction);
      break;
      
    // Unused or Unknown/Unsupported
    default:
      break;
  }
}
```

### Simple handling slash commands
```cs
private async Task MySlashCommandHandler(SocketSlashCommand interaction)
{
    // Checking command name
    if (interaction.Data.Name == "ping")
    {
      // Respond to interaction with message.
      // You can also use "ephemeral" so that only the original user of the interaction sees the message
      await interaction.RespondAsync($"Pong!", ephemeral: true);
      
      // Also you can followup with a additional messages, which also can be "ephemeral"
      await interaction.FollowupAsync($"PongPong!", ephemeral: true);
    }
}
```

### Simple handling button clicks and selection dropdowns
```cs
private async Task MyMessageComponentHandler(SocketMessageComponent interaction)
{
    // Get the custom ID 
    var customId = interaction.Data.CustomId;
    // Get the user
    var user = (SocketGuildUser) interaction.User;
    // Get the guild
    var guild = user.Guild;
    
    // Respond with the update message. This edits the message which this component resides.
    await interaction.UpdateAsync(msgProps => msgProps.Content = $"Clicked {interaction.Data.CustomId}!");
    
    // Also you can followup with a additional messages
    await interaction.FollowupAsync($"Clicked {interaction.Data.CustomId}!", ephemeral: true);
    
    // If you are using selection dropdowns, you can get the selected label and values using these
    var selectedLabel = ((SelectMenu) interaction.Message.Components.First().Components.First()).Options.FirstOrDefault(x => x.Value == interaction.Data.Values.FirstOrDefault())?.Label;
    var selectedValue = interaction.Data.Values.First();
}
```

> Note: The example above assumes that the selection dropdown is expecting only 1 returned value, if you configured your dropdown for multiple values, you'll need to modify the code slightly.

### Sending messages with buttons
Theres a new field in all `SendMessageAsync` functions that takes in a `MessageComponent`, you can use it like so:
```cs
var builder = new ComponentBuilder().WithButton("Hello!", customId: "id_1", ButtonStyle.Primary, row: 0);
await Context.Channel.SendMessageAsync("Test buttons!", component: builder.Build());
```

### Sending messages with selection dropdowns
Theres a new field in all `SendMessageAsync` functions that takes in a `MessageComponent`, you can use it like so:
```cs
var builder = new ComponentBuilder()
  .WithSelectMenu(new SelectMenuBuilder()
  .WithCustomId("id_2")
  .WithPlaceholder("This is a placeholder")
  .AddOption(
    label: "Option",
    value: "value1",
    description: "Evan pog champ",
    emote: Emote.Parse("<:evanpog:810017136814194698>")
  )
  .AddOption("Option B", "value2", "Option B is poggers");
  
await Context.Channel.SendMessageAsync("Test selection!", component: builder.Build());
```

> Note: You can only have 5 buttons per row and 5 rows per message. If a row contains a selection dropdown it cannot contain any buttons.

## Slash commands
Slash command example how to's can be found [here](https://github.com/Discord-Net-Labs/Discord.Net-Labs/tree/Interactions/docs/guides/slash-commands). If you want to read some code using slash commands, you can do that [here](https://github.com/quinchs/SwissbotCore/blob/master/SwissbotCore/Handlers/AutoMod/Censor.cs)
