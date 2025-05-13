using UnityEngine;

public class BulbSpawner : MonoBehaviour
{
    public GameObject bulbPrefab;
    public int rows = 6;
    public int columns = 6;

    private void Start()
    {
        for (var i = 0; i < rows * columns; i++) Instantiate(bulbPrefab, transform);
    }
}