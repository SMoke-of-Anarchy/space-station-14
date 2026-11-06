using Robust.Shared.Serialization;

namespace Content.Shared.TTS;

/// <summary>
///     Sent from server to clients to play synthesized IC speech audio (OGG Vorbis).
/// </summary>
[Serializable, NetSerializable]
public sealed class PlayTTSEvent : EntityEventArgs
{
    public byte[] AudioData { get; }
    public NetEntity? SourceUid { get; }
    public bool IsWhisper { get; }
    public bool IsRadio { get; }

    public PlayTTSEvent(byte[] audioData, NetEntity? sourceUid, bool isWhisper = false, bool isRadio = false)
    {
        AudioData = audioData;
        SourceUid = sourceUid;
        IsWhisper = isWhisper;
        IsRadio = isRadio;
    }
}
