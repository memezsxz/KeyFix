using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simulates wind that applies force to the player if detected in front of the blower.
/// Wind strength is reduced based on player scale and movement direction.
/// </summary>
public class WindBlower : MonoBehaviour
{
    /// <summary>
    /// The direction the wind is blowing in local space.
    /// </summary>
    [SerializeField] private Vector3 windDirection = Vector3.forward;

    /// <summary>
    /// The base strength of the wind before scaling factors are applied.
    /// </summary>
    [SerializeField] private float baseWindStrength = 10f;

    /// <summary>
    /// The maximum range (in world units) for wind detection.
    /// </summary>
    [SerializeField] private float detectionRange = 10f;

    /// <summary>
    /// Minimum percentage of base wind strength applied at max resistance.
    /// </summary>
    [SerializeField] private float minimumWindResistance = 0.3f;

    /// <summary>
    /// Sound clip to play when wind hits the player.
    /// </summary>
    [SerializeField] private AudioClip sound;

    /// <summary>
    /// Cached reference to the player's movement script.
    /// </summary>
    private PlayerMovement playerMovement;

    /// <summary>
    /// Cached reference to the player's transform.
    /// </summary>
    private Transform playerTransform;

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerMovement == null || playerTransform == null) return;

        var center = transform.position;
        var halfExtents = transform.localScale * 0.5f;
        var rotation = transform.rotation;

        // BoxCast simulates the blower's shape and checks for player in front
        RaycastHit hit;
        if (Physics.BoxCast(center, halfExtents, windDirection, out hit, rotation, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Compute resistance based on player size
                float scaleFactor = playerTransform.localScale.magnitude / Mathf.Sqrt(3f);
                float effectiveWindStrength = baseWindStrength;

                if (scaleFactor > 1f)
                {
                    effectiveWindStrength /= scaleFactor * 2f;
                    effectiveWindStrength = Mathf.Max(effectiveWindStrength, baseWindStrength * minimumWindResistance);
                }

                // Reduce wind effect if the player is moving against the wind
                var inputActions = playerTransform.GetComponent<PlayerInput>().actions;
                var moveValue = inputActions.FindAction("Move").ReadValue<Vector2>();

                if (moveValue != Vector2.zero && scaleFactor > 1f)
                {
                    var moveInput = new Vector3(moveValue.x, 0f, moveValue.y).normalized;
                    float againstWind = Vector3.Dot(moveInput, windDirection.normalized);

                    if (againstWind < 0f) effectiveWindStrength *= 0.5f;
                }

                // Apply wind force and play sound
                var force = windDirection.normalized * effectiveWindStrength;
                playerMovement.ApplyExternalForce(force);
                SoundManager.Instance.PlaySound(sound);
            }
        }
    }
}