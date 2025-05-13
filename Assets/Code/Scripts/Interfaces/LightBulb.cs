using UnityEngine;
using UnityEngine.UI;

public class LightBulb : MonoBehaviour
{
    public AudioSource bulbSound;
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
            bulbSound.Play();
        }
        else
        {
            offSprite.SetActive(true);
            onSprite.SetActive(false);
            bulbSound.Play();
        }
    }

    public bool IsOn()
    {
        return isOn;
    }
}