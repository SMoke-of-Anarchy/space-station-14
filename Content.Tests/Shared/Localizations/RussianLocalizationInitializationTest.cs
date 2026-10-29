using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Content.Shared.Localizations;
using Moq;
using NUnit.Framework;
using Robust.Shared.Localization;

namespace Content.Tests.Shared.Localizations;

[TestFixture]
public sealed class RussianLocalizationInitializationTest
{
    private static ContentLocalizationManager CreateManager(
        out Mock<ILocalizationManager> loc,
        out Dictionary<(string Culture, string Name), LocFunction> functions)
    {
        var loaded = new HashSet<string>();
        var callbacks = new Dictionary<(string, string), LocFunction>();
        loc = new Mock<ILocalizationManager>(MockBehavior.Strict);
        loc.Setup(x => x.HasCulture(It.IsAny<CultureInfo>()))
            .Returns((CultureInfo c) => loaded.Contains(c.Name));
        loc.Setup(x => x.LoadCulture(It.IsAny<CultureInfo>()))
            .Callback((CultureInfo c) => Assert.That(loaded.Add(c.Name), Is.True));
        loc.Setup(x => x.AddFunction(It.IsAny<CultureInfo>(), It.IsAny<string>(), It.IsAny<LocFunction>()))
            .Callback((CultureInfo c, string name, LocFunction fn) =>
            {
                Assert.That(loaded.Contains(c.Name), Is.True, "Register only after loading the bundle");
                callbacks[(c.Name, name)] = fn;
            });
        loc.Setup(x => x.SetCulture(It.IsAny<CultureInfo>()))
            .Callback((CultureInfo c) => Assert.That(loaded.Contains(c.Name), Is.True));
        loc.Setup(x => x.SetFallbackCulture(It.IsAny<CultureInfo[]>()))
            .Callback((CultureInfo[] cultures) =>
            {
                foreach (var c in cultures)
                    Assert.That(loaded.Contains(c.Name), Is.True, "Fallback bundle must be loaded");
            });

        var manager = new ContentLocalizationManager();
        typeof(ContentLocalizationManager).GetField("_loc", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(manager, loc.Object);
        functions = callbacks;
        return manager;
    }

    [Test]
    public void InitializationSeparatesCulturesAndCanReuseLoadedBundles()
    {
        var manager = CreateManager(out var loc, out var functions);
        manager.Initialize();
        manager.Initialize();

        loc.Verify(x => x.LoadCulture(It.Is<CultureInfo>(c => c.Name == "ru-RU")), Times.Once());
        loc.Verify(x => x.LoadCulture(It.Is<CultureInfo>(c => c.Name == "en-US")), Times.Once());
        loc.Verify(x => x.SetCulture(It.Is<CultureInfo>(c => c.Name == "ru-RU")), Times.Exactly(2));
        loc.Verify(x => x.SetFallbackCulture(It.Is<CultureInfo[]>(c => c.Length == 1 && c[0].Name == "en-US")), Times.Exactly(2));
        foreach (var name in new[] { "PRESSURE", "POWERWATTS", "POWERJOULES", "ENERGYWATTHOURS", "UNITS", "TOSTRING", "LOC", "NATURALFIXED", "NATURALPERCENT", "PLAYTIME" })
        {
            Assert.That(functions.ContainsKey(("ru-RU", name)), Is.True);
            Assert.That(functions.ContainsKey(("en-US", name)), Is.True);
        }
        Assert.That(functions.ContainsKey(("ru-RU", "MANY")), Is.False);
        Assert.That(functions.ContainsKey(("ru-RU", "MAKEPLURAL")), Is.False);
        Assert.That(functions.ContainsKey(("en-US", "MANY")), Is.True);
        Assert.That(functions.ContainsKey(("en-US", "MAKEPLURAL")), Is.True);
    }

    [TestCase("ru-RU", "NATURALFIXED", 1.25, 3, "1,25")]
    [TestCase("en-US", "NATURALFIXED", 1.25, 3, "1.25")]
    [TestCase("ru-RU", "NATURALFIXED", 20, 0, "20")]
    [TestCase("en-US", "NATURALFIXED", 20, 0, "20")]
    [TestCase("ru-RU", "NATURALPERCENT", 1, 0, "100%")]
    [TestCase("en-US", "NATURALPERCENT", 0.125, 2, "12.5%")]
    public void NumericFunctionsUseTheirBundleCulture(string culture, string name, double number, int decimals, string expected)
    {
        var manager = CreateManager(out _, out var functions);
        manager.Initialize();
        var args = new LocArgs(new ILocValue[] { new LocValueNumber(number), new LocValueNumber(decimals) },
            new Dictionary<string, ILocValue>());
        Assert.That(functions[(culture, name)](args).Value, Is.EqualTo(expected));
    }

    [TestCase(1, "second")]
    [TestCase(2, "seconds")]
    public void EnglishPluralFunctionStillWorks(double count, string expected)
    {
        var manager = CreateManager(out _, out var functions);
        manager.Initialize();
        var args = new LocArgs(new ILocValue[] { new LocValueString("second"), new LocValueNumber(count) },
            new Dictionary<string, ILocValue>());
        Assert.That(functions[("en-US", "MANY")](args).Value, Is.EqualTo(expected));
    }
}
