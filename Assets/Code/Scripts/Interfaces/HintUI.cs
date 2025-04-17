using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public string message = "";
    public float duration = 3f;

    // Show hint for a duration
    public void ShowHint()
    {
        gameObject.SetActive(true); // display the hint object
        hintText.text = message;
        CancelInvoke(); // Prevent overlap
        Invoke(nameof(HideHint), duration); //hide the hint after the duration reach
    }


    // Hind the hint object
    public void HideHint()
    {
        gameObject.SetActive(false);
    }


}
