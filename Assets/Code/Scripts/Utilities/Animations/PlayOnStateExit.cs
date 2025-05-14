using UnityEngine;

/// <summary>
/// Plays an audio clip when an Animator state exits.
/// Useful for triggering sounds at the end of an animation, such as footsteps or closing effects.
/// </summary>
public class PlayOnStateExit : StateMachineBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    [Tooltip("Volume override for the sound (range -1 to 1). -1 means use default volume.")]
    [SerializeField, Range(-1f, 1f)] private float _volume = -1f;

    /// <summary>
    /// Gets the effective playback volume. Ensures volume is never exactly zero.
    /// </summary>
    public float Volume => _volume == 0f ? 0.001f : _volume;

    /// <summary>
    /// Called by Unity when the animator exits the state this behaviour is attached to.
    /// Plays the assigned audio clip via the SoundManager.
    /// </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.Instance.PlaySound(_audioClip, Volume);
    }
}