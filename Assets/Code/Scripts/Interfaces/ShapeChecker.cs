using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeChecker : MonoBehaviour
{
    [SerializeField]
    private GameObject wholeGamePanel;

    [SerializeField]
    private GameObject successfulPanel;
    [SerializeField]
    private GameObject failedPanel;

    [SerializeField]
    private AudioSource successfulSound;

    [SerializeField]
    private AudioSource failedSound;

    public Transform bulbGrid; // parent of bulbs
    public bool[] targetPattern; // must match manually to your shape

    public void CheckShape()
    {
        bool match = true;
        for (int i = 0; i < bulbGrid.childCount; i++)
        {
            LightBulb bulb = bulbGrid.GetChild(i).GetComponent<LightBulb>();
            bool bulbState = bulb.IsOn();
            if (bulbState != targetPattern[i])
            {
                match = false;
                break;
            }
        }

        if (match)
        {
            Debug.Log("Shape matched!");
            StartCoroutine(ShowSuccessfulPanel());
        }
        else
        {
            Debug.Log("Wrong shape!");
            StartCoroutine(ShowFailedPanel());
        }
    }

    IEnumerator ShowSuccessfulPanel()
    {
        successfulPanel.SetActive(true);
        successfulSound.Play();

        yield return new WaitForSeconds(1.5f); // Wait for 1 second

        wholeGamePanel.SetActive(false);

        successfulSound.Stop();
    }

    IEnumerator ShowFailedPanel()
    {
        failedPanel.SetActive(true);
        failedSound.Play();

        yield return new WaitForSeconds(1.5f); // Wait for 1 second

        failedPanel.SetActive(false);
        failedSound.Stop();
    }
}
