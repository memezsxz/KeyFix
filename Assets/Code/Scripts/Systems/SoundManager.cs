using UnityEngine;
using UnityEngine.Audio;

/// <summary>
///     Manages sound and music playback, volumes, and persistence.
/// </summary>
public class SoundManager : Singleton<SoundManager>, IDataPersistence
{
    #region Fields & Properties

    [Header("Audio Sources")] [SerializeField]
    private AudioSource musicAudioSource;

    [SerializeField] private AudioSource sfxAudioSource;

    public AudioSource MusicAudioSource => musicAudioSource;
    public AudioSource SFXAudioSource => sfxAudioSource;

    [Header("Audio Mixer Groups")] [SerializeField]
    private AudioMixerGroup SFXGroup;

    [SerializeField] private AudioMixerGroup MusicGroup;

    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Levels")] [SerializeField] [Range(0f, 1f)]
    private float musicVolume = 1f;

    [SerializeField] [Range(0f, 1f)] private float soundVolume = 1f;

    public float MusicVolume => musicVolume;
    public float SoundVolume => soundVolume;
    public bool IsMusicPlaying => MusicAudioSource.isPlaying;
    public bool IsSoundPlaying => SFXAudioSource.isPlaying;

    #endregion

    #region Constants

    private const string MusicPref = "MusicVolume";
    private const string SfxPref = "SFXVolume";

    #endregion

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        // Assign mixer groups
        musicAudioSource.outputAudioMixerGroup = MusicGroup;
        sfxAudioSource.outputAudioMixerGroup = SFXGroup;
    }

    private void Start()
    {
        // Add debug commands for adjusting volume via debug console
        DebugController.Instance?.AddDebugCommand(new DebugCommand(
            "set_music_vol",
            "Changes the music volume",
            "set_music_vol <float>",
            args =>
            {
                if (float.TryParse(args[0], out var vol))
                    SetMusicVolume(vol);
            }));

        DebugController.Instance?.AddDebugCommand(new DebugCommand(
            "set_sound_vol",
            "Changes the sound effect volume",
            "set_sound_vol <float>",
            args =>
            {
                if (float.TryParse(args[0], out var vol))
                    SetSoundVolume(vol);
            }));
    }

    #endregion

    #region Playback Methods

    /// <summary>Plays a sound effect clip through the SFX audio source.</summary>
    /// <param name="clip">The AudioClip to play.</param>
    /// <param name="volumeMultiplier">Optional scale volume (0.001–1), otherwise uses current SFX volume.</param>
    public void PlaySound(AudioClip clip, float volumeMultiplier = -1f)
    {
        if (clip == null) return;

        var finalVolume = volumeMultiplier < 0.001f || volumeMultiplier > 1f ? MusicVolume : volumeMultiplier;

        volumeMultiplier = Mathf.Clamp01(finalVolume);

        sfxAudioSource.PlayOneShot(clip, volumeMultiplier);
    }

    /// <summary>Plays a music clip through the music audio source.</summary>
    /// <param name="clip">The AudioClip to play.</param>
    /// <param name="volumeMultiplier">Optional scale volume (0.001–1), otherwise uses current music volume.</param>
    public void PlayMusic(AudioClip clip, float volumeMultiplier = -1f)
    {
        if (clip == null) return;

        musicAudioSource.clip = clip;
        var finalVolume = volumeMultiplier < 0.001f || volumeMultiplier > 1f ? MusicVolume : volumeMultiplier;

        volumeMultiplier = Mathf.Clamp01(finalVolume);

        if (!musicAudioSource.isPlaying)
            musicAudioSource.Play();
    }

    /// <summary>Stops the music clip currently playing</summary>
    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    /// <summary>Stops the music clip currently playing</summary>
    public void StopSound()
    {
        sfxAudioSource.Stop();
    }

    public void StopAllAudio()
    {
        if (IsMusicPlaying) StopMusic();
        if (IsSoundPlaying) StopSound();
    }

    #endregion

    #region Volume Control

    /// <summary>Sets the global sound effect volume and updates the mixer.</summary>
    /// <param name="volume">Volume between 0 and 1.</param>
    public void SetSoundVolume(float volume)
    {
        if (volume is < 0 or > 1) return;

        soundVolume = volume;
        var decibels = ConvertVolumeToDecibels(volume);
        audioMixer.SetFloat(SfxPref, decibels);
    }

    /// <summary>Sets the global music volume and updates the mixer.</summary>
    /// <param name="volume">Volume between 0 and 1.</param>
    public void SetMusicVolume(float volume)
    {
        if (volume is < 0 or > 1) return;

        musicVolume = volume;
        var decibels = ConvertVolumeToDecibels(volume);
        audioMixer.SetFloat(MusicPref, decibels);
    }

    /// <summary>Converts a linear volume (0–1) to decibels for mixer use.</summary>
    /// <param name="volume">Linear volume.</param>
    /// <returns>Volume in decibels.</returns>
    private float ConvertVolumeToDecibels(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
    }

    #endregion

    #region IDataPersistence Implementation

    public void SaveData(ref SaveData data)
    {
        data.Sounds.SoundVolume = soundVolume;
        data.Sounds.MusicVolume = musicVolume;
    }

    public void LoadData(ref SaveData data)
    {
        SetSoundVolume(data.Sounds.SoundVolume);
        SetMusicVolume(data.Sounds.MusicVolume);
    }

    #endregion
}