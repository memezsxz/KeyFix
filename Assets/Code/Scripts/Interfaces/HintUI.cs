using TMPro;
using UnityEngine;

/// <summary>
/// Displays a UI hint that always faces the camera.
/// Can be toggled on and off dynamically by interactable objects.
/// </summary>
public class HintUI : MonoBehaviour
{
    /// <summary>
    /// The text component used to display the hint message.
    /// </summary>
    public TextMeshProUGUI hintText;

    /// <summary>
    /// The default message that will be shown on the hint.
    /// </summary>
    public string message = "";

    /// <summary>
    /// Duration (in seconds) the hint should stay active if needed.
    /// Currently not used in logic but available for expansion.
    /// </summary>
    public float duration = 3f;

    private void LateUpdate()
    {
        // Rotate the hint to face the player's camera horizontally
        if (CameraSwitchManager.Instance)
        {
            Camera cam = CameraSwitchManager.Instance.ActiveCamera.GetComponent<Camera>();
            Vector3 cameraForward = cam.transform.forward;
            cameraForward.y = 0f; // Ignore vertical tilt
            cameraForward.Normalize();
            transform.forward = cameraForward;
        }
        else
        {
            // Fallback to main camera if no active camera is set
            transform.forward = Camera.main.transform.forward;
        }
    }

    /// <summary>
    /// Activates the hint UI and sets the hint text.
    /// </summary>
    public void ShowHint()
    {
        gameObject.SetActive(true);

        if (hintText != null)
            hintText.text = message;
    }

    /// <summary>
    /// Deactivates the hint UI.
    /// </summary>
    public void HideHint()
    {
        gameObject.SetActive(false);
    }
}