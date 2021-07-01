using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a <see cref="IMessageComponent"/> Selection Dropdown.
    /// </summary>
    public class SelectComponent : IMessageComponent
    {
        /// <inheritdoc/>
        public ComponentType Type { get; } = ComponentType.Select;

        /// <summary>
        ///     A unique id that will be sent with a <see cref="IDiscordInteraction"/>. This is how you know which selection dropdown was used.
        /// </summary>
        public string CustomId { get; }
        
        /// <summary>
        ///     Custom placeholder text if nothing is selected, max 100 characters
        /// </summary>
        public string Placeholder { get; }
        
        /// <summary>
        ///     The choices in the selection dropdown, max 25
        /// </summary>
        public List<SelectComponentOption> Options { get; }
        
        /// <summary>
        ///     The minimum number of items that must be chosen; default 1, min 0, max 25
        /// </summary>
        public int MinValues { get; }
        
        /// <summary>
        ///     The maximum number of items that can be chosen; default 1, max 25
        /// </summary>
        public string MaxValues { get; }

        internal SelectComponent(string customId)
        {
	        this.CustomId = customId;
        }


    }
}
