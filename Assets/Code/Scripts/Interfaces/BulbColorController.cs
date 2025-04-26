using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulbColorController : MonoBehaviour
{
    private List<Light> lights = new List<Light>();

    public Image bulbImage;
    public Image bulbEffect;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public List<Image> TargetColorImages;
    [SerializeField] Image currentColorImage;
    public TMP_Text matchMessageText;
    public AudioClip doneSound;
    public GameObject ArrowRoomInterface;
    [SerializeField] Color targetColor;

    private int roundTo = 100;
    [SerializeField] int minimumMatch = 30;

    public bool IsDone { get; private set; } = false;

    private void Start()
    {
        lights = MazeController.Instance.Lights;
        redSlider.onValueChanged.AddListener(_ => UpdateLightColor());
        greenSlider.onValueChanged.AddListener(_ => UpdateLightColor());
        blueSlider.onValueChanged.AddListener(_ => UpdateLightColor());
    }

    public void ShowArrowChallenge()
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

        ArrowRoomInterface.SetActive(true);

        targetColor.a = 1;
        TargetColorImages.ForEach(i => i.color = targetColor);

        if (matchMessageText != null)
            matchMessageText.gameObject.SetActive(false);
    }

    void UpdateLightColor()
    {
        Color currentColor = new Color(redSlider.value, greenSlider.value, blueSlider.value, 1f);

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
        return total < minimumMatch;
    }

    IEnumerator HideArrowInterfaceAfterDelay()
    {
        lights.ForEach(l => l.color = targetColor);
        yield return new WaitForSeconds(2f);
        ArrowRoomInterface.SetActive(false);
        IsDone = true;
        MazeController.Instance.BoardIsDone();
    }
}