# Text Input
Text input components are a type of message component that can only 
be used in modals. There can be one text input component per action 
row.

## Creating Text Input 
Text input components can be built using the `TextInputBuilder`.
The simplest text input can build with:
```cs
var tb = new TextInputBuilder()
    .WithLabel("My Label")
    .WithCustomId("text_input");
```
and would produce a component looking like:

![](images/image5.png)

Specifying more values lets you control the look of your components
and enforce length requirements. Setting the `MinValue` property to 0
will make an input optional and setting both `MinValue` and `MaxValue`
to the same number will require the input to be exactly that many 
characters long. The valid `TextInputStyle`s are `Short` (the default
style) and `Paragraph`. A more complex text entry box might look like:
```cs
var tb = new TextInputBuilder()
    .WithLabel("Labeled")
    .WithCustomId("text_input")
    .WithPlaceholder("Consider this place held.")
    .WithStyle(TextInputStyle.Paragraph)
    .WithMinLength(6)
    .WithMaxLength(42);
```
and be displayed in Discord as:

![](images/image6.png)
