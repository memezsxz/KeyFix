using UnityEngine;

/// <summary>
/// Spawns falling paper objects at intervals and ends the game if a limit is exceeded.
/// </summary>
public class PaperPrinterParticleSystem : MonoBehaviour
{
    #region Spawn Settings

    /// <summary>
    /// Prefab for the paper object to spawn.
    /// </summary>
    [Header("Spawn Settings")] [SerializeField]
    private GameObject paperPrefab;

    /// <summary>
    /// Time (in seconds) between paper spawns.
    /// </summary>
    [SerializeField] private float spawnInterval = 0.5f;

    /// <summary>
    /// Maximum number of paper objects allowed before triggering game over.
    /// </summary>
    [SerializeField] private int paperLimit = 50;

    #endregion

    #region Paper Physics

    /// <summary>
    /// Force with which paper is ejected (not currently used but reserved for future expansion).
    /// </summary>
    // [Header("Paper Physics")] [SerializeField]
    // private float launchForce = 1f;

    #endregion

    /// <summary>
    /// Transform of the model that holds spawned papers.
    /// </summary>
    private Transform modelTransform;

    /// <summary>
    /// Area around the spawn point where paper can appear.
    /// </summary>
    private Vector2 spawnAreaSize;

    /// <summary>
    /// Internal timer to track time between spawns.
    /// </summary>
    private float timer;

    private void Start()
    {
        modelTransform = transform.parent.GetChild(0).GetComponent<Transform>();

        if (modelTransform == null) Debug.LogWarning("Child 'model' not found under Paper Printer Particle System!");

        // Use local scale as a basis for spawn area
        var localScale = transform.localScale;
        spawnAreaSize = new Vector2(localScale.x, localScale.z);
    }

    private void Update()
    {
        if (modelTransform == null) return;

        // If the number of papers exceeds the limit, trigger game over
        if (modelTransform.childCount >= paperLimit)
        {
            Debug.Log("Too many papers! You lose!");
            GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
            enabled = false;
            return;
        }

        // Spawn paper at regular intervals
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPaper();
            timer = 0f;
        }
    }

    /// <summary>
    /// Instantiates a paper prefab at a randomized position within the spawn area.
    /// </summary>
    private void SpawnPaper()
    {
        // Create a random offset inside the defined spawn area
        var localOffset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            0f,
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
        );

        // Convert the local offset to a world position
        var spawnPos = transform.TransformPoint(localOffset);

        // Instantiate and parent the paper to the model
        var paper = Instantiate(paperPrefab, spawnPos, Quaternion.identity);
        paper.transform.SetParent(modelTransform, true);
    }
}