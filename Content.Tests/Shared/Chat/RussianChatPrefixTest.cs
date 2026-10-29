using Content.Shared.Chat;
using NUnit.Framework;

namespace Content.Tests.Shared.Chat;

[TestFixture]
public sealed class RussianChatPrefixTest
{
    [Test]
    public void RussianPrefixesKeepCommonRadioAndAlternateEmotes()
    {
        Assert.Multiple(() =>
        {
            Assert.That(SharedChatSystem.LOOCPrefix, Is.EqualTo('_'));
            Assert.That(SharedChatSystem.EmotesPrefix, Is.EqualTo('%'));
            Assert.That(SharedChatSystem.EmotesAltPrefix, Is.EqualTo('*'));
            Assert.That(SharedChatSystem.DefaultChannelPrefix, Is.EqualTo(":р"));
            Assert.That(SharedChatSystem.RadioCommonPrefix, Is.EqualTo(';'));
            Assert.That(SharedChatSystem.RadioChannelPrefix, Is.EqualTo(':'));
            Assert.That(SharedChatSystem.RadioChannelAltPrefix, Is.EqualTo('.'));
        });
    }
}
