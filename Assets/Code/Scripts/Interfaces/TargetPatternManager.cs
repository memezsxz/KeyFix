using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a static grid representing a predefined target pattern of lights.
/// Used as a reference for color-matching or memory puzzles.
/// </summary>
public class TargetPatternManager : MonoBehaviour
{
    /// <summary>
    /// Prefab used to represent a single light pixel in the grid.
    /// </summary>
    public GameObject targetPixelPrefab;

    /// <summary>
    /// Parent transform to contain all instantiated pixels.
    /// </summary>
    public Transform targetGridParent;

    /// <summary>
    /// Width of the grid in number of cells.
    /// </summary>
    public int gridSizeX = 10;

    /// <summary>
    /// Height of the grid in number of cells.
    /// </summary>
    public int gridSizeY = 10;

    /// <summary>
    /// Color to use when the target pixel is "on".
    /// </summary>
    public Color onColor = Color.yellow;

    /// <summary>
    /// Color to use when the target pixel is "off".
    /// </summary>
    public Color offColor = Color.black;

    /// <summary>
    /// Hardcoded pattern used to define which pixels should be "on" or "off".
    /// </summary>
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

    /// <summary>
    /// Instantiates the grid and applies color based on the target pattern array.
    /// </summary>
    private void GenerateTargetPattern()
    {
        for (var y = 0; y < gridSizeY; y++)
        {
            for (var x = 0; x < gridSizeX; x++)
            {
                // Create a new pixel and parent it under the grid
                var pixel = Instantiate(targetPixelPrefab, targetGridParent);

                // Set the pixel's color based on the value in the pattern
                var img = pixel.GetComponent<Image>();
                img.color = targetPattern[y, x] == 1 ? onColor : offColor;
            }
        }
    }
}