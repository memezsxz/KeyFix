using UnityEngine;
using UnityEngine.Audio;

/// <summary>
///     Manages sound and music playback, volumes, and persistence.
/// </summary>
public class SoundManager : Singleton<SoundManager>, IDataPersistence
{
    #region Fields & Properties

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    /// <summary>Audio source used for music playback.</summary>
    public AudioSource MusicAudioSource => musicAudioSource;

    /// <summary>Audio source used for sound effects.</summary>
    public AudioSource SFXAudioSource => sfxAudioSource;

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup SFXGroup;
    [SerializeField] private AudioMixerGroup MusicGroup;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Levels")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float soundVolume = 1f;

    /// <summary>Current user-defined music volume (0–1).</summary>
    public float MusicVolume => musicVolume;

    /// <summary>Current user-defined sound effect volume (0–1).</summary>
    public float SoundVolume => soundVolume;

    /// <summary>Whether music is currently playing.</summary>
    public bool IsMusicPlaying => MusicAudioSource.isPlaying;

    /// <summary>Whether a sound effect is currently playing.</summary>
    public bool IsSoundPlaying => SFXAudioSource.isPlaying;

    #endregion

    #region Constants

    private const string MusicPref = "MusicVolume";
    private const string SfxPref = "SFXVolume";

    #endregion

    #region Unity Events

    /// <summary>
    /// Assigns mixer groups to audio sources.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        musicAudioSource.outputAudioMixerGroup = MusicGroup;
        sfxAudioSource.outputAudioMixerGroup = SFXGroup;
    }

    /// <summary>
    /// Registers debug commands for volume adjustment via console.
    /// </summary>
    private void Start()
    {
        DebugController.Instance?.AddDebugCommand(new DebugCommand(
            "set_music_vol", "Changes the music volume", "set_music_vol <float>",
            args =>
            {
                if (float.TryParse(args[0], out var vol))
                    SetMusicVolume(vol);
            }));

        DebugController.Instance?.AddDebugCommand(new DebugCommand(
            "set_sound_vol", "Changes the sound effect volume", "set_sound_vol <float>",
            args =>
            {
                if (float.TryParse(args[0], out var vol))
                    SetSoundVolume(vol);
            }));
    }

    #endregion

    #region Playback Methods

    /// <summary>
    /// Plays a one-shot sound effect.
    /// </summary>
    /// <param name="clip">Sound clip to play.</param>
    /// <param name="volumeMultiplier">Optional override volume (0.001–1).</param>
    public void PlaySound(AudioClip clip, float volumeMultiplier = -1f)
    {
        if (clip == null) return;

        var finalVolume = volumeMultiplier < 0.001f || volumeMultiplier > 1f
            ? MusicVolume
            : volumeMultiplier;

        volumeMultiplier = Mathf.Clamp01(finalVolume);
        sfxAudioSource.PlayOneShot(clip, volumeMultiplier);
    }

    /// <summary>
    /// Plays music through the music source.
    /// </summary>
    /// <param name="clip">Music track to play.</param>
    /// <param name="volumeMultiplier">Optional override volume (0.001–1).</param>
    public void PlayMusic(AudioClip clip, float volumeMultiplier = -1f)
    {
        if (clip == null) return;

        musicAudioSource.clip = clip;

        var finalVolume = volumeMultiplier < 0.001f || volumeMultiplier > 1f
            ? MusicVolume
            : volumeMultiplier;

        volumeMultiplier = Mathf.Clamp01(finalVolume);

        if (!musicAudioSource.isPlaying)
            musicAudioSource.Play();
    }

    /// <summary>
    /// Stops any currently playing music.
    /// </summary>
    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    /// <summary>
    /// Stops any currently playing sound effects.
    /// </summary>
    public void StopSound()
    {
        sfxAudioSource.Stop();
    }

    /// <summary>
    /// Stops all audio including music and sound effects.
    /// </summary>
    public void StopAllAudio()
    {
        if (IsMusicPlaying) StopMusic();
        if (IsSoundPlaying) StopSound();
    }

    #endregion

    #region Volume Control

    /// <summary>
    /// Sets the sound effect volume and applies it to the mixer.
    /// </summary>
    /// <param name="volume">Volume from 0 to 1.</param>
    public void SetSoundVolume(float volume)
    {
        if (volume is < 0 or > 1) return;

        soundVolume = volume;
        var decibels = ConvertVolumeToDecibels(volume);
        audioMixer.SetFloat(SfxPref, decibels);
    }

    /// <summary>
    /// Sets the music volume and applies it to the mixer.
    /// </summary>
    /// <param name="volume">Volume from 0 to 1.</param>
    public void SetMusicVolume(float volume)
    {
        if (volume is < 0 or > 1) return;

        musicVolume = volume;
        var decibels = ConvertVolumeToDecibels(volume);
        audioMixer.SetFloat(MusicPref, decibels);
    }

    /// <summary>
    /// Converts a linear volume scale (0–1) into decibel scale used by mixers.
    /// </summary>
    /// <param name="volume">Linear volume value.</param>
    /// <returns>Volume in decibels.</returns>
    private float ConvertVolumeToDecibels(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
    }

    #endregion

    #region IDataPersistence Implementation

    /// <summary>
    /// Saves current sound and music volumes to the save data.
    /// </summary>
    public void SaveData(ref SaveData data)
    {
        data.Sounds.SoundVolume = soundVolume;
        data.Sounds.MusicVolume = musicVolume;
    }

    /// <summary>
    /// Loads and applies sound and music volumes from the save data.
    /// </summary>
    public void LoadData(ref SaveData data)
    {
        SetSoundVolume(data.Sounds.SoundVolume);
        SetMusicVolume(data.Sounds.MusicVolume);
    }

    #endregion
}
