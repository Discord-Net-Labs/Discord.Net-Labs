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
        ///     Gets the modal's custom id.
        /// </summary>
        string CustomId { get; }
        
        /// <summary>
        ///     Gets the modal's title.
        /// </summary>
        string Title { get; }
    }
}
