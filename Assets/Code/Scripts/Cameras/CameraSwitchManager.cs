using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitchManager : Singleton<CameraSwitchManager>
{
    public GameObject ActiveCamera { get; private set; }

    public void SetActiveCamera(GameObject newCam)
    {
        if (ActiveCamera != null)
            ActiveCamera.gameObject.SetActive(false);

        ActiveCamera = newCam;

        if (ActiveCamera != null)
            ActiveCamera.gameObject.SetActive(true);
    }
}
