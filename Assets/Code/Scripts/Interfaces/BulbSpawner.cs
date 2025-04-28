using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbSpawner : MonoBehaviour
{
    public GameObject bulbPrefab;
    public int rows = 6;
    public int columns = 6;

    void Start()
    {
        for (int i = 0; i < rows * columns; i++)
        {
            Instantiate(bulbPrefab, transform);
        }
    }
}
