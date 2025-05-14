using UnityEngine;

/// <summary>
/// Represents a colored panel in the color-matching puzzle.
/// Each panel has an assigned color and can be visually changed at runtime.
/// </summary>
public class Panel : MonoBehaviour
{
    /// <summary>
    /// The currently assigned logical color of this panel.
    /// </summary>
    [SerializeField] private PressButton.PressedColor color;

    /// <summary>
    /// The material used to visually reflect the panel's color.
    /// </summary>
    private Material _material;

    /// <summary>
    /// Gets the color currently assigned to this panel.
    /// </summary>
    public PressButton.PressedColor AssignedColor => color;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    /// <summary>
    /// Changes the logical and visual color of the panel.
    /// </summary>
    /// <param name="color">The new color to apply.</param>
    public void ChangeColor(PressButton.PressedColor color)
    {
        this.color = color;

        // Update the panel's material color based on the assigned enum
        switch (color)
        {
            case PressButton.PressedColor.Blue:
                _material.color = Color.blue;
                break;
            case PressButton.PressedColor.Red:
                _material.color = Color.red;
                break;
            case PressButton.PressedColor.Green:
                _material.color = Color.green;
                break;
        }
    }
}