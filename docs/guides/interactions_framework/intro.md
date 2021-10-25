# Getting Started

Interaction Service provides an attribute based framework for creating Discord Interaction handlers.

To start using the Interaction Service, you need to create a service instance. Optionally you can provide the `InterctionService` constructor with a `InteractionServiceConfig` to change the services behaviour to suit your needs.

```csharp
...

var commands = new InteractionService(discord);

...
```

## Modules

Attribute based Interaction handlers must be defined within a command module class. Command modules are responsible for executing the Interaction handlers and providing them with the necessary execution info and helper functions.

Command modules are transient objects. A new module instance is created before a command execution starts then it will be disposed right after the method returns.

Every module class must:

- be public
- inherit `InteractionModuleBase`

Optionally you can override the included :

- OnModuleBuilding (executed after the module is built)
- BeforeExecute (executed before a command execution starts)
- AfterExecute (executed after a command execution concludes)

methods to configure the modules behaviour.

Every command module exposes a set of helper methods, namely:

- `RespondAsync()` => Respond to the interaction
- `FollowupAsync()` => Create a followup message for an interaction
- `ReplyAsync()` => Send a message to the origin channel of the interaction
- `DeleteOriginalResponseAsync()` => Delete the original interaction response

## Commands

Valid **Interaction Commands** must comply with the following requirements:

|                               | return type                  | max parameter count | allowed parameter types       | attribute                |
|-------------------------------|------------------------------|---------------------|-------------------------------|--------------------------|
|[Slash Command](#slash-commands)| `Task`/`Task<RuntimeResult>` | 25                  | any*                           | `[SlashCommand]`         |
|[User Command](#user-commands)  | `Task`/`Task<RuntimeResult>` | 1                   | Implementations of `IUser`    | `[UserCommand]`          |
|[Message Command](#message-commands)| `Task`/`Task<RuntimeResult>` | 1                   | Implementations of `IMessage` | `[MessageCommand]`       |
|[Component Interaction Command](#component-interaction-commands)| `Task`/`Task<RuntimeResult>` | -                 | `string` or `string[]`        | `[ComponentInteraction]` |

**a `TypeConverter` that is capable of parsing type in question must be registered to the `InteractionService` instance.*

> You should avoid using long running code in your command module. Depending on your setup, long running code may block the Gateway thread of your bot, interrupting its connection to Discord.

### Slash Commands

Slash Commands are created using the `[SlashCommandAttribute]`. Every Slash Command must declare a name and a description. You can check Discords **Application Command Naming Guidelines** [here](https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-naming).

```csharp
[SlashCommand("echo", "Echo an input")]
public async Task Echo(string input)
{
    await RespondAsync(input);
}
```

#### Parameters

Slash Commands can have up to 25 method parameters. You must name your parameters in accordance with [Discords Naming Guidelines](https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-naming). By default, your methods can feature the following parameter types:

- Implementations of `IUser`
- Implementations of `IChannel`*
- Implementations of `IRole`
- Implementations of `IMentionable`
- `string`
- `float`, `double`, `decimal`
- `bool`
- `char`
- `sbyte`, `byte`
- `int16`, `int32`, `int64`
- `uint16`, `uint32`, `uint64`
- `enum` (Values are registered as multiple choice options and are enforced by Discord)
- `DateTime`
- `TimeSpan`

---

**You can use more specialized implementations of `IChannel` to restrict the allowed channel types for a channel type option.*
| interface           | Channel Type                  |
|---------------------|-------------------------------|
| `IStageChannel`     | Stage Channels                |
| `IVoiceChannel`     | Voice Channels                |
| `IDMChannel`        | DM Channels                   |
| `IGroupChannel`     | Group Channels                |
| `ICategory Channel` | Category Channels             |
| `INewsChannel`      | News Channels                 |
| `IThreadChannel`    | Public, Private, News Threads |
| `ITextChannel`      | Text Channels                 |

---

##### Optional Parameters

Parameters with default values (ie. `int count = 0`) will be displayed as optional parameters on Discord Client.

##### Parameter Summary

By using the `[SummaryAttribute]` you can customize the displayed name and description of a parameter

```csharp
[Summary(description: "this is a parameter description")] string input
```

##### Parameter Choices

`[ChoiceAttribute]` can be used to add choices to a parameter.

```csharp
[SlashCommand("blep", "Send a random adorable animal photo")]
public async Task Blep([Choice("Dog","dog"), Choice("Cat", "cat"), Choice("Penguin", "penguin")] string animal)
{
    ...
}
```

In most cases, instead of relying on this attribute, you should use an `Enum` to create multiple choice parameters. Ex.

```csharp
public enum Animal
{
    Cat,
    Dog,
    Penguin
}

[SlashCommand("blep", "Send a random adorable animal photo")]
public async Task Blep(Animal animal)
{
    ...
}
```

This Slash Command will be displayed exactly the same as the previous example.

##### Channel Types

Channel types for an `IChannel` parameter can also be restricted using the `[ChannelTypesAttribute]`.

```csharp
[SlashCommand("name", "Description")]
public async Task Command([ChannelTypes(ChannelType.Stage, ChannelType.Text)]IChannel channel)
{
    ...
}
```

In this case, user can only input Stage Channels and Text Channels to this parameter.

### User Commands

A valid User Command must have the following structure:

```csharp
[UserCommand("Say Hello")]
public async Task SayHello(IUser user)
{
    ...
}
```

User commands can only have one parameter and its type must be an implementation of `IUser`.

### Message Commands

A valid Message Command must have the following structure:

```csharp
[UserCommand("Bookmark")]
public async Task Bookmark(IUser user)
{
    ...
}
```

Message commands can only have one parameter and its type must be an implementation of `IMessage`.

### Component Interaction Commands

Component Interaction Commands are used to handle interactions that originate from **Discord Message Component**s. This pattern is particularly useful if you will be reusing a set a **Custom ID**s.

```csharp
[ComponentInteraction("custom_id")]
public async Task RoleSelection()
{
    ...
}
```

Component Interaction Commands support wild card matching, by default `*` character can be used to create a wild card pattern. Interaction Service will use lazy matching to capture the words corresponding to the wild card character. And the captured words will be passed on to the command method in the same order they were captured.

*Ex.*

If Interaction Service recieves a component interaction with **player:play,rickroll** custom id, `op` will be *play* and `name` will be *rickroll*

```csharp
[ComponentInteraction("player:*,*")]
public async Task Play(string op, string name)
{
    ...
}
```

You may use as many wild card characters as you want.

#### Select Menus

Unlike button interactions, select menu interactions also contain the values of the selected menu items. In this case, you should structure your method to accept a string array.

```csharp
[ComponentInteraction("role_selection")]
public async Task RoleSelection(string[] selectedRoles)
{
    ...
}
```

 Wild card pattern can also be used to match select menu custom ids but remember that the array containing the select menu values should be the last parameter.

```csharp
[ComponentInteraction("role_selection_*")]
public async Task RoleSelection(string id, string[] selectedRoles)
{
    ...
}
```

## Command Context

Every command module provides its commands with an execution context. This context property includes general information about the underlying interaction that triggered the command execution. The base command context.

You can design your modules to work with different implementation types of `IInteractionCommandContext`. To achieve this, make sure your module classes inherit from the generic variant of the `InteractionModuleBase`.

> Context type must be consistent throughout the project, or you will run into issues during runtime.

## Loading Modules

