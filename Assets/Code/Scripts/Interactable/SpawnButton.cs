using UnityEngine;

public class SpawnButton : InteractableBase
{
    [SerializeField] private GameObject spawnPrefab;

    // public  string InteractMessage => hintUI.message;

    public override void Interact()
    {
        Spawn();
    }

    private void Spawn()
    {
        var spawnedObject = Instantiate(spawnPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);

        var randomSize = Random.Range(0.1f, 1f);
        spawnedObject.transform.localScale = Vector3.one * randomSize;

        var randomColour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        spawnedObject.GetComponent<MeshRenderer>().material.color = randomColour;
    }
}