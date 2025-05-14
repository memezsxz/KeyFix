using UnityEngine;

/// <summary>
/// Continuously plays an audio clip during every frame update of an Animator state.
/// Use with caution as this plays every frame unless limited by custom logic or volume threshold.
/// </summary>
public class PlayOnStateUpdate : StateMachineBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    [Tooltip("Optional volume override (-1 to use default).")]
    [SerializeField, Range(-1f, 1f)] private float _volume = -1f;

    /// <summary>
    /// Gets the effective playback volume. Returns 0.001f if explicitly set to 0.
    /// </summary>
    public float Volume => _volume == 0f ? 0.001f : _volume;

    /// <summary>
    /// Called every frame while the state is playing.
    /// Plays the clip continuously. Consider using logic to prevent spamming.
    /// </summary>
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_volume > 0)
            SoundManager.Instance.PlaySound(_audioClip, Volume);
        else
            SoundManager.Instance.PlaySound(_audioClip);
    }
}