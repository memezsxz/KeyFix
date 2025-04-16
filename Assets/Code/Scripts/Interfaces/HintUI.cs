using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public string message = "";
    public float duration = 3f;

    // Show hint for a duration (optional)
    public void ShowHint()
    {
        gameObject.SetActive(true);
        hintText.text = message;
        CancelInvoke(); // Prevent overlap
        Invoke(nameof(HideHint), duration);
    }


    // Manually hide if needed
    public void HideHint()
    {
        gameObject.SetActive(false);
    }


}
