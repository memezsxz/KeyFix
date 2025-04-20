using UnityEngine;

public class SpawnButton : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject spawnPrefab;
    [SerializeField] HintUI hintUI;

    public string InteractMessage => hintUI.message;

    public void Interact()
    {
        Spawn();
    }

    void Spawn()
    {
        var spawnedObject = Instantiate(spawnPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);

        float randomSize = Random.Range(0.1f, 1f);
        spawnedObject.transform.localScale = Vector3.one * randomSize;

        var randomColour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        spawnedObject.GetComponent<MeshRenderer>().material.color = randomColour;
    }

    public void ShowHint(bool show)
    {
        if (hintUI == null) return;

        if (show)
        {
            hintUI.ShowHint();
        }
        else
        {
            hintUI.HideHint();
        }
    }

}
