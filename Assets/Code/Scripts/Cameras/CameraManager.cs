using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages camera switching between multiple virtual cameras and a focus camera (lock-on).
/// Responds to player input to change perspective or focus on a target (e.g., enemy).
/// </summary>
public class CameraManager : Singleton<CameraManager>
{
    #region Serialized Fields

    /// <summary>
    /// The camera used when focusing (e.g., enemy lock-on mode).
    /// </summary>
    [SerializeField] private CinemachineVirtualCameraBase focusCamera;

    /// <summary>
    /// Reference to the player GameObject.
    /// </summary>
    [SerializeField] private GameObject Player;

    /// <summary>
    /// Reference to the player's eye camera (first-person perspective).
    /// </summary>
    [SerializeField] private GameObject PlayerEye;

    #endregion

    #region Private Fields

    // List of available non-focus Cinemachine virtual cameras.
    private List<CinemachineVirtualCameraBase> _cameras;

    // Input action for cycling through cameras.
    private InputAction _changeInputAction;

    // Index of the currently active camera in the _cameras list.
    private int _firstCameraIndex = -1;

    // Input action for toggling focus mode.
    private InputAction _lockInputAction;

    /// <summary>
    /// Indicates whether the focus camera is currently active.
    /// </summary>
    public bool IsFocusing { get; private set; }

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Get all virtual cameras under this object excluding FreeLook rigs
        _cameras = new List<CinemachineVirtualCameraBase>(
            gameObject.GetComponentsInChildren<CinemachineVirtualCameraBase>()
                .Where(cam =>
                    !(cam.transform.parent != null &&
                      cam.transform.parent.GetComponent<CinemachineFreeLook>() != null))
        );

        // Remove the focus camera from the switchable list
        _cameras.Remove(focusCamera);

        if (_cameras.Count == 0)
        {
            Debug.LogWarning("No virtual cameras found!");
            return;
        }

        // Get input actions from PlayerInput
        var input = GetComponent<PlayerInput>();
        _lockInputAction = input.actions["Lock"];
        _changeInputAction = input.actions["Change"];

        // Subscribe to input events
        _changeInputAction.started += OnChange;
        _lockInputAction.started += OnFocusStarted;
        _lockInputAction.canceled += OnFocusCanceled;

        // Assign player reference if not already done
        Player = GameObject.FindGameObjectWithTag("Player");

        // Set LookAt and Follow for each camera
        _cameras.ForEach(c =>
        {
            c.enabled = false;
            c.LookAt = Player.transform;
            c.Follow = Player.transform;
        });

        // Start with the first available camera
        EnableNextCamera();
    }

    private void OnDisable()
    {
        // Unsubscribe from input actions when disabled
        if (_changeInputAction != null)
            _changeInputAction.started -= OnChange;

        if (_lockInputAction != null)
        {
            _lockInputAction.started -= OnFocusStarted;
            _lockInputAction.canceled -= OnFocusCanceled;
        }
    }

    #endregion

    #region Camera Switching Logic

    private void OnChange(InputAction.CallbackContext ctx)
    {
        if (IsFocusing) return;
        EnableNextCamera();
    }

    private void EnableNextCamera()
    {
        DisableAllCameras();
        _firstCameraIndex = (_firstCameraIndex + 1) % _cameras.Count;
        _cameras.ElementAt(_firstCameraIndex).enabled = true;
    }

    private void DisableAllCameras()
    {
        _cameras.ForEach(c => c.enabled = false);
    }

    #endregion

    #region Focus Mode Logic

    private void OnFocusStarted(InputAction.CallbackContext ctx)
    {
        IsFocusing = true;

        DisableAllCameras();
        if (focusCamera != null)
            focusCamera.enabled = true;
    }

    private void OnFocusCanceled(InputAction.CallbackContext ctx)
    {
        IsFocusing = false;

        DisableAllCameras();

        if (_firstCameraIndex >= 0 && _firstCameraIndex < _cameras.Count)
            _cameras[_firstCameraIndex].enabled = true;
    }

    #endregion
}