using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    
    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.SetDirectAudioVolume(0, SoundManager.Instance.MusicVolume);
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        print("handling game victory");
        GameManager.Instance.HandleGameVictory();
    }
}