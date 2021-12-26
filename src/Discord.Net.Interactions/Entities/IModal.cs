using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public interface IModal
    {
        /// <summary>
        ///     Gets the modals custom id.
        /// </summary>
        string CustomId { get; }
        
        /// <summary>
        ///     Gets the modals title.
        /// </summary>
        string Title { get; }
    }
}
