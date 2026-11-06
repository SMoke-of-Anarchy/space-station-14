using System.IO;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.TTS;
using Robust.Client.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;

namespace Content.Client.TTS;

public sealed partial class TTSSystem : EntitySystem
{
    [Dependency] private IAudioManager _audioManager = default!;
    [Dependency] private AudioSystem _audio = default!;
    [Dependency] private IConfigurationManager _cfg = default!;

    private readonly Dictionary<EntityUid, EntityUid> _activeEntityStreams = new();
    private float _volume = 1.0f;

    public override void Initialize()
    {
        base.Initialize();

        Subs.CVar(_cfg, CCVars.TTSVolume, v => _volume = Math.Clamp(v, 0f, 3f), true);
        SubscribeNetworkEvent<PlayTTSEvent>(OnPlayTTS);
    }

    private void OnPlayTTS(PlayTTSEvent ev)
    {
        if (_volume <= 0.01f || ev.AudioData.Length == 0)
            return;

        AudioStream stream;
        try
        {
            using var ms = new MemoryStream(ev.AudioData, writable: false);
            stream = _audioManager.LoadAudioOggVorbis(ms);
        }
        catch (Exception ex)
        {
            Log.Warning($"Failed to decode incoming TTS audio: {ex.Message}");
            return;
        }

        var volumeDb = SharedAudioSystem.GainToVolume(_volume);

        if (ev.IsRadio || ev.SourceUid == null)
        {
            var radioParams = AudioParams.Default.WithVolume(volumeDb);
            _audio.PlayGlobal(stream, null, radioParams);
            return;
        }

        if (!TryGetEntity(ev.SourceUid, out var sourceEntity) || !Exists(sourceEntity.Value))
            return;

        // Stop previous unfinished local speech from the same entity so it doesn't overlap with itself
        if (_activeEntityStreams.TryGetValue(sourceEntity.Value, out var prevAudioEnt) && Exists(prevAudioEnt))
        {
            _audio.Stop(prevAudioEnt);
        }

        var maxDistance = ev.IsWhisper ? SharedChatSystem.WhisperClearRange + 1f : SharedChatSystem.VoiceRange;
        var refDistance = ev.IsWhisper ? 1f : 2.5f;
        var entityParams = AudioParams.Default
            .WithVolume(ev.IsWhisper ? volumeDb - 4f : volumeDb)
            .WithMaxDistance(maxDistance)
            .WithReferenceDistance(refDistance);

        var playing = _audio.PlayEntity(stream, sourceEntity.Value, null, entityParams);
        if (playing != null)
        {
            _activeEntityStreams[sourceEntity.Value] = playing.Value.Entity;
        }
    }
}
