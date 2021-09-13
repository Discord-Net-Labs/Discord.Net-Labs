You can change settings of DiscordSocketConfig by doing this in your Program.cs:
```csharp
public async Task MainAsync() {
    var client = new DiscordSocketClient(new DiscordSocketConfig() {
        MessageCacheSize = 0, // If you want to use cache messages then you would have to change this.
    });
}
```
For a more detailed explanation of the rest of the options please have a look at: [DiscordSocketConfig.cs](https://github.com/Discord-Net-Labs/Discord.Net-Labs/blob/release/3.x/src/Discord.Net.WebSocket/DiscordSocketConfig.cs)
