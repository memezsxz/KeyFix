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
        if (hintText != null)
            hintText.text = message;
    }


    // Hind the hint object
    public void HideHint()
    {
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
