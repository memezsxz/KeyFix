using UnityEngine;

/// <summary>
/// Plays an audio clip once when an Animator state is entered.
/// Attach this to an Animator state via the Animator StateMachineBehaviours.
/// </summary>
public class PlayOnStateEnter : StateMachineBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    [Tooltip("Volume override for the sound (range -1 to 1). -1 means use default volume.")]
    [SerializeField, Range(-1f, 1f)]
    private float _volume = -1f;

    /// <summary>
    /// Gets the effective volume. If volume is set to zero, it returns a minimal value (0.001f).
    /// </summary>
    public float Volume => _volume == 0f ? 0.001f : _volume;

    /// <summary>
    /// Called automatically by Unity when the Animator enters a state.
    /// Plays the assigned audio clip via the SoundManager.
    /// </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.Instance.PlaySound(_audioClip, Volume);
    }
}