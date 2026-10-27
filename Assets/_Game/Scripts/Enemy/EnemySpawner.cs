using System.Collections.Generic;
using UnityEngine;

// Spawns enemies at child EnemySpawnPoints, optionally only those outside the flashlight beam.
// After all generators are on, only the chaser spawns, at a faster rate.
[RequireComponent(typeof(EffectSpawner))]
public class EnemySpawner : MonoBehaviour
{
    // found in the scene when empty, since a prefab asset can't reference scene objects
    [SerializeField] private FlashlightDetector flashlight;
    // true = skip spawn points inside the flashlight beam
    [SerializeField] private bool avoidLitPoints = false;
    // enemies alive at the same time
    [SerializeField, Min(1)] private int maxAlive = 5;
    [SerializeField, Min(0.1f)] private float spawnInterval = 5f;

    [Header("After all generators are on")]
    [SerializeField] private EnemyBase chaserPrefab;
    [SerializeField, Min(0.1f)] private float finalSpawnInterval = 2f;
    // true = stop spawning instead, e.g. before a boss fight
    [SerializeField] private bool stopWhenGeneratorsOn = false;

    private EffectSpawner effectSpawner;
    private EnemySpawnPoint[] spawnPoints;
    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private readonly List<EnemySpawnPoint> candidates = new List<EnemySpawnPoint>();
    private bool finalPhase;
    private float timer;

    private void Awake()
    {
        effectSpawner = GetComponent<EffectSpawner>();

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
        if (stopWhenGeneratorsOn)
        {
            // enemies still behind their smoke would appear after the wipe
            effectSpawner.CancelAll();
            enabled = false;
            Debug.Log("[EnemySpawner] All generators on, spawning stopped");
            return;
        }

        finalPhase = true;
        Debug.Log("[EnemySpawner] All generators on, only chasers spawn now");
    }

    private void TrySpawn()
    {
        // enemies destroy themselves on death, so destroyed entries compare equal to null
        aliveEnemies.RemoveAll(enemy => enemy == null);
        if (aliveEnemies.Count + effectSpawner.PendingCount >= maxAlive) return;

        candidates.Clear();
        foreach (EnemySpawnPoint point in spawnPoints)
        {
            if (finalPhase && !point.Allows(chaserPrefab)) continue;
            // optionally keep enemies from popping up inside the beam
            if (avoidLitPoints && flashlight != null && flashlight.IsPointLit(point.Position)) continue;
            candidates.Add(point);
        }
        if (candidates.Count == 0) return;

        EnemySpawnPoint chosen = candidates[Random.Range(0, candidates.Count)];
        EnemyBase prefab = finalPhase ? chaserPrefab : chosen.GetRandomEnemy();
        if (prefab == null) return;

        effectSpawner.Spawn(prefab, chosen.Position, enemy => aliveEnemies.Add(enemy.gameObject));
    }
}
