using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Handles video playback and triggers a victory event when the video finishes.
/// </summary>
public class PlayVideo : MonoBehaviour
{
    /// <summary>
    /// Reference to the VideoPlayer component playing the cutscene or outro.
    /// </summary>
    public VideoPlayer videoPlayer;

    private void Start()
    {
        // Trigger OnVideoFinished when the video reaches its end
        videoPlayer.loopPointReached += OnVideoFinished;

        // Set audio volume using current music settings
        videoPlayer.SetDirectAudioVolume(0, SoundManager.Instance.MusicVolume);

        // Start video playback
        videoPlayer.Play();
    }

    /// <summary>
    /// Called when the video finishes playing.
    /// Transitions to game victory state.
    /// </summary>
    private void OnVideoFinished(VideoPlayer vp)
    {
        GameManager.Instance.HandleGameVictory();
    }
}