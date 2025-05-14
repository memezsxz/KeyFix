using UnityEngine;

/// <summary>
/// Handles camera switching when the player collides with or enters a trigger in a hallway.
/// Disables the current camera and enables the assigned virtual camera.
/// </summary>
public class HallwaysCameraScript : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Reference to the virtual camera to activate when triggered.
    /// </summary>
    [SerializeField] private Camera vcCamera;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Optional initialization
        // Debug.Log("HallwaysCameraScript started!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Camera view on collided!");

        // If the player hits this object physically, change camera view
        if (collision.gameObject.CompareTag("Player"))
            ChangeCameraView();
    }

    private void OnTriggerEnter(Collider other) 
    {
        // Debug.Log("Camera view on triggered!");

        // If the player enters the trigger zone, change camera view
        if (other.CompareTag("Player"))
            ChangeCameraView();
    }

    #endregion

    #region Camera Control

    /// <summary>
    /// Disables this object and activates the assigned virtual camera.
    /// </summary>
    private void ChangeCameraView()
    {
        gameObject.SetActive(false);
        vcCamera.gameObject.SetActive(true);
        // Debug.Log("Camera view changed!");
    }

    #endregion
}