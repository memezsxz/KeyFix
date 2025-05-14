using UnityEngine;

/// <summary>
/// Controls the activation of the light challenge interface or scene.
/// </summary>
public class ControllLightChanllenge : MonoBehaviour
{
    /// <summary>
    /// The GameObject representing the light challenge scene/UI.
    /// </summary>
    [Header("Challenge Reference")] [SerializeField]
    private GameObject challengeScene;

    /// <summary>
    /// Displays the light challenge scene by enabling its GameObject.
    /// </summary>
    public void showChallenge()
    {
        // Activate the assigned challenge scene UI
        challengeScene.SetActive(true);
    }
}