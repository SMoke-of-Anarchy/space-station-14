# Russian localization patches

These patches adapt the historical Russian fork changes to the current code, rather than restoring old server systems wholesale.

1. Character-name filtering retains Cyrillic, including Ё/ё, while preserving the existing Latin/digit/punctuation restrictions.
2. Russian is the primary content culture, English is an explicitly loaded fallback. Shared Fluent helpers are registered for both bundles. English `MANY`/`MAKEPLURAL` are registered only for English; Russian duration messages use a four-category Fluent term instead. Number formatting uses the bundle culture and does not truncate significant integer zeroes at zero decimal places.
3. All 222 localized datasets are checked against the actual Russian Fluent IDs. Fourteen counts were updated, including names, adjectives, verbs, AI names and quartermaster figurines. Obsolete combined death-commando datasets were not restored.
4. Added 26 chat shorthand entries, their missing clap/snap translations, and 81 Russian emote triggers across 27 emotes. The correct spelling «аплодирует» is accepted alongside the historical misspelling. Triggers are unique, and English triggers are retained.
5. Russian radio keycodes and chat prefixes are shared by server/client: LOOC `_`, emotes `%` (or `*`), automatic departmental radio `:р`. The common radio prefix `;` is unchanged. The radio guide lists the matching Russian keys.
6. Eight speech accents support Cyrillic in their modern Shared systems. Entity-bound randomness still uses `PredictedRandom`; the old accidental Unicode ranges in the stutter regex were replaced by an explicit consonant list. Lisp probability is per matching character, not one random choice for the whole message. Cyrillic upper/lower case is retained where appropriate.
7. The research console opens at `760 550`; its minimum size remains `625 400`.

## Checks available without the game

Install Python dependencies from `requirements.txt` in a virtual environment, and have Node available for real `Intl.PluralRules("ru")` categories:

```sh
python Tools/localization/test_ru_patches.py
```

On the development workspace, Fluent is in the parent `.venv` and PyYAML is provided by the system Python:

```sh
PYTHONPATH=/usr/lib/python3/dist-packages ../.venv/bin/python Tools/localization/test_ru_patches.py
git diff --check
```

These are **static/resource checks, not C# execution or Robust runtime tests**. They parse Russian Fluent, check duplicate IDs, inspect dataset coverage and YAML emote/radio data, check XML/XAML, test the simple character-class patterns, and guard the Shared prediction paths. The small regex/text models do not replace the C# tests.

## Required .NET recheck

The project requires the SDK declared in `global.json` (.NET 10). NUnit regressions were added for:

- `RestrictedNameTest`;
- `RussianLocalizationInitializationTest` (mocked manager: registration/load order, reuse of bundles, numeric callbacks and English plural callbacks);
- `RussianChatSanitizationTest`;
- `RussianChatPrefixTest`;
- `RussianAccentTest` (including deterministic random-seed checks).

Run from the repository root:

```sh
dotnet test Content.Tests/Content.Tests.csproj --filter 'FullyQualifiedName~RestrictedNameTest|FullyQualifiedName~RussianLocalizationInitializationTest|FullyQualifiedName~RussianChatSanitizationTest|FullyQualifiedName~RussianChatPrefixTest|FullyQualifiedName~RussianAccentTest'
dotnet test Content.IntegrationTests/Content.IntegrationTests.csproj --filter 'FullyQualifiedName~LocalizedDatasetPrototypeTest'
```

**The editing environment had no `dotnet` executable. These NUnit tests, compilation, and game integration were NOT run there.** In particular, mocked initialization tests alone cannot prove that a real missing Russian message renders successfully from the English fallback.

Before deploying, boot both client and server and verify: a Russian message and an English-only fallback message (including a common formatting helper), names containing Ё/ё, fractional/zero durations, department/default radio routing with actual headset permissions, Cyrillic emotes, prediction/replay of accented speech, and the research console layout at different resolutions. Changing radio codes/prefixes changes player input conventions; use the updated guide.
