using System;
using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class BulbColorController : MonoBehaviour
{
    [SerializeField] private List<Light> lights;

    //design inteface
    public Image bulbImage;
    public Image bulbEffect;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    public List<Image> TargetColorImages;

    [SerializeField] Image currentColorImage;
    public TMP_Text matchMessageText; // Reference to UI Text

    private int roundTo = 100;
    private int minimumMatch = 30;

    public AudioClip doneSound;

    public GameObject ArrowRoomInteface;


    [SerializeField] Color targetColor;


    private void Start()
    {
        redSlider.onValueChanged.AddListener(_ => UpdateLightColor());
        greenSlider.onValueChanged.AddListener(_ => UpdateLightColor());
        blueSlider.onValueChanged.AddListener(_ => UpdateLightColor());
    }

    public void showArrowChalange()
    {
        if (lights.Count > 1)
        {
            redSlider.value = lights[0].color.r;
            greenSlider.value = lights[0].color.g;
            blueSlider.value = lights[0].color.b;
        }
        else
        {
            redSlider.value = 1;
            greenSlider.value = 1;
            blueSlider.value = 1;
        }

        blueSlider.interactable = true;
        redSlider.interactable = true;
        greenSlider.interactable = true;


        ArrowRoomInteface.SetActive(true);

        targetColor.a = 1;
        // print(targetColor.r + " : " + targetColor.g + " : " + targetColor.b);

        TargetColorImages.ForEach(i => i.color = targetColor);


        if (matchMessageText != null)
        {
            matchMessageText.gameObject.SetActive(false);
        }
    }


    void UpdateLightColor()
    {
        float r = redSlider.value;
        float g = greenSlider.value;
        float b = blueSlider.value;

        Color currentColor = new Color(r, g, b, 1f);

        lights.ForEach(l => l.color = currentColor);
        currentColorImage.color = currentColor;
        bulbImage.color = currentColor;
        bulbEffect.color = currentColor;

        if (ColorsAreClose(currentColor, targetColor))
        {
            if (matchMessageText.gameObject.activeSelf) return;

            blueSlider.interactable = false;
            redSlider.interactable = false;
            greenSlider.interactable = false;

            matchMessageText.gameObject.SetActive(true);
            SoundManager.Instance.PlaySound(doneSound);
            StartCoroutine(HideArrowInterfaceAfterDelay());
        }
        else
        {
            matchMessageText.gameObject.SetActive(false);
        }
    }

    bool ColorsAreClose(Color a, Color b)
    {
        int r1 = (int)(a.r * roundTo);
        int g1 = (int)(a.g * roundTo);
        int b1 = (int)(a.b * roundTo);

        int r2 = (int)(b.r * roundTo);
        int g2 = (int)(b.g * roundTo);
        int b2 = (int)(b.b * roundTo);

        int total = Mathf.Abs(r1 - r2) + Mathf.Abs(g1 - g2) + Mathf.Abs(b1 - b2);

        // hint, DO NOT DELETE
        // print( Mathf.Abs(r1 - r2)+ " : " + Mathf.Abs(g1 - g2) + " : " + Mathf.Abs(b1 - b2));
        return total < minimumMatch;
    }


    IEnumerator HideArrowInterfaceAfterDelay()
    {
        lights.ForEach(l => l.color = targetColor);
        yield return new WaitForSeconds(2f);
        ArrowRoomInteface.SetActive(false);
    }
}