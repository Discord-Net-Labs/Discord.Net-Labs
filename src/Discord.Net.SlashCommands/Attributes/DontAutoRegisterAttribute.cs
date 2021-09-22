using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// <see cref="SlashModuleBase{T}"/>s with this attribute will not be registered by the <see cref="SlashCommandService.RegisterCommandsGloballyAsync(bool)"/> or
    /// <see cref="SlashCommandService.RegisterCommandsToGuildAsync(ulong, bool)"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DontAutoRegisterAttribute : Attribute
    {
    }
}
