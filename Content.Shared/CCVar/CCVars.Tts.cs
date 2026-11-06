using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     Master switch for IC Text-to-Speech (TTS) generation on the server.
    /// </summary>
    public static readonly CVarDef<bool> TTSEnabled =
        CVarDef.Create("tts.enabled", false, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE, "Enables IC Text-to-Speech generation.");

    /// <summary>
    ///     URL of the external TTS HTTP endpoint (POST JSON -> audio/ogg).
    /// </summary>
    public static readonly CVarDef<string> TTSApiUrl =
        CVarDef.Create("tts.api_url", "", CVar.SERVERONLY | CVar.ARCHIVE, "URL of the external TTS server endpoint.");

    /// <summary>
    ///     Timeout in seconds for HTTP requests to the TTS server.
    /// </summary>
    public static readonly CVarDef<float> TTSApiTimeoutSeconds =
        CVarDef.Create("tts.api_timeout_seconds", 15.0f, CVar.SERVERONLY | CVar.ARCHIVE, "HTTP request timeout in seconds for TTS generation.");

    /// <summary>
    ///     Maximum character length of a chat message to synthesize via TTS.
    /// </summary>
    public static readonly CVarDef<int> TTSMaxChars =
        CVarDef.Create("tts.max_chars", 250, CVar.SERVERONLY | CVar.ARCHIVE, "Maximum number of characters per TTS message.");

    /// <summary>
    ///     Client-side volume multiplier for TTS playback (0.0 disables TTS on the client).
    /// </summary>
    public static readonly CVarDef<float> TTSVolume =
        CVarDef.Create("tts.volume", 1.0f, CVar.CLIENTONLY | CVar.ARCHIVE, "Client-side TTS volume multiplier.");
}
