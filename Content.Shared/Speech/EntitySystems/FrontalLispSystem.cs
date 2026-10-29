using System.Text.RegularExpressions;
using Content.Shared.Random.Helpers;
using Content.Shared.Speech.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Speech.EntitySystems;

public sealed partial class FrontalLispSystem : RelayAccentSystem<FrontalLispComponent>
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IRobustRandom _random = default!;

    private static readonly Regex RegexRussianLisp = new(
        @"[сСчЧцЦзЗ]|\B[тТ](?![АЕЁИОУЫЭЮЯаеёиоуыэюя])", RegexOptions.Compiled);
    // @formatter:off
    private static readonly Regex RegexUpperTh = new("[T]+[Ss]+|[S]+[Cc]+(?=[IiEeYy]+)|[C]+(?=[IiEeYy]+)|[P][Ss]+|([S]+[Tt]+|[T]+)(?=[Ii]+[Oo]+[Uu]*[Nn]*)|[C]+[Hh]+(?=[Ii]*[Ee]*)|[Z]+|[S]+|[X]+(?=[Ee]+)");
    private static readonly Regex RegexLowerTh = new("[t]+[s]+|[s]+[c]+(?=[iey]+)|[c]+(?=[iey]+)|[p][s]+|([s]+[t]+|[t]+)(?=[i]+[o]+[u]*[n]*)|[c]+[h]+(?=[i]*[e]*)|[z]+|[s]+|[x]+(?=[e]+)");
    private static readonly Regex RegexUpperEcks = new("[E]+[Xx]+[Cc]*|[X]+");
    private static readonly Regex RegexLowerEcks = new("[e]+[x]+[c]*|[x]+");
    // @formatter:on

    public override string Accentuate(string message, Entity<FrontalLispComponent>? ent = null)
    {
        // handles ts, sc(i|e|y), c(i|e|y), ps, st(io(u|n)), ch(i|e), z, s
        message = RegexUpperTh.Replace(message, "TH");
        message = RegexLowerTh.Replace(message, "th");
        // handles ex(c), x
        message = RegexUpperEcks.Replace(message, "EKTH");
        message = RegexLowerEcks.Replace(message, "ekth");

        var random = ent.HasValue
            ? SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent.Value))
            : _random;
        return RegexRussianLisp.Replace(message, match =>
        {
            if (!random.Prob(0.90f))
                return match.Value;

            return match.Value[0] switch
            {
                'с' or 'ч' => "ш",
                'С' or 'Ч' => "Ш",
                'ц' or 'т' => "ч",
                'Ц' or 'Т' => "Ч",
                'з' => "ж",
                'З' => "Ж",
                _ => match.Value,
            };
        });
    }
}
