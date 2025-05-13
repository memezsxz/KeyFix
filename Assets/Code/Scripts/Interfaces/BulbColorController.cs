using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulbColorController : MonoBehaviour
{
    public Image bulbImage;
    public Image bulbEffect;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public List<Image> TargetColorImages;
    [SerializeField] private Image currentColorImage;
    public TMP_Text matchMessageText;
    public AudioClip doneSound;
    public GameObject ArrowRoomInterface;
    [SerializeField] private Color targetColor;
    [SerializeField] private int minimumMatch = 30;
    private List<Light> lights = new();

    private readonly int roundTo = 100;

    public bool IsDone { get; private set; }

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

    private void UpdateLightColor()
    {
        var currentColor = new Color(redSlider.value, greenSlider.value, blueSlider.value, 1f);

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

    private bool ColorsAreClose(Color a, Color b)
    {
        var r1 = (int)(a.r * roundTo);
        var g1 = (int)(a.g * roundTo);
        var b1 = (int)(a.b * roundTo);

        var r2 = (int)(b.r * roundTo);
        var g2 = (int)(b.g * roundTo);
        var b2 = (int)(b.b * roundTo);

        var total = Mathf.Abs(r1 - r2) + Mathf.Abs(g1 - g2) + Mathf.Abs(b1 - b2);
        return total < minimumMatch;
    }

    private IEnumerator HideArrowInterfaceAfterDelay()
    {
        lights.ForEach(l => l.color = targetColor);
        yield return new WaitForSeconds(2f);
        ArrowRoomInterface.SetActive(false);
        IsDone = true;
        MazeController.Instance.BoardIsDone();
    }
}