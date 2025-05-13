using TMPro;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public string message = "";
    public float duration = 3f;

    private void LateUpdate()
    {

        if (CameraSwitchManager.Instance)
        {
            Camera cam =  CameraSwitchManager.Instance.ActiveCamera.GetComponent<Camera>();
            Vector3 cameraForward = cam.transform.forward;
            cameraForward.y = 0; // Remove vertical component
            cameraForward.Normalize(); // Ensure it's a valid direction
            transform.forward = cameraForward;

        }
        else
        {
            transform.forward = Camera.main.transform.forward;
        }
    }

    // Show hint for a duration
    public void ShowHint()
    {
        gameObject.SetActive(true); // display the hint object
        if (hintText != null)
            hintText.text = message;
    }


    // Hind the hint object
    public void HideHint()
    {
        gameObject.SetActive(false);
    }
}