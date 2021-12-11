---
uid: Guides.V2V3Guide
title: V2 -> V3 Guide
---

# V2 to V3 Guide

V3 is designed to be a more feature complete, more reliable, and more flexible library than any previous version.

- GuildMemberUpdated event (user 1 cached)
- ReactionAdded event (channel cached)
- UserIsTyping event (user & channel cached)

### GuildMemberUpdated Event

The guild member updated event now passes a `Cacheable<SocketGuildUser, RestGuildUser, IGuildUser, ulong>` for the first argument instead of a normal `SocketGuildUser`. This new cacheable type allows you to download a `RestGuildUser` if the user isn't cached.

### ReactionAdded Event

The reaction added event has been changed to have both parameters cacheable. This allows you to download the channel and message if they aren't cached instead of them being null.

### UserIsTyping Event

THe user is typing event has been changed to have both parameters cacheable. This allows you to download the user and channel if they aren't cached instead of them being null.

## Migrating your commands to slash command

The new InteractionService was designed to act like the previous service for text-based commands. Your pre-existing code will continue to work, but you will need to migrate your modules and response functions to use the new InteractionService methods. Docs on this can be found in the Guides section.
