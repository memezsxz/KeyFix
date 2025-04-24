using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulbColorController : MonoBehaviour
{
    //design inteface
    public Image bulbImage;
    public Image bulbEffect;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    public List<Image> TargetColor;
    public string colorInHex = "#FFFFFF";

    public TMP_Text matchMessageText;  // Reference to UI Text
    public float matchThreshold = 0.1f; // How close is “close enough”?
    public AudioSource doneSound;

    public GameObject ArrowRoomInteface;


    private Color targetColor;


    public void showArrowChalange() {

        redSlider.value = 225;
        greenSlider.value = 255;
        blueSlider.value = 255;

        blueSlider.interactable = true;
        redSlider.interactable = true;
        greenSlider.interactable = true;


        ArrowRoomInteface.SetActive(true);

        if (ColorUtility.TryParseHtmlString(colorInHex, out Color hexColor))
        {
            targetColor = hexColor;

            foreach (Image img in TargetColor)
                img.color = targetColor;
        }
        else
        {
            Debug.LogWarning("Invalid HEX color string: " + colorInHex);
        }

        if (matchMessageText != null)
        {
            matchMessageText.gameObject.SetActive(false);

        }
    }

   

    void Update()
    {
        float r = redSlider.value;
        float g = greenSlider.value;
        float b = blueSlider.value;

        Color currentColor = new Color(r, g, b, 1f);

        bulbImage.color = currentColor;
        bulbEffect.color = currentColor;

        // Compare with target color
        if (ColorsAreClose(currentColor, targetColor, matchThreshold))
        {
            if (!matchMessageText.gameObject.activeSelf)
            {
                blueSlider.interactable = false;
                redSlider.interactable = false;
                greenSlider.interactable = false;


                matchMessageText.gameObject.SetActive(true);
                doneSound.Play();
                StartCoroutine(HideArrowInterfaceAfterDelay());
            }
        }
        else
        {
            matchMessageText.gameObject.SetActive(false);
        }
    }

    bool ColorsAreClose(Color a, Color b, float threshold)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }


    IEnumerator HideArrowInterfaceAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        ArrowRoomInteface.SetActive(false);
    }
}
