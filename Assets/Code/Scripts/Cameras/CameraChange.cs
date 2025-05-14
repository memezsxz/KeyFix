using UnityEngine;

/// <summary>
/// Handles entering and exiting camera trigger zones, switching active cameras accordingly.
/// </summary>
public class CameraChange : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the camera GameObject found as a child of this GameObject.
    /// This camera will be activated or deactivated when the player enters or exits the trigger zone.
    /// </summary>
    private GameObject areaCam;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Look for a child Camera component, even if it's disabled, and get its GameObject
        areaCam = GetComponentInChildren<Camera>(true)?.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player enters the trigger zone, notify the CameraSwitchManager to activate this camera
        if (other.CompareTag("Player"))
        {
            CameraSwitchManager.Instance?.EnterCameraZone(areaCam);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player exits the trigger zone, notify the CameraSwitchManager to deactivate this camera
        if (other.CompareTag("Player"))
        {
            CameraSwitchManager.Instance?.ExitCameraZone(areaCam);
        }
    }

    #endregion
}