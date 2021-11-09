using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Extensions
{
    public static class EmbedFieldBuilderExtensions
    {
        public static List<EmbedFieldBuilder> WithField(this List<EmbedFieldBuilder> EmbedFieldBuilders, string Name, string Value, bool IsInline = true)
        {
            EmbedFieldBuilders.Add(new EmbedFieldBuilder() { IsInline = IsInline, Name = Name, Value = Value });
            return EmbedFieldBuilders;
        }
        public static List<EmbedFieldBuilder> WithFields(this List<EmbedFieldBuilder> List, List<EmbedFieldBuilder> EmbedFieldBuilders)
        {
            List.AddRange(EmbedFieldBuilders);
            return List;
        }
    }
}
