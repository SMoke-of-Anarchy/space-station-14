using System.Reflection;
using System.Text.RegularExpressions;
using Content.Shared.Preferences;
using NUnit.Framework;

namespace Content.Tests.Shared.Preferences;

[TestFixture]
public sealed class RestrictedNameTest
{
    // Exercise the exact filter used by EnsureValid, without booting a game or
    // duplicating the character class in the test.
    private static Regex Filter => (Regex) typeof(HumanoidCharacterProfile)
        .GetField("RestrictedNameRegex", BindingFlags.Static | BindingFlags.NonPublic)!
        .GetValue(null)!;

    [TestCase("Иван Петров", "Иван Петров")]
    [TestCase("Ёжик ёжик", "Ёжик ёжик")]
    [TestCase("Анна-Мария O'Brien 42", "Анна-Мария O'Brien 42")]
    [TestCase("John Smith", "John Smith")]
    [TestCase("Иван_Петров<script>!", "ИванПетровscript")]
    [TestCase("Иван\nПетров\t", "ИванПетров")]
    [TestCase("山田 Иван", " Иван")]
    public void FilterPreservesRussianAndExistingAllowedCharacters(string input, string expected)
    {
        Assert.That(Filter.Replace(input, string.Empty), Is.EqualTo(expected));
    }
}
