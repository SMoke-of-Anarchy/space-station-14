using System.Text.RegularExpressions;
using Content.Shared.Random.Helpers;
using Content.Shared.Speech.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Speech.EntitySystems;

public sealed partial class LizardAccentSystem : RelayAccentSystem<LizardAccentComponent>
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IRobustRandom _random = default!;

    private static readonly Regex RegexRussianHiss = new("с+|С+|з+|З+|ш+|Ш+|ч+|Ч+", RegexOptions.Compiled);
    private static readonly Regex RegexLowerS = new("s+");
    private static readonly Regex RegexUpperS = new("S+");
    private static readonly Regex RegexInternalX = new(@"(\w)x");
    private static readonly Regex RegexLowerEndX = new(@"\bx([\-|r|R]|\b)");
    private static readonly Regex RegexUpperEndX = new(@"\bX([\-|r|R]|\b)");

    public override string Accentuate(string message, Entity<LizardAccentComponent>? ent = null)
    {
        // hissss
        message = RegexLowerS.Replace(message, "sss");
        // hiSSS
        message = RegexUpperS.Replace(message, "SSS");
        // ekssit
        message = RegexInternalX.Replace(message, "$1kss");
        // ecks
        message = RegexLowerEndX.Replace(message, "ecks$1");
        // eckS
        message = RegexUpperEndX.Replace(message, "ECKS$1");

        var random = ent.HasValue
            ? SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent.Value))
            : _random;
        return RegexRussianHiss.Replace(message, match =>
        {
            var consonant = match.Value[0] switch
            {
                'з' => 'с',
                'З' => 'С',
                'ч' => 'щ',
                'Ч' => 'Щ',
                var other => other,
            };
            return new string(consonant, random.Next(2, 4));
        });
    }
}
