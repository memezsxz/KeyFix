using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton class that manages camera switching in the game.
/// Allows entering and exiting camera zones while keeping a stack-based record
/// of active cameras to support nested camera transitions (e.g. entering and exiting zones).
/// </summary>
public class CameraSwitchManager : Singleton<CameraSwitchManager>
{
    /// <summary>
    /// The currently active camera, or null if no camera is active.
    /// </summary>
    public GameObject ActiveCamera => _cameraStack.Count > 0 ? _cameraStack[^1] : null;

    /// <summary>
    /// Stack of active camera GameObjects.
    /// The top of the stack is the currently active camera.
    /// </summary>
    private readonly List<GameObject> _cameraStack = new();

    /// <summary>
    /// Enters a new camera zone.
    /// Deactivates the currently active camera (if any), activates the new one,
    /// and sets it as the current camera.
    /// </summary>
    /// <param name="cam">The camera GameObject to activate and push onto the stack.</param>
    public void EnterCameraZone(GameObject cam)
    {
        // Prevent duplicate entries in the camera stack
        if (!_cameraStack.Contains(cam))
        {
            // Deactivate the previous top camera
            if (_cameraStack.Count > 0)
                _cameraStack[^1].SetActive(false);

            // Push the new camera to the stack and activate it
            _cameraStack.Add(cam);
            cam.SetActive(true);

            // Set the Unity main camera reference to this new camera
            Camera.SetupCurrent(cam.GetComponent<Camera>());
        }
    }

    /// <summary>
    /// Exits a camera zone.
    /// Removes the specified camera from the stack, deactivates it,
    /// and reactivates the previous camera (if any).
    /// </summary>
    /// <param name="cam">The camera GameObject to deactivate and remove from the stack.</param>
    public void ExitCameraZone(GameObject cam)
    {
        // Remove the specified camera and deactivate it
        if (_cameraStack.Remove(cam))
        {
            cam.SetActive(false);

            // Reactivate the new top camera (if available)
            if (_cameraStack.Count > 0)
            {
                var newTop = _cameraStack[^1];
                newTop.SetActive(true);

                // Set the Unity current camera to the reactivated one
                Camera.SetupCurrent(newTop.GetComponent<Camera>());
            }
        }
    }
}
