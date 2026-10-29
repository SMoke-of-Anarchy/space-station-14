using System.Reflection;
using System.Text.RegularExpressions;
using Content.Shared.Random.Helpers;
using Content.Shared.Speech.EntitySystems;
using Moq;
using NUnit.Framework;
using Robust.Shared.GameObjects;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Tests.Shared.Localizations;

[TestFixture]
public sealed class RussianAccentTest
{
    private static T Create<T>() where T : EntitySystem, new()
    {
        var random = new Mock<IRobustRandom>();
        random.Setup(x => x.Next(It.IsAny<int>(), It.IsAny<int>()))
            .Returns((int minimum, int maximum) => minimum);
        var system = new T();
        typeof(T).GetField("_random", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(system, random.Object);
        return system;
    }

    [Test]
    public void Bark()
    {
        var system = Create<BarkAccentSystem>();
        Assert.That(system.Accentuate("Лапа"), Is.EqualTo("Рапа"));
        Assert.That(system.Accentuate("угу!"), Is.EqualTo("вуф Гав!"));
    }

    [Test]
    public void OwO()
    {
        var system = Create<OwOAccentSystem>();
        Assert.That(system.Accentuate("ты Лара"), Is.EqualTo("ти Вава"));
        Assert.That(system.Accentuate("you really"), Is.EqualTo("wu weawwy"));
    }

    [Test]
    public void Lisp()
    {
        var system = Create<FrontalLispSystem>();
        Assert.That(system.Accentuate("с С ч Ч ц Ц з З атк АтК"), Is.EqualTo("ш Ш ш Ш ч Ч ж Ж ачк АчК"));
        Assert.That(system.Accentuate("аеёиоуыэюя АЕЁИОУЫЭЮЯ"), Is.EqualTo("аеёиоуыэюя АЕЁИОУЫЭЮЯ"));
    }

    [Test]
    public void Lizard()
    {
        var system = Create<LizardAccentSystem>();
        Assert.That(system.Accentuate("с з ш ч С З Ш Ч"), Is.EqualTo("сс сс шш щщ СС СС ШШ ЩЩ"));
        Assert.That(system.Accentuate("snake"), Is.EqualTo("sssnake"));
    }

    [Test]
    public void Moth()
    {
        var system = Create<MothAccentSystem>();
        Assert.That(system.Accentuate("ж з Ж З"), Is.EqualTo("жж зз ЖЖ ЗЗ"));
        Assert.That(system.Accentuate("buzz"), Is.EqualTo("buzzz"));
    }

    [Test]
    public void Monkey()
    {
        Assert.That(Create<MonkeyAccentSystem>().Accentuate("abcd"), Is.EqualTo("АААА!"));
    }

    [TestCase('о', "а")]
    [TestCase('К', "КХ")]
    [TestCase('щ', "шч")]
    [TestCase('Ц', "ТС")]
    [TestCase('o', "u")]
    [TestCase('s', "ch")]
    public void SlurredCharacterRules(char input, string expected)
    {
        var method = typeof(SlurredSystem).GetMethod("SlurCharacter", BindingFlags.Static | BindingFlags.NonPublic)!;
        Assert.That(method.Invoke(null, new object[] { input }), Is.EqualTo(expected));
    }

    [Test]
    public void StutteringIncludesOnlyIntendedConsonants()
    {
        var regex = (Regex) typeof(StutteringSystem)
            .GetField("Stutter", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        foreach (var c in "бвгджзйклмнпрстфхцчшщБВГДЖЗЙКЛМНПРСТФХЦЧШЩbcdfghjklmnpqrstvwxyz")
            Assert.That(regex.IsMatch(c.ToString()), Is.True, c.ToString());
        foreach (var c in "аеёиоуыэюяАЕЁИОУЫЭЮЯъьЪЬaeiou0123 _-αé™")
            Assert.That(regex.IsMatch(c.ToString()), Is.False, c.ToString());
    }

    [Test]
    public void PredictedRandomIsRepeatableForTheSameEntityAndTick()
    {
        var timing = new Mock<IGameTiming>();
        var first = SharedRandomExtensions.PredictedRandom(timing.Object, new NetEntity(42));
        var second = SharedRandomExtensions.PredictedRandom(timing.Object, new NetEntity(42));
        for (var i = 0; i < 16; i++)
            Assert.That(first.Next(), Is.EqualTo(second.Next()));
    }
}
