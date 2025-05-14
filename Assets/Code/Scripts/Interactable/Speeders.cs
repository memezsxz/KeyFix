using System.Collections;
using UnityEngine;

/// <summary>
/// Pushes the player forward when they enter the trigger zone from the correct direction.
/// Plays a short animation and changes material to indicate activation.
/// </summary>
public class Speeders : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Reference to the model object used for scale and visuals.
    /// </summary>
    [SerializeField] private GameObject model;

    /// <summary>
    /// Target end position for the pusher object (for layout reference only).
    /// </summary>
    [SerializeField] private Transform endposition;

    /// <summary>
    /// Material to apply when the pusher is actively pushing.
    /// </summary>
    [SerializeField] private Material activeMaterial;

    /// <summary>
    /// Material to apply when the pusher is idle.
    /// </summary>
    [SerializeField] private Material inactiveMaterial;

    /// <summary>
    /// MeshRenderer to update material visuals.
    /// </summary>
    [SerializeField] private MeshRenderer meshRenderer;

    #endregion

    #region Private Fields

    /// <summary>
    /// Cached end position based on the endposition transform.
    /// </summary>
    private Vector3 endpoint;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Store end position and apply model's scale to root GameObject
        endpoint = endposition.position;
        gameObject.transform.localScale = model.transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<SpeedersInteraction>();
        if (player == null) return;

        // Check the entry direction
        var toPlayer = (player.transform.position - transform.position).normalized;
        var alignment = Vector3.Dot(transform.forward, toPlayer);

        if (alignment > 0.3f)
        {
            // Entered from behind (wrong direction), apply minimal nudge
            var minimalPushTarget = player.transform.position + transform.forward * 0.4f;
            player.TeleportTo(minimalPushTarget);
        }
        else
        {
            // Correct direction: push forward along the object's forward vector
            meshRenderer.material = activeMaterial;

            var pushTarget = player.transform.position + transform.forward * model.transform.localScale.z;
            player.MoveTowards(pushTarget);

            StartCoroutine(DelayedReset()); // Reset material after a short delay
        }
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Waits briefly before resetting the material to its inactive state.
    /// </summary>
    private IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(0.3f); // tiny delay
        meshRenderer.material = inactiveMaterial;
    }

    /// <summary>
    /// Optionally wait for the player to finish being pushed before resetting.
    /// Not currently used.
    /// </summary>
    private IEnumerator WaitUntilPushEnds(SpeedersInteraction mover)
    {
        while (mover != null && mover.IsBeingPushed)
            yield return null;

        meshRenderer.material = inactiveMaterial;
    }

    #endregion
}
