using Code.Scripts.Managers;
using UnityEngine;

public class PaperPrinterParticleSystem : MonoBehaviour
{
    [Header("Spawn Settings")] [SerializeField]
    private GameObject paperPrefab;

    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private int paperLimit = 50;

    [Header("Paper Physics")] [SerializeField]
    private float launchForce = 1f;

    private Transform modelTransform;

    private Vector2 spawnAreaSize;
    private float timer;

    private void Start()
    {
        modelTransform = transform.parent.GetChild(0).GetComponent<Transform>();
        if (modelTransform == null) Debug.LogWarning("Child 'model' not found under Paper Printer Particle System!");

        var localScale = transform.localScale;
        spawnAreaSize = new Vector2(localScale.x, localScale.z);
    }

    private void Update()
    {
        if (modelTransform == null) return;

        if (modelTransform.childCount >= paperLimit)
        {
            Debug.Log("Too many papers! You lose!");
            GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
            enabled = false;

            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPaper();
            timer = 0f;
        }
    }

    private void SpawnPaper()
    {
        var localOffset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            0f,
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
        );

        var spawnPos = transform.TransformPoint(localOffset);

        var paper = Instantiate(paperPrefab, spawnPos, Quaternion.identity);
        paper.transform.SetParent(modelTransform, true);
    }
}