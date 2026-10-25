using System.Collections.Generic;
using UnityEngine;

// Summons one minion every interval at a child EnemySpawnPoint outside the flashlight beam.
// The minion type is picked by weight from a list that changes once HP drops low enough.
[RequireComponent(typeof(Health))]
public class BossSummoner : MonoBehaviour
{
    // found in the scene when empty, since a prefab asset can't reference scene objects
    [SerializeField] private FlashlightDetector flashlight;
    [SerializeField, Min(0.1f)] private float summonInterval = 3f;
    // minions alive at the same time
    [SerializeField, Min(1)] private int maxAlive = 5;
    // at or below this share of max HP the phase two list is used
    [SerializeField, Range(0f, 1f)] private float phaseTwoHpRatio = 0.5f;
    [SerializeField] private WeightedEnemy[] phaseOneEnemies;
    [SerializeField] private WeightedEnemy[] phaseTwoEnemies;

    private Health health;
    private EnemySpawnPoint[] spawnPoints;
    private readonly List<GameObject> aliveMinions = new List<GameObject>();
    private readonly List<EnemySpawnPoint> candidates = new List<EnemySpawnPoint>();
    private float timer;

    private bool IsPhaseTwo => health.CurrentHp <= health.MaxHp * phaseTwoHpRatio;

    private void Awake()
    {
        health = GetComponent<Health>();

        spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
        if (spawnPoints.Length == 0) Debug.LogWarning($"[BossSummoner] No EnemySpawnPoint children: {name}", this);

        if (flashlight == null) flashlight = FindFirstObjectByType<FlashlightDetector>();
        if (flashlight == null) Debug.LogWarning($"[BossSummoner] No FlashlightDetector in scene, summoning ignores light: {name}", this);
    }

    private void Update()
    {
        if (health.CurrentHp <= 0) return;

        timer += Time.deltaTime;
        if (timer < summonInterval) return;

        timer = 0f;
        TrySummon();
    }

    private void TrySummon()
    {
        // minions destroy themselves on death, so destroyed entries compare equal to null
        aliveMinions.RemoveAll(minion => minion == null);
        if (aliveMinions.Count >= maxAlive) return;

        candidates.Clear();
        foreach (EnemySpawnPoint point in spawnPoints)
        {
            // the player must never see a minion pop up inside the beam
            if (flashlight != null && flashlight.IsPointLit(point.Position)) continue;
            candidates.Add(point);
        }
        if (candidates.Count == 0) return;

        EnemyBase prefab = PickEnemy(IsPhaseTwo ? phaseTwoEnemies : phaseOneEnemies);
        if (prefab == null) return;

        EnemySpawnPoint chosen = candidates[Random.Range(0, candidates.Count)];
        EnemyBase minion = Instantiate(prefab, chosen.Position, Quaternion.identity);
        aliveMinions.Add(minion.gameObject);
    }

    private static EnemyBase PickEnemy(WeightedEnemy[] pool)
    {
        if (pool == null) return null;

        int totalWeight = 0;
        foreach (WeightedEnemy entry in pool)
        {
            if (entry.Prefab != null) totalWeight += entry.Weight;
        }
        if (totalWeight <= 0) return null;

        int roll = Random.Range(0, totalWeight);
        foreach (WeightedEnemy entry in pool)
        {
            if (entry.Prefab == null) continue;
            if (roll < entry.Weight) return entry.Prefab;
            roll -= entry.Weight;
        }
        return null;
    }
}
