using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Discord
{
    public class EmoteTests
    {
        [Fact]
        public void Test_Emote_Parse()
        {
            Assert.True(CustomEmoji.TryParse("<:typingstatus:394207658351263745>", out CustomEmoji emote));
            Assert.NotNull(emote);
            Assert.Equal("typingstatus", emote.Name);
            Assert.Equal(394207658351263745UL, emote.Id);
            Assert.False(emote.Animated);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1514056829775), emote.CreatedAt);
            Assert.EndsWith("png", emote.Url);
        }
        [Fact]
        public void Test_Invalid_Emote_Parse()
        {
            Assert.False(CustomEmoji.TryParse("invalid", out _));
            Assert.False(CustomEmoji.TryParse("<:typingstatus:not_a_number>", out _));
            Assert.Throws<ArgumentException>(() => CustomEmoji.Parse("invalid"));
        }
        [Fact]
        public void Test_Animated_Emote_Parse()
        {
            Assert.True(CustomEmoji.TryParse("<a:typingstatus:394207658351263745>", out CustomEmoji emote));
            Assert.NotNull(emote);
            Assert.Equal("typingstatus", emote.Name);
            Assert.Equal(394207658351263745UL, emote.Id);
            Assert.True(emote.Animated);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1514056829775), emote.CreatedAt);
            Assert.EndsWith("gif", emote.Url);
        }
        [Fact]
        public void Test_Invalid_Amimated_Emote_Parse()
        {
            Assert.False(CustomEmoji.TryParse("<x:typingstatus:394207658351263745>", out _));
            Assert.False(CustomEmoji.TryParse("<a:typingstatus>", out _));
            Assert.False(CustomEmoji.TryParse("<a:typingstatus:not_a_number>", out _));
        }
    }
}
