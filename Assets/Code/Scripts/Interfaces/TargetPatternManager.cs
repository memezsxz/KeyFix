using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetPatternManager : MonoBehaviour
{
    public GameObject targetPixelPrefab;
    public Transform targetGridParent;
    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public Color onColor = Color.yellow;
    public Color offColor = Color.black;

    // Example target pattern: 1 = light ON, 0 = light OFF
    private int[,] targetPattern = new int[,]
    {
        {1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,1},
        {1,0,1,0,0,0,0,1,0,1},
        {1,0,0,1,0,0,1,0,0,1},
        {1,0,0,0,1,1,0,0,0,1},
        {1,0,0,0,1,1,0,0,0,1},
        {1,0,0,1,0,0,1,0,0,1},
        {1,0,1,0,0,0,0,1,0,1},
        {1,0,0,0,0,0,0,0,0,1},
        {1,1,1,1,1,1,1,1,1,1}
    };

    void Start()
    {
        GenerateTargetPattern();
    }

    void GenerateTargetPattern()
    {
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                GameObject pixel = Instantiate(targetPixelPrefab, targetGridParent);
                Image img = pixel.GetComponent<Image>();

                if (targetPattern[y, x] == 1)
                {
                    img.color = onColor; // Yellow for ON
                }
                else
                {
                    img.color = offColor; // Black for OFF
                }
            }
        }
    }
}
