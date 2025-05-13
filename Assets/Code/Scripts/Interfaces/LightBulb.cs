using UnityEngine;
using UnityEngine.UI;

public class LightBulb : MonoBehaviour
{
    public GameObject offSprite;
    public GameObject onSprite;
    private Image bulbImage;
    private bool isOn;

    private void Awake()
    {
        bulbImage = GetComponent<Image>(); // automatically find the Image
    }

    public void ToggleBulb()
    {
        isOn = !isOn;
        if (isOn)
        {
            offSprite.SetActive(false);
            onSprite.SetActive(true);
        }
        else
        {
            offSprite.SetActive(true);
            onSprite.SetActive(false);
        }
    }

    public bool IsOn()
    {
        return isOn;
    }
}