using UnityEngine;

/// <summary>
/// Spawns a grid of bulb objects as children of this GameObject.
/// Typically used to create a visual puzzle board for the light challenge.
/// </summary>
public class BulbSpawner : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// The prefab to instantiate for each bulb.
    /// </summary>
    [SerializeField] private GameObject bulbPrefab;

    /// <summary>
    /// Number of rows in the bulb grid.
    /// </summary>
    [SerializeField] private int rows = 6;

    /// <summary>
    /// Number of columns in the bulb grid.
    /// </summary>
    [SerializeField] private int columns = 6;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Instantiates a grid of bulbs as children of this object at Start.
    /// </summary>
    private void Start()
    {
        for (var i = 0; i < rows * columns; i++)
        {
            Instantiate(bulbPrefab, transform);
        }
    }

    #endregion
}