using UnityEngine;

/// <summary>
/// Represents a timed toggle laser beam that damages the player when intersected.
/// Uses a LineRenderer to visually simulate the beam and raycasts for collision.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Lazar : MonoBehaviour
{
    /// <summary>
    /// The direction in which the laser fires from its origin.
    /// </summary>
    [SerializeField] private Vector3 fireDirection = Vector3.left;

    /// <summary>
    /// Minimum delay before the laser toggles its state (on/off).
    /// </summary>
    [SerializeField] [Range(0.5f, 5f)] private float minToggleTime = 0.5f;

    /// <summary>
    /// Optional sound to play when the laser toggles state (currently unused).
    /// </summary>
    [SerializeField] private AudioClip sound;

    /// <summary>
    /// Maximum delay before the laser toggles its state (on/off).
    /// </summary>
    [SerializeField] [Range(0.5f, 5f)] private float maxToggleTime = 1f;

    /// <summary>
    /// Whether the laser is currently active and visible.
    /// </summary>
    private bool isActive = true;

    /// <summary>
    /// LineRenderer component used to draw the laser beam.
    /// </summary>
    private LineRenderer lr;

    /// <summary>
    /// Time threshold at which the next toggle should happen.
    /// </summary>
    private float nextToggleTime;

    /// <summary>
    /// Timer tracking elapsed time since the last toggle.
    /// </summary>
    private float timer;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        SetNextToggleTime();
    }

    private void Update()
    {
        // Handle laser toggle timing
        timer += Time.deltaTime;
        if (timer >= nextToggleTime)
        {
            isActive = !isActive;
            lr.enabled = isActive;
            SetNextToggleTime();
            timer = 0f;

            // Uncomment if sound is needed on toggle:
            // SoundManager.Instance.PlaySound(sound);
        }

        // Skip raycasting and drawing if laser is inactive
        if (!isActive) return;

        // Set laser origin
        lr.SetPosition(0, transform.position);

        // Perform raycast to determine where laser hits
        RaycastHit hit;
        if (Physics.Raycast(transform.position, fireDirection, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hit Player");
                GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
            }
            else
            {
                // Stop at hit point
                lr.SetPosition(1, hit.point);
            }
        }
        else
        {
            // Extend laser to max distance if no hit
            lr.SetPosition(1, transform.position + fireDirection * 50);
        }
    }

    /// <summary>
    /// Randomizes the next toggle time between min and max thresholds.
    /// </summary>
    private void SetNextToggleTime()
    {
        nextToggleTime = Random.Range(minToggleTime, maxToggleTime);
    }
}