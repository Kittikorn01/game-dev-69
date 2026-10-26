using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns enemies at child EnemySpawnPoints that are outside the flashlight beam.
// After all generators are on, only the chaser spawns, at a faster rate.
public class EnemySpawner : MonoBehaviour
{
    // found in the scene when empty, since a prefab asset can't reference scene objects
    [SerializeField] private FlashlightDetector flashlight;
    // enemies alive at the same time
    [SerializeField, Min(1)] private int maxAlive = 5;
    [SerializeField, Min(0.1f)] private float spawnInterval = 5f;
    // optional, played at the spawn point before the enemy appears
    [SerializeField] private SpawnEffect spawnEffectPrefab;
    // seconds between the effect and the enemy, 0.33 = smoke at its biggest
    [SerializeField, Min(0f)] private float spawnDelay = 0.33f;

    [Header("After all generators are on")]
    [SerializeField] private EnemyBase chaserPrefab;
    [SerializeField, Min(0.1f)] private float finalSpawnInterval = 2f;

    private EnemySpawnPoint[] spawnPoints;
    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private readonly List<EnemySpawnPoint> candidates = new List<EnemySpawnPoint>();
    private bool finalPhase;
    private float timer;
    // enemies waiting for spawnDelay, counted toward maxAlive
    private int pendingSpawns;

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
        if (spawnPoints.Length == 0) Debug.LogWarning($"[EnemySpawner] No EnemySpawnPoint children: {name}", this);

        if (flashlight == null) flashlight = FindFirstObjectByType<FlashlightDetector>();
        if (flashlight == null) Debug.LogWarning($"[EnemySpawner] No FlashlightDetector in scene, spawning ignores light: {name}", this);
    }

    // Start, not OnEnable: GameManager.Instance is only set after every Awake
    private void Start()
    {
        if (chaserPrefab == null) Debug.LogWarning($"[EnemySpawner] Chaser Prefab not set: {name}", this);

        if (GameManager.Instance != null) GameManager.Instance.AllGeneratorsActivated += EnterFinalPhase;
        else Debug.LogWarning($"[EnemySpawner] No GameManager in scene: {name}", this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.AllGeneratorsActivated -= EnterFinalPhase;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < (finalPhase ? finalSpawnInterval : spawnInterval)) return;

        timer = 0f;
        TrySpawn();
    }

    private void EnterFinalPhase()
    {
        finalPhase = true;
        Debug.Log("[EnemySpawner] All generators on, only chasers spawn now");
    }

    private void TrySpawn()
    {
        // enemies destroy themselves on death, so destroyed entries compare equal to null
        aliveEnemies.RemoveAll(enemy => enemy == null);
        if (aliveEnemies.Count + pendingSpawns >= maxAlive) return;

        candidates.Clear();
        foreach (EnemySpawnPoint point in spawnPoints)
        {
            if (finalPhase && !point.Allows(chaserPrefab)) continue;
            // the player must never see an enemy pop up inside the beam
            if (flashlight != null && flashlight.IsPointLit(point.Position)) continue;
            candidates.Add(point);
        }
        if (candidates.Count == 0) return;

        EnemySpawnPoint chosen = candidates[Random.Range(0, candidates.Count)];
        EnemyBase prefab = finalPhase ? chaserPrefab : chosen.GetRandomEnemy();
        if (prefab == null) return;

        StartCoroutine(SpawnAfterEffect(prefab, chosen.Position));
    }

    // the enemy still comes out if the flashlight hits the point during the delay
    private IEnumerator SpawnAfterEffect(EnemyBase prefab, Vector2 position)
    {
        if (spawnEffectPrefab != null) Instantiate(spawnEffectPrefab, position, Quaternion.identity);

        pendingSpawns++;
        yield return new WaitForSeconds(spawnDelay);
        pendingSpawns--;

        EnemyBase enemy = Instantiate(prefab, position, Quaternion.identity);
        aliveEnemies.Add(enemy.gameObject);
    }
}
