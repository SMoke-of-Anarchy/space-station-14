using System.Text.RegularExpressions;
using Content.Shared.Random.Helpers;
using Content.Shared.Speech.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Speech.EntitySystems;

public sealed partial class MothAccentSystem : RelayAccentSystem<MothAccentComponent>
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IRobustRandom _random = default!;

    private static readonly Regex RegexRussianBuzz = new("ж+|Ж+|з+|З+", RegexOptions.Compiled);
    private static readonly Regex RegexLowerBuzz = new("z{1,3}");
    private static readonly Regex RegexUpperBuzz = new("Z{1,3}");

    public override string Accentuate(string message, Entity<MothAccentComponent>? ent = null)
    {
        // buzzz
        message = RegexLowerBuzz.Replace(message, "zzz");
        // buZZZ
        message = RegexUpperBuzz.Replace(message, "ZZZ");

        var random = ent.HasValue
            ? SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent.Value))
            : _random;
        return RegexRussianBuzz.Replace(message, match => new string(match.Value[0], random.Next(2, 4)));
    }
}
