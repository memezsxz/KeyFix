using UnityEngine;

public class CameraChange : MonoBehaviour
{
    private GameObject areaCam;

    private void Start()
    {
        areaCam = GetComponentInChildren<Camera>(true)?.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraSwitchManager.Instance?.EnterCameraZone(areaCam);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraSwitchManager.Instance?.ExitCameraZone(areaCam);
        }
    }
}