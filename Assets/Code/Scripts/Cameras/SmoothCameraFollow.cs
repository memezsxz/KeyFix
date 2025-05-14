using UnityEngine;

/// <summary>
/// Smoothly follows a target transform (e.g., a player) while maintaining a constant offset.
/// Uses Vector3.SmoothDamp for smooth motion over time.
/// </summary>
public class SmoothCameraFollow : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// The time it takes for the camera to catch up to the target position.
    /// Higher values result in slower, smoother motion.
    /// </summary>
    [SerializeField] private float smoothTime;

    /// <summary>
    /// The target the camera will follow, typically the player.
    /// </summary>
    [SerializeField] private Transform target;

    #endregion

    #region Private Fields

    /// <summary>
    /// The initial positional offset between the camera and the target.
    /// </summary>
    private Vector3 _offset;

    /// <summary>
    /// Velocity used internally by SmoothDamp to track movement smoothing over time.
    /// </summary>
    private Vector3 _velocity;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Capture the starting offset from the target to the camera
        _offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        // Compute the target position with offset
        var targetPosition = target.position + _offset;

        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
    }

    #endregion
}