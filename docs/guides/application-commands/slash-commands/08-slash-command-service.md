# Slash Command Service
Slash Command Service provides a parser for creating attribute based [Discord Application Commands](https://discord.com/developers/docs/interactions/slash-commands).

Slash Commands extension can be used without installing any additional libraries (except from the core Discord.NET libraries like Rest and WebSocket)

## Getting Started
`SlashCommandService` is a singleton service for creating attribute based *SlashCommand* and *MessageComponent* interaction handlers. Before you can start using this command service, you need to create an instance of it, optionally with a `SlashCommandServiceConfig`. Command execution and discovery methods are very simalar to the ones in `Discord.Commands` extension.

## Commands
*MessageComponent*/*SlashCommand* interaction handlers must be public methods with the return type `Task`. They must be declared by a class that inherits the base module class (`SlashModuleBase`). They also have to be tagged with a `[SlashCommandAttribute]` if it is a SlashCommand handler, name and description values of this attribute will be used to register this command as an *ApplicationCommand* to Discord. If a method should be used as a *MessageComponent* interaction handler, then you should use the `[InteractionAttribute]`(This attribute type is only used internally and it won't affect the SlashCommands of the application ). You are free to use any type of parameter as long as there is a valid `TypeReader` registered to the `SlashCommandService` for that type (though *MessageComponent* must be parametersless if they will handle button interactions, and they must have `params string[]` parameter if they will handle select menu interactions). By default, the `SlashCommandService` can parse these types:

 - Types that implement `IChannel`
 - Types that implement `IRole`
 - Types that implement `IUser`
 - Types that implement `IMentionable`
 - Enums (Default `TypeReader` automatically adds parameters choices)
 - `TimeSpan`
 - Types that implement `IConvertable` (ie. string, int, bool, DateTime... [full list](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible?view=net-5.0))

You can create and add your own `TypeReaders`. [For more info...](#TypeReaders)

By default the name and description values displayed in the Discord Client for any parameter will be the same as name of that parameter. To customize them, you can use the `[SummaryAttribute]`.

You can create parameter choices using the `[ChoiceAttribute]`.

Parameters will be registered as optional if they have a default value.

Ex. Component Interaction Handler:
```csharp
[Interaction("test_menu")]
public async Task SelectMenu ( params string[] args )
{
    await RespondAsync(" You've selected " + String.Join(" and ", args));
}
```

Ex. Slash Command Interaction Handler:
```csharp
[SlashCommand("ping", "recieve a pong")]
public async Task Ping()
{
	await RespondAsync("pong");
}
```

Ex. User Command Interaction Handler:
```csharp
[UserCommand("Tag")]
public async Task Tag(IUser user)
{
	...
}
```

Ex. Message Command Interaction Handler:
```csharp
[UserCommand("Bookmark")]
public async Task Bookmark(IMessage message)
{
	...
}
```

### Message Component Interaction Specific Behaviour
`[InteractionAttribute]` supports a wild card pattern where you can use `*` character in place of custom ID variables. For instance, an interaction handler flaged with the following attribute `[Interaction("music:*")]` will handle message component interactions like *music:1*, *music:play* etc. This functionality is useful if you want to differentiate between similar message components from different slash commands. 
You may use as many `*`s as you want and you can access these wild card values by adding `string` parameters to the method.  This pattern is best suited for pinned messages with message components like, role selection messages for community servers.

Ex.
```csharp
[Interaction("music:*,*")]
public async Task TestInteraction(string id, string value)
{
	...
}
```
if a message component with the custom id: *music:5,test* is interacted with, id will be `5` and the value parameter will be `test`.

## Modules
Modules are non-abstract, non-generic, public classes that inherit `SlashModuleBase<>` and they provide a model similar to `Discord.Commands` modules and ASP.NET Core controllers, for loading and executing methods. 

Every module has a life span of one command execution. 

You can add group names to modules and sub-modules by using the `[SlashGroupAttribute]`. `SlashCommandService` will use this attribute (if present) to create the Slash Command hierachy. For instance, if a command is situated in a module without a Group name, then the command will be registered as a standalone **SlashCommand** but if a `[SlashGroupAttribute]` is used to tag that module, then the command is registered as a **Sub-Command**.

```csharp
public class Module : SlashModuleBase<SocketSlashCommandContext>
    {
        [SlashCommand("ping", "recive a pong")]
        public async Task Ping( )
        {
            await RespondAsync("pong");
        }
    }
 ```
 Slash Command: `ping`
 
 ---
 
```csharp
[SlashGroup("utility", "utility commands")]
public class Module : SlashModuleBase<SocketSlashCommandContext>
    {
        [SlashCommand("ping", "recive a pong")]
        public async Task Ping( )
        {
            await RespondAsync("pong");
        }
    }
 ```
 Slash Command: `utility ping`

Nested modules can also be created, and using `[SlashGroupAttribute]` will in return create **Sub-Command Groups** but no command should have more than 2 parents with `[SlashGroupAttribute]`s.

### Adding the Modules
You can add the modules manually or use the auto discovery method provided by the Slash Command Service.
```csharp
await  SlashCommands.AddModules(System.Reflection.Assembly.GetExecutingAssembly(), Services);
```

## TypeReaders
TypeReaders are an essential part of the `SlashCommandService`. They are the components that determine the Discord Option Type of a parameter while registering, and parse the incoming data into the requested parameter type. 
`TypeReader`s can be added to a `SlashCommandService` instance by using one of two methods, you can either use the `AddGenericTypeReader` or `AddTypeReader` method. Every user defined `TypeReader` must implement the abstract `TypeReader` class. Your `TypeReader` class can be a concrete class or a generic class with one generic type parameter. 

Every `TypeReader` must have a `GetDiscordType` method that provides the `SlashCommandService` with a Discord Option Type, which should be used to retrieve the data from the user. While this method is used to determine the external parameter type, you need to declare the method body for `CanConvertTo` method, which is used to determine the internal parameter type that can be handled by this `TypeReader`. Alternatively you can derive the `TypeReader<T>` abstract class instead of `TypeReader` which will generate the `CanConvertTo` for you and it will return true for every type that is derived from `T`.

You must also implement the `ReadAsync` method for the `SlashCommandService` to work properly. You are required to return a `TypeReaderResult` with the read result, if successful, with the error reason, if unsuccessful. The internal logic of this method is totally up to you, but you should avoid using long running operations inside this method. 
Optionally, you can utilize the virtual `Write` method to manipulate the parameter properties before they are sent to Discord for command registration. This operation will be performed on every command parameter that use this `TypeReader`. For example, in the default enum `TypeReader` this method is used to add the enum values as options to the parameter to be displayed in Discord.

Every `TypeReader` must be added to the `SlashCommandService` instance before the module discovery, otherwise the command service will raise an exception whenever it encounters a parameter type that it can't handle. During the module discovery, the `SlashCommandService` will seek `TypeReader`s for the corresponding type, if it cannot find a suitable `TypeReader` it will try to get a `TypeReader` that "Can Convert To" this parameter type. If all fails, it will try to create a `TypeReader` instance for the most suitable generic `TypeReader` (if there is any overlap between the generic `TypeReader` types, the most specific one will be selected).

## Attributes
### DefaultPermission
To set the default permission of a command or group you can use the `[DefaultPermissionAttribute]`. For more information on command permissions, [see](https://discord.com/developers/docs/interactions/slash-commands#permissions).
### Summary Attribute
By default, a method parameters name is displayed in place of the name and description fields of the Discord Command parameter whenever it is highlighted in the Discord client. To customize this information, use the `[SummaryAttribute]`. 
### Choice Attribute
To add option choices to a command parameter, use the `[ChoiceAttribute]`. Alternatively, you create an enum for this parameter and use that instead. Declaring enum method arguments will generate choices automatically based on that enum types names. 

## Handling Interactions
```csharp
client.InteractionCreated += Client_InteractionCreated;
IServiceProvider services;
...
private async Task Client_InteractionCreated(SocketInteraction arg)

{
	var ctx = new ShardedSlashCommandContext(_client, arg);

	switch (arg)
		{
			case SocketSlashCommand command:
				await _slash.ExecuteCommandAsync(ctx, command.GetCommandKeywords(), _services);
				break;
			case SocketMessageComponent component:
				await _slash.ExecuteInteractionAsync(ctx, component.Data.CustomId, _services);
				break;
			case SocketUserCommand:
			case SocketMessageCommand:
				await _slash.ExecuteContextCommandAsync(ctx, arg.Data.Name, _services);
				break;
			default:
			break;
		}
}
```

## Registering Commands
You can use the included `RegisterCommandsToGuild()` and `RegisterCommandsGlobally()` methods to register the internal commands to Discord.  **You must call these methods after the bot client is ready.** If you are in a debug environment, you should register your commands to the guild that you are testing the commands on. I recommend you to use a snippet like the following,
```csharp
client.Ready += Client_Ready;

...
private async Task Client_Ready()
{
	if(IsDebug())
		await _slash.SyncCommands(<testGuildId>);
	else
		await _slash.SyncCommands();
}

private static bool IsDebug ( )
{
	#if DEBUG
		return true;
	#else
		return false;
	#endif
}
```

## Waiting for an Interaction
Since the interactive UI elements are here, you might want to use them to get a user selection or a confirmation inside a command handler. To do this, you can use the `InteractionUtility.WaitForInteraction()` method. By using this method you can asynchronously wait for an interaction. This method uses the provided predicate to filter the incoming interactions and returns the first valid interaction. If no valid interaction is recieved during the waiting period, the Task result will be equals to `null`.

**This method should not be used for long waiting periods, since it prevents the command module from being disposed. Instead, you should use the `[InteractionAttribute]` for such operations.**