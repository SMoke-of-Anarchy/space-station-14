using System.Reflection;
using Content.Server.Chat.Managers;
using Moq;
using NUnit.Framework;
using Robust.Shared.Localization;

namespace Content.Tests.Server.Chat;

[TestFixture]
public sealed class RussianChatSanitizationTest
{
    [TestCase("хд", "", "chatsan-laughs")]
    [TestCase("Привет кек", "Привет", "chatsan-laughs")]
    [TestCase("о7", "", "chatsan-salutes")]
    [TestCase("Т_Т", "", "chatsan-cries")]
    [TestCase("Привет ))", "Привет", "chatsan-smiles-widely")]
    [TestCase("Привет ((", "Привет", "chatsan-frowns-deeply")]
    [TestCase("хд привет о7", "привет", "chatsan-salutes")]
    public void ConvertsRussianShorthandsAndSelectsTheLastEmote(string input, string expected, string key)
    {
        var loc = new Mock<ILocalizationManager>();
        loc.Setup(x => x.GetString(It.IsAny<string>(), It.IsAny<(string, object)>()))
            .Returns((string id, (string, object) argument) => id);
        var sanitizer = new ChatSanitizationManager();
        typeof(ChatSanitizationManager).GetField("_loc", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(sanitizer, loc.Object);
        typeof(ChatSanitizationManager).GetField("_doSanitize", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(sanitizer, true);

        Assert.That(sanitizer.TrySanitizeEmoteShorthands(input, default, out var text, out var emote), Is.True);
        Assert.That(text, Is.EqualTo(expected));
        Assert.That(emote, Is.EqualTo(key));
    }

    [TestCase("кексы")]
    [TestCase("Привет))")]
    public void DoesNotTreatPartsOfWordsAsShorthands(string input)
    {
        var sanitizer = new ChatSanitizationManager();
        typeof(ChatSanitizationManager).GetField("_doSanitize", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(sanitizer, true);
        Assert.That(sanitizer.TrySanitizeEmoteShorthands(input, default, out var text, out var emote), Is.False);
        Assert.That(text, Is.EqualTo(input));
        Assert.That(emote, Is.Null);
    }

    [Test]
    public void SanitizerCanStillBeDisabled()
    {
        var sanitizer = new ChatSanitizationManager();
        Assert.That(sanitizer.TrySanitizeEmoteShorthands("хд", default, out var text, out _), Is.False);
        Assert.That(text, Is.EqualTo("хд"));
    }
}
