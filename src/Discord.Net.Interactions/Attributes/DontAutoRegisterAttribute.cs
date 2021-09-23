using System;

namespace Discord.Interactions
{
    /// <summary>
    /// <see cref="InteractionModuleBase{T}"/>s with this attribute will not be registered by the <see cref="InteractionService.RegisterCommandsGloballyAsync(bool)"/> or
    /// <see cref="InteractionService.RegisterCommandsToGuildAsync(ulong, bool)"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DontAutoRegisterAttribute : Attribute
    {
    }
}
