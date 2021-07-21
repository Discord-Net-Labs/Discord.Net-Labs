# Slash Command Service
Slash Command Service provides a parser for creating attribute based [Discord Application Commands](https://discord.com/developers/docs/interactions/slash-commands)

## Getting Started
Command modules are discovered and instantiated by the **SlashCommandService** which can be created with an optional **SlashCommandServiceConfig**.
Both the **SocketSlashCommand** and **SocketMessageComponent** can be used to execute commands.

Command execution method is very simalar to the one in **Discord.Commands**.

## Creating Modules
All Slash Commands must be declared by a module class which inherits the **SlashModuleBase** base class. Module classes must be public, non-abstract, non-generic classes.
Ex.
```csharp
public class Module : SlashModuleBase<SocketSlashCommandContext>
    {
        [SlashCommand("ping", "recive a pong")]
        public async Task Ping( )
        {
            await Context.Interaction.RespondAsync("pong");
        }
    }
 ```
 Slash Command Modules are analogous to the Command Module of **Discord.Commands** thereby they are also very  simalar to ASP.NET Core Controller pattern. A separate instance class is created for every individual command execution and then disposed right after the it. 
 
Dependency Injection pattern can be utilized while using **SlashCommandService**.

## Creating Commands
Commands / Slash Command handlers are class functions with a return type of `Task`.
Just like in the original **Commands** extension, you should avoid long running functions that could interrupt your bots execution. 

Whenever you want to flag a function as a **SlashCommand** you should use the `[SlashCommandAttribute]`.

Discord's SlashCommands API defines a command either as a standalone command or as a group that contains other Sub-Command Groups or Sub Commands, while creating your commands you can stack command keywords by using the `[SlashGroupAttribute]`.`[SlashGroupAttribute]` can be applied to methods or classes. A `[SlashGroupAttribute]` applied to a module class will affect every method in that class.

```csharp
[SlashGroup("utility", "utility commands")]
public class FirstModule: CommandBase<SocketSlashCommandContext>
    {
        [SlashCommand("ping", "recieve a pong")]
        public async Task Ping ( )
        {
            await Context.Interaction.RespondAsync("pong");
        }
		
		[SlashGroup("misc", "miscellaneous commands")]
		[SlashCommand("ping", "recieve a pong")]
		public async Task Ping2 ( )
		{
			await Context.Interaction.RespondAsync("pong2");
		}
	}
public class SecondModule : CommandBase<SocketSlashCommandContext>
    {
        [SlashCommand("echo", "echoes the input")]
        public async Task Ping ( string text )
        {
            await Context.Interaction.RespondAsync(text);
        }
	}
```

For example, for the two module definitions given above there are 3 functions that can be invoked with "utility ping", "utility misc ping", "echo" commands in that order.

## Creating Interaction Handlers
With the new Discord Interactions API you can send messages with interactive UI elements that are called "Message  Components" every interactable Message Component has a Custom ID that can be used to execute an Interaction Handler.

Interaction Handlers are registered in the same way as the Slash Command Handlers. Only this you need to use the `[InteractionAttribute]` instead of the `[SlashCommandAttribute]`. This function will be invoked whenever an Interaction is received from a Message Component  with the matching Custom ID.
Interaction Handlers are also must be declared by a Slash Command Module and `[SlashGroupAttribute]` can also be applied to Interaction Handlers and their declaring class, in that case `,` character is treated as the group  name delimiter.

Interaction Handler Ex. for **SelectMenu** Component with `'select_song'` Custom ID:
```csharp
public class InteractionHandlers
{
	[Interaction("select_song")]
        public async Task Song ( params string[] values )
        {
            await Context.Interaction.FollowupAsync($"you selected {string.Join(',', values)}");
        }
}
```
Interaction Handler Ex. for **SelectMenu** Component with `'utility,select_song'` Custom ID:
```csharp
[SlashGroup("utility")]
public class InteractionHandlers
{
[Interaction("select_song")]
        public async Task Song ( params string[] values )
        {
            await Context.Interaction.FollowupAsync($"you selected {string.Join(',', values)}");
        }
}
```

## Choices
When you register a Slash Command you can define choices for a specific command parameter,
choices are defined using the `[ChoiceAttribute]`. 
```csharp
[SlashCommand('favanimal', 'select your favourite animal')]
public async Task Animal([Choice("dog", "dog"), Choice("cat", "cat")]string type)
{
	await Context.Interaction.RespondAsync($"Your favourite animal is: {type}");
}
```

## DefaultPermission
Default permission of a command is set by using the `[DefaultPermissionAttribute]`

## Parameter Name and Description
By default, a function parameters name is registered as the name and description of its Discord counterpart. You can customize the name or description of the **SlashCommand Option** by using the `[SummaryAttribute]`.

## Optional Parameters
To create an optional **SlashCommand Option** you don't need to add any additional
work, just give it a default value. `string text = "default string"`

## Command Context
Every module exposes an execution context for its methods to use. This context is very similar to its counterpart in  the  **Discord.Commands**. There are three variants of **SlashCommandContext** you can choose from. The base **SlashCommandContext**, **SocketSlashCommandContext**, **ShardedSlashCommandContext**.

## Auto Module Discovery
Slash Command Service can discover and load modules and commands in an `Assembly`. 
For that you just need to invoke the `SlashCommandService.AddModules` method.

## Registering the Loaded Modules to Discord
You can invoke the `SlashCommandService.SyncCommands` method to register the previously discovered modules to Discord.

## Command Execution
When the underlying Discord client receives an `InteractionCreated` event you can use the `SocketInteraction` event argument to execute the Slash Command handlers using the `SlashCommandService.ExecuteCommandAsync` and `SlashCommandService.ExecuteInteractionAsync` methods. To execute these methods you need to create a `SlashCommandContext` of the same type that you used with `SlashModuleBase` generic class when you created your modules.

`SlashCommandService.ExecuteCommandAsync` expects a `string[]` that contains the command keywords of the executed Slash Command which can be obtained by typecasting the `SocketInteraction` into `SocketSlashCommand` and getting the `Command` property.

`SlashCommandService.ExecuteInteractionAsync` expects a `string` that contains the Custom ID of the Message Component that raised the Interaction event which can be obtained by typecasting the `SocketInteraction` into `SocketMessageComponent` and getting the `Data.CustomId` property.

Ex.
```csharp
public async Task Discord_InteractionRecieved ( SocketInteraction arg )
        {
            var ctx = new SocketSlashCommandContext(Discord, arg);

            if (arg is SocketSlashCommand commandInteraction)
                    await Slash.ExecuteCommandAsync(ctx, commandInteraction.Command, Services);
            else if (arg is SocketMessageComponent messageInteraction)
                    await Slash.ExecuteInteractionAsync(ctx, messageInteraction.Data.CustomId, Services);
        }
```