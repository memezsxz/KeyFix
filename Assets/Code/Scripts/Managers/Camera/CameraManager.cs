using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;


public class CameraManager : PersistentSingleton<CameraManager>
{
    // Camera manager needs to adapt to change between multiple cameras for cut scenes
    // we can do that using camera zones -colliders-

    private List<CinemachineVirtualCameraBase> _cameras;
    [SerializeField] private CinemachineVirtualCameraBase focusCamera;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject PlayerEye;
    private int _firstCameraIndex = -1;
    private InputAction _lockInputAction;
    private InputAction _changeInputAction;
    private bool _focusOnEnemy = false;

    public bool IsFocusing => _focusOnEnemy;

    private void Start()
    {
        // this is excluding TopRig, MiddleRig, and BottomRig of FreeLook camera
        _cameras = new List<CinemachineVirtualCameraBase>(
            gameObject.GetComponentsInChildren<CinemachineVirtualCameraBase>()
                .Where(cam =>
                    !(cam.transform.parent != null &&
                      cam.transform.parent.GetComponent<Cinemachine.CinemachineFreeLook>() != null))
        );

        // Debug.LogWarning("cms:" + _cameras.Count);

        _cameras.ForEach(c => Debug.Log(c.name));
        _cameras.Remove(focusCamera);
        // Debug.LogWarning("cms:" + _cameras.Count);


        if (_cameras.Count == 0)
        {
            Debug.LogWarning("No virtual cameras found!");
            return;
        }

        var input = GetComponent<PlayerInput>();
        _lockInputAction = input.actions["Lock"];
        _changeInputAction = input.actions["Change"];

        _changeInputAction.started += OnChange;
        _lockInputAction.started += OnFocusStarted;
        _lockInputAction.canceled += OnFocusCanceled;

        _cameras.ForEach(c =>
        {
            c.enabled = false;
            c.LookAt = Player.transform;
            c.Follow = Player.transform;
        });

        EnableNextCamera();
    }

    private void OnChange(InputAction.CallbackContext ctx)
    {
        if (_focusOnEnemy) return;
        EnableNextCamera();
    }

    private void OnDisable()
    {
        if (_changeInputAction != null)
            _changeInputAction.started -= OnChange;

        if (_lockInputAction != null)
        {
            _lockInputAction.started -= OnFocusStarted;
            _lockInputAction.canceled -= OnFocusCanceled;
        }
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

    private void OnFocusStarted(InputAction.CallbackContext ctx)
    {
        _focusOnEnemy = true;

        DisableAllCameras();
        if (focusCamera != null)
            focusCamera.enabled = true;
    }

    private void OnFocusCanceled(InputAction.CallbackContext ctx)
    {
        _focusOnEnemy = false;

        DisableAllCameras();

        if (_firstCameraIndex >= 0 && _firstCameraIndex < _cameras.Count)
            _cameras[_firstCameraIndex].enabled = true;
    }
}