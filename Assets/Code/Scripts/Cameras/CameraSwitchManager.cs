using UnityEngine;
using System.Collections.Generic;

public class CameraSwitchManager : Singleton<CameraSwitchManager>
{
    public GameObject ActiveCamera => _cameraStack.Count > 0 ? _cameraStack[^1] : null;
    private readonly List<GameObject> _cameraStack = new();

    public void EnterCameraZone(GameObject cam)
    {
        if (!_cameraStack.Contains(cam))
        {
            if (_cameraStack.Count > 0)
                _cameraStack[^1].SetActive(false);

            _cameraStack.Add(cam);
            cam.SetActive(true);
            Camera.SetupCurrent(cam.GetComponent<Camera>());
        }
    }

    public void ExitCameraZone(GameObject cam)
    {
        if (_cameraStack.Remove(cam))
        {
            cam.SetActive(false);

            if (_cameraStack.Count > 0)
            {
                var newTop = _cameraStack[^1];
                newTop.SetActive(true);
                Camera.SetupCurrent(newTop.GetComponent<Camera>());
            }
        }
    }
}