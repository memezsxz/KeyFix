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
    private readonly int[,] targetPattern =
    {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 1, 0, 0, 0, 0, 1, 0, 1 },
        { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
        { 1, 0, 0, 0, 1, 1, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 1, 1, 0, 0, 0, 1 },
        { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
        { 1, 0, 1, 0, 0, 0, 0, 1, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
    };

    private void Start()
    {
        GenerateTargetPattern();
    }

    private void GenerateTargetPattern()
    {
        for (var y = 0; y < gridSizeY; y++)
        for (var x = 0; x < gridSizeX; x++)
        {
            var pixel = Instantiate(targetPixelPrefab, targetGridParent);
            var img = pixel.GetComponent<Image>();

            if (targetPattern[y, x] == 1)
                img.color = onColor; // Yellow for ON
            else
                img.color = offColor; // Black for OFF
        }
    }
}