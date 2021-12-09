# Modals
Modals are a special type of dialog box you can send when responding
to Message Components and Application Commands. Modals are currently
the only way to get text input with components.

![](images/image1.png)

## Creating Modals

Lets create a simple modal with an entry field for users to
tell us their favorite food. We can start by creating a slash
command that will respond with the modal.
```cs
[SlashCommand("food", "Tell us about your favorite food!")]
public async Task FoodPreference()
{
    // send a modal
}
```

Now that we have our command setup we need to build a modal.
We can use the aptly named `ModalBuilder` for that:

| Method          | Description                               |
| --------------- | ----------------------------------------- |
| `WithTitle`     | Sets the modals title.                    |
| `WithCustomId`  | Sets the modals custom id.                |
| `AddTextInput`  | Adds a `TextInputBuilder` to the modal.   |
| `AddComponents` | Adds multiple components to the modal.    |
| `Build`         | Builds the `ModalBuilder` into a `Modal`. |

We know we need to add a text input to the modal, so let's look at that
method's parameters.

| Parameter     | Description                                |
| ------------- | ------------------------------------------ |
| `label`       | Sets the inputs label.                     |
| `customId`    | Sets the inputs custom id.                 |
| `style`       | Sets the inputs style.                     |
| `placeholder` | Sets the inputs placeholder.               |
| `minLength`   | Sets the minimum input length.             |
| `maxLength`   | Sets the maximum input length.             |
| `required`    | Sets whether or not the modal is required. |

To make a basic text input we would only need to set the `label` and
`customId`, but in this example we will also use the `placeholder` 
parameter. Now that we understand the methods involved let's build our
modal:

```cs
var mb = new ModalBuilder()
	.WithTitle("Fav Food")
	.WithCustomId("food_menu")
	.AddTextInput("What??", "food_name", placeholder:"Pizza")
	.AddTextInput("Why??", "food_reason", TextInputStyle.Paragraph, 
        "Kus it's so tasty");
```

And respond to the command with it:

```cs
await Context.Interaction.RespondWithModalAsync(mb.Build());
```

When we run the command we should see the modal we built:

![](images/image3.png)

## Responding to modals

Responding to modals works similarly to other interactions, with the 
caveat that you cannot call `RespondWithModalAsync`. Modals trigger 
the `ModalSubmitted` event on your socket or sharded client. In this 
case we want to get the user's favorite food from the modal, and notify
everyone of their preference. We can hook this code to our
`ModalSubmitted` event to respond to the modal in the example:

```cs
// Get the values of components.
List<SocketMessageComponentData> components =
    modal.Data.Components.ToList();
string food = components
    .Where(x => x.CustomId == "food_name").First().Value;
string reason = components
    .Where(x => x.CustomId == "food_reason").First().Value;

// Build the message to send.
string message = "hey @everyone; I just learned " + 
    $"{modal.User.Mention}'s favorite food is " +
    $"{food} because {reason}.";

// Specify the AllowedMentions so we don't actually ping everyone.
AllowedMentions mentions = new AllowedMentions();
mentions.AllowedTypes = AllowedMentionTypes.Users;

// Respond to the modal.
await modal.RespondAsync(message, allowedMentions:mentions);
```

Now responding to the modal should inform everyone of our tasty 
choices.

![](images/image4.png)