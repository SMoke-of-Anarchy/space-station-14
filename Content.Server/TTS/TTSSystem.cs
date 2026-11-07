using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Humanoid;
using Content.Shared.Radio.Components;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Silicons.Laws.Components;
using Content.Shared.Speech;
using Content.Shared.TTS;
using Content.Shared.VendingMachines.Components;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.TTS;

public sealed partial class TTSSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = default!;

    private readonly HttpClient _httpClient = new(new SocketsHttpHandler
    {
        UseProxy = false
    });
    private readonly ConcurrentDictionary<string, byte[]> _audioCache = new();
    private readonly ConcurrentDictionary<string, Task<byte[]?>> _inFlight = new();
    private readonly ConcurrentQueue<(PlayTTSEvent Event, List<ICommonSession> Recipients)> _pendingEvents = new();

    private const int MaxCacheEntries = 512;
    private static readonly string[] MaleSpeakers = ["aidar", "eugene"];
    private static readonly string[] FemaleSpeakers = ["baya", "kseniya", "xenia"];
    private static readonly Regex LatinWordRegex = new(@"[a-zA-Z]+", RegexOptions.Compiled);
    private static readonly Regex DigitRunRegex = new(@"\d+", RegexOptions.Compiled);

    private bool _enabled;
    private string _apiUrl = string.Empty;
    private float _timeoutSeconds = 15.0f;
    private int _maxChars = 250;
    private DateTime _backoffUntilUtc = DateTime.MinValue;

    public override void Initialize()
    {
        base.Initialize();

        Subs.CVar(_cfg, CCVars.TTSEnabled, v => _enabled = v, true);
        Subs.CVar(_cfg, CCVars.TTSApiUrl, v =>
        {
            _apiUrl = v;
            _backoffUntilUtc = DateTime.MinValue;
        }, true);
        Subs.CVar(_cfg, CCVars.TTSApiTimeoutSeconds, v => _timeoutSeconds = Math.Clamp(v, 1f, 30f), true);
        Subs.CVar(_cfg, CCVars.TTSMaxChars, v => _maxChars = Math.Clamp(v, 10, 500), true);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        while (_pendingEvents.TryDequeue(out var item))
        {
            foreach (var session in item.Recipients)
            {
                if (session.Status == Robust.Shared.Enums.SessionStatus.InGame)
                    RaiseNetworkEvent(item.Event, session.Channel);
            }
        }
    }

    public void RequestSpeakTTS(
        EntityUid source,
        string message,
        IReadOnlyList<ICommonSession> recipients,
        bool isWhisper = false)
    {
        if (!_enabled || recipients.Count == 0 || string.IsNullOrWhiteSpace(_apiUrl))
            return;

        if (DateTime.UtcNow < _backoffUntilUtc)
            return;

        var cleanText = NormalizeTextForRussianTTS(FormattedMessage.RemoveMarkupPermissive(message));
        if (string.IsNullOrWhiteSpace(cleanText) || !ContainsCyrillic(cleanText))
            return;

        if (cleanText.Length > _maxChars)
            cleanText = cleanText[.._maxChars];

        var (speaker, pitch, speed, effect, seed, species) = ResolveVoiceParameters(source, isRadio: false);
        var netSource = GetNetEntity(source);
        var sessions = new List<ICommonSession>(recipients);

        _ = DispatchTTSAsync(cleanText, speaker, pitch, speed, effect, seed, species, netSource, isWhisper, isRadio: false, sessions);
    }

    public void RequestRadioTTS(
        EntityUid messageSource,
        string message,
        ICommonSession recipient)
    {
        if (!_enabled || string.IsNullOrWhiteSpace(_apiUrl))
            return;

        if (DateTime.UtcNow < _backoffUntilUtc)
            return;

        var cleanText = NormalizeTextForRussianTTS(FormattedMessage.RemoveMarkupPermissive(message));
        if (string.IsNullOrWhiteSpace(cleanText) || !ContainsCyrillic(cleanText))
            return;

        if (cleanText.Length > _maxChars)
            cleanText = cleanText[.._maxChars];

        var (speaker, pitch, speed, _, seed, species) = ResolveVoiceParameters(messageSource, isRadio: true);

        _ = DispatchTTSAsync(cleanText, speaker, pitch, speed, "radio", seed, species, null, isWhisper: false, isRadio: true, [recipient]);
    }

    public void RequestAnnouncementTTS(string message, Filter filter, string? sender = null)
    {
        if (!_enabled || string.IsNullOrWhiteSpace(_apiUrl))
            return;

        if (DateTime.UtcNow < _backoffUntilUtc)
            return;

        var sessions = new List<ICommonSession>();
        foreach (var session in filter.Recipients)
        {
            sessions.Add(session);
        }

        if (sessions.Count == 0)
            return;

        var cleanText = NormalizeTextForRussianTTS(FormattedMessage.RemoveMarkupPermissive(message));
        if (string.IsNullOrWhiteSpace(cleanText) || !ContainsCyrillic(cleanText))
            return;

        var maxAnnounceChars = Math.Max(_maxChars, 400);
        if (cleanText.Length > maxAnnounceChars)
            cleanText = cleanText[..maxAnnounceChars];

        var senderKey = string.IsNullOrWhiteSpace(sender) ? "CentCom" : sender;
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(senderKey));
        var announceSpeakers = new[] { "xenia", "eugene", "kseniya", "aidar" };
        var speaker = announceSpeakers[hashBytes[0] % announceSpeakers.Length];

        _ = DispatchTTSAsync(cleanText, speaker, 0, 1.0f, "announce", hashBytes[0], "Human", null, isWhisper: false, isRadio: true, sessions);
    }

    private (string Speaker, int Pitch, float Speed, string Effect, int Seed, string Species) ResolveVoiceParameters(EntityUid uid, bool isRadio)
    {
        var nameEv = new TransformSpeakerNameEvent(uid, MetaData(uid).EntityName);
        RaiseLocalEvent(uid, nameEv);
        var voiceName = string.IsNullOrWhiteSpace(nameEv.VoiceName) ? MetaData(uid).EntityName : nameEv.VoiceName;

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(voiceName));
        var h0 = hashBytes[0];
        var h1 = hashBytes[1];
        var h2 = hashBytes[2];

        var isFemale = false;
        var speciesId = "Human";
        var pitchOffset = 0;
        var speedOffset = 0f;

        if (TryComp<HumanoidProfileComponent>(uid, out var profile))
        {
            isFemale = profile.Sex == Sex.Female ||
                       (profile.Sex == Sex.Unsexed && profile.Gender == Gender.Female);

            if (profile.Age >= 55)
            {
                pitchOffset -= 1;
                speedOffset -= 0.05f;
            }

            speciesId = profile.Species.Id;
            if (speciesId == "Dwarf")
                pitchOffset -= 3;
            else if (speciesId is "Moth" or "Arachnid")
                pitchOffset += 2;
            else if (speciesId is "Reptilian" or "Vox")
                pitchOffset -= 2;
        }
        else if (TryComp<GrammarComponent>(uid, out var grammar))
        {
            isFemale = grammar.Gender == Gender.Female;
        }

        var speakerPool = isFemale ? FemaleSpeakers : MaleSpeakers;
        var speaker = speakerPool[h0 % speakerPool.Length];

        // Wide per-character pitch range (server.py preserves formants via rubberband/pyworld)
        int pitch;
        if (isFemale)
        {
            var basePitch = (h1 % 9) - 4; // -4 .. +4
            pitch = Math.Clamp(basePitch + pitchOffset, -5, 5);
        }
        else
        {
            var basePitch = (h1 % 9) - 5; // -5 .. +3
            pitch = Math.Clamp(basePitch + pitchOffset, -6, 3);
        }

        var baseSpeed = 0.88f + (h2 / 255f) * 0.24f;
        var speed = (float)Math.Round(Math.Clamp(baseSpeed + speedOffset, 0.84f, 1.18f), 2);

        string effect;
        if (isRadio || HasComp<RadioSpeakerComponent>(uid))
        {
            effect = "radio";
        }
        else if (HasComp<VendingMachineComponent>(uid))
        {
            effect = "vendor";
        }
        else if (HasComp<BorgChassisComponent>(uid) ||
                 HasComp<SiliconLawBoundComponent>(uid) ||
                 (TryComp<SpeechComponent>(uid, out var speech) && speech.SpeechVerb == "Robotic"))
        {
            effect = "borg";
        }
        else
        {
            effect = "normal";
        }

        var seed = h0 | (h1 << 8);
        return (speaker, pitch, speed, effect, seed, speciesId);
    }

    private async Task DispatchTTSAsync(
        string text,
        string speaker,
        int pitch,
        float speed,
        string effect,
        int seed,
        string species,
        NetEntity? sourceUid,
        bool isWhisper,
        bool isRadio,
        List<ICommonSession> recipients)
    {
        var cacheKey = $"{speaker}:{species}:{seed}:{pitch}:{speed:F2}:{effect}:{text}";

        if (!_audioCache.TryGetValue(cacheKey, out var audioBytes))
        {
            var task = _inFlight.GetOrAdd(cacheKey, _ => FetchTTSBytesAsync(text, speaker, pitch, speed, effect, seed, species));
            try
            {
                audioBytes = await task;
            }
            finally
            {
                _inFlight.TryRemove(cacheKey, out _);
            }

            if (audioBytes is { Length: > 0 })
            {
                if (_audioCache.Count >= MaxCacheEntries)
                    _audioCache.Clear();

                _audioCache[cacheKey] = audioBytes;
            }
        }

        if (audioBytes is not { Length: > 0 })
            return;

        var ev = new PlayTTSEvent(audioBytes, sourceUid, isWhisper, isRadio);
        _pendingEvents.Enqueue((ev, recipients));
    }

    private async Task<byte[]?> FetchTTSBytesAsync(string text, string speaker, int pitch, float speed, string effect, int seed, string species)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));
            var payload = new TTSApiRequest
            {
                Text = text,
                Speaker = speaker,
                Pitch = pitch,
                Speed = speed,
                Effect = effect,
                Seed = seed,
                Species = species
            };

            using var response = await _httpClient.PostAsJsonAsync(_apiUrl, payload, cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsByteArrayAsync(cts.Token);
        }
        catch (Exception ex)
        {
            // Back off for 15 seconds when the TTS backend is offline so chat stays completely unaffected
            _backoffUntilUtc = DateTime.UtcNow.AddSeconds(15);
            Log.Debug($"TTS server at {_apiUrl} unreachable ({ex.Message}); backing off for 15s.");
            return null;
        }
    }

    private static readonly Dictionary<string, string> EnglishWordOverrides = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = "айди",
        ["pda"] = "кпк",
        ["ok"] = "окей",
        ["okay"] = "окей",
        ["pls"] = "плиз",
        ["plz"] = "плиз",
        ["please"] = "плиз",
        ["help"] = "хелп",
        ["lol"] = "лол",
        ["lmao"] = "лмао",
        ["afk"] = "афк",
        ["wtf"] = "втф",
        ["gg"] = "гэгэ",
        ["wp"] = "вэпэ",
        ["nt"] = "нанотрейзен",
        ["ert"] = "обр",
        ["eva"] = "ева",
        ["apc"] = "апц",
        ["ame"] = "аме",
        ["ai"] = "ии",
        ["hos"] = "гсб",
        ["hop"] = "гп",
        ["cmo"] = "гв",
        ["rd"] = "нр",
        ["ce"] = "си",
        ["qm"] = "км",
        ["sec"] = "сек",
        ["med"] = "мед",
        ["medbay"] = "медбэй",
        ["sci"] = "сай",
        ["cargo"] = "карго",
        ["brig"] = "бриг",
        ["airlock"] = "шлюз",
        ["hello"] = "хэллоу",
        ["hi"] = "хай",
        ["hey"] = "хэй",
        ["yes"] = "ес",
        ["no"] = "ноу",
        ["go"] = "гоу",
        ["stop"] = "стоп",
        ["bro"] = "бро",
        ["man"] = "мэн",
        ["what"] = "вот",
        ["why"] = "вай",
        ["who"] = "ху",
        ["how"] = "хау",
        ["you"] = "ю",
        ["fuck"] = "фак",
        ["shit"] = "шит",
        ["nice"] = "найс",
        ["cool"] = "кул",
        ["good"] = "гуд",
        ["bad"] = "бэд",
        ["dead"] = "дэд"
    };

    private static readonly (string Pattern, string Replacement)[] EnglishDigraphs =
    [
        ("tion", "шн"),
        ("sion", "жн"),
        ("tch", "тч"),
        ("sch", "ш"),
        ("sh", "ш"),
        ("ch", "ч"),
        ("th", "т"),
        ("ph", "ф"),
        ("gh", "г"),
        ("ck", "к"),
        ("qu", "кв"),
        ("oo", "у"),
        ("ee", "и"),
        ("ea", "и"),
        ("ai", "эй"),
        ("ay", "эй"),
        ("oi", "ой"),
        ("oy", "ой"),
        ("ou", "ау"),
        ("ow", "оу"),
        ("au", "о"),
        ("aw", "о"),
        ("ew", "ью"),
        ("ng", "нг"),
        ("wh", "в"),
        ("wr", "р"),
        ("kn", "н")
    ];

    private static string NormalizeTextForRussianTTS(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var text = input.Trim();

        // Transliterate Latin words into Russian phonetics so Silero v4_ru pronounces them
        text = LatinWordRegex.Replace(text, match =>
        {
            var word = match.Value;
            if (EnglishWordOverrides.TryGetValue(word, out var mapped))
                return mapped;

            var lower = word.ToLowerInvariant();
            foreach (var (pattern, rep) in EnglishDigraphs)
            {
                lower = lower.Replace(pattern, rep);
            }

            var sb = new StringBuilder(lower.Length * 2);
            foreach (var c in lower)
            {
                sb.Append(c switch
                {
                    'a' => "а",
                    'b' => "б",
                    'c' => "к",
                    'd' => "д",
                    'e' => "е",
                    'f' => "ф",
                    'g' => "г",
                    'h' => "х",
                    'i' => "и",
                    'j' => "дж",
                    'k' => "к",
                    'l' => "л",
                    'm' => "м",
                    'n' => "н",
                    'o' => "о",
                    'p' => "п",
                    'q' => "к",
                    'r' => "р",
                    's' => "с",
                    't' => "т",
                    'u' => "у",
                    'v' => "в",
                    'w' => "в",
                    'x' => "кс",
                    'y' => "й",
                    'z' => "з",
                    _ => c.ToString()
                });
            }

            return sb.ToString();
        });

        // Convert digits to Russian words so Silero v4_ru pronounces numbers
        text = DigitRunRegex.Replace(text, match =>
        {
            if (int.TryParse(match.Value, out var num) && num is >= 0 and <= 9999)
                return NumberToRussianWords(num);

            var sb = new StringBuilder();
            foreach (var d in match.Value)
            {
                if (sb.Length > 0)
                    sb.Append(' ');
                sb.Append(DigitToWord(d - '0'));
            }
            return sb.ToString();
        });

        return text;
    }

    private static string DigitToWord(int d) => d switch
    {
        0 => "ноль",
        1 => "один",
        2 => "два",
        3 => "три",
        4 => "четыре",
        5 => "пять",
        6 => "шесть",
        7 => "семь",
        8 => "восемь",
        9 => "девять",
        _ => ""
    };

    private static string NumberToRussianWords(int n)
    {
        if (n == 0)
            return "ноль";

        string[] ones = ["", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять"];
        string[] teens = ["десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"];
        string[] tens = ["", "", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто"];
        string[] hundreds = ["", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот"];
        string[] thousands = ["", "одна тысяча", "две тысячи", "три тысячи", "четыре тысячи", "пять тысяч", "шесть тысяч", "семь тысяч", "восемь тысяч", "девять тысяч"];

        var parts = new List<string>(4);
        if (n >= 1000)
        {
            parts.Add(thousands[n / 1000]);
            n %= 1000;
        }
        if (n >= 100)
        {
            parts.Add(hundreds[n / 100]);
            n %= 100;
        }
        if (n >= 20)
        {
            parts.Add(tens[n / 10]);
            n %= 10;
        }
        else if (n >= 10)
        {
            parts.Add(teens[n - 10]);
            n = 0;
        }
        if (n > 0)
        {
            parts.Add(ones[n]);
        }

        return string.Join(' ', parts);
    }

    private static bool ContainsCyrillic(string text)
    {
        foreach (var c in text)
        {
            if (c is (>= 'а' and <= 'я') or (>= 'А' and <= 'Я') or 'ё' or 'Ё')
                return true;
        }

        return false;
    }

    private sealed class TTSApiRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("speaker")]
        public string Speaker { get; set; } = "aidar";

        [JsonPropertyName("pitch")]
        public int Pitch { get; set; }

        [JsonPropertyName("speed")]
        public float Speed { get; set; } = 1.0f;

        [JsonPropertyName("effect")]
        public string Effect { get; set; } = "normal";

        [JsonPropertyName("seed")]
        public int Seed { get; set; }

        [JsonPropertyName("species")]
        public string Species { get; set; } = "Human";
    }
}
