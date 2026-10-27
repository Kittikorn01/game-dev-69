using System.Collections.Generic;
using UnityEngine;

// Summons summonCount minions every interval at child EnemySpawnPoints, optionally only outside the flashlight beam.
// The minion type is picked by weight from a list that changes once HP drops low enough.
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EffectSpawner))]
public class BossSummoner : MonoBehaviour
{
    // found in the scene when empty, since a prefab asset can't reference scene objects
    [SerializeField] private FlashlightDetector flashlight;
    // true = skip spawn points inside the flashlight beam
    [SerializeField] private bool avoidLitPoints = false;
    [SerializeField, Min(0.1f)] private float summonInterval = 3f;
    // minions summoned together each interval, each at a different point
    [SerializeField, Min(1)] private int summonCount = 1;
    // minions alive at the same time
    [SerializeField, Min(1)] private int maxAlive = 5;
    // at or below this share of max HP the phase two list is used
    [SerializeField, Range(0f, 1f)] private float phaseTwoHpRatio = 0.5f;
    [SerializeField] private WeightedEnemy[] phaseOneEnemies;
    [SerializeField] private WeightedEnemy[] phaseTwoEnemies;

    private Health health;
    private EffectSpawner effectSpawner;
    private EnemySpawnPoint[] spawnPoints;
    private readonly List<GameObject> aliveMinions = new List<GameObject>();
    private readonly List<EnemySpawnPoint> candidates = new List<EnemySpawnPoint>();
    private float timer;

    // full name: importing System would make Random ambiguous
    public event System.Action Summoned;

    private bool IsPhaseTwo => health.CurrentHp <= health.MaxHp * phaseTwoHpRatio;

    private void Awake()
    {
        health = GetComponent<Health>();
        effectSpawner = GetComponent<EffectSpawner>();

        spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
        if (spawnPoints.Length == 0) Debug.LogWarning($"[BossSummoner] No EnemySpawnPoint children: {name}", this);

        if (flashlight == null) flashlight = FindFirstObjectByType<FlashlightDetector>();
        if (flashlight == null) Debug.LogWarning($"[BossSummoner] No FlashlightDetector in scene, summoning ignores light: {name}", this);
    }

    private void OnEnable()
    {
        health.Died += KillAllMinions;
    }

    private void OnDisable()
    {
        health.Died -= KillAllMinions;
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
        if (aliveMinions.Count + effectSpawner.PendingCount >= maxAlive) return;

        candidates.Clear();
        foreach (EnemySpawnPoint point in spawnPoints)
        {
            // optionally keep minions from popping up inside the beam
            if (avoidLitPoints && flashlight != null && flashlight.IsPointLit(point.Position)) continue;
            candidates.Add(point);
        }
        if (candidates.Count == 0) return;

        int count = Mathf.Min(summonCount, maxAlive - (aliveMinions.Count + effectSpawner.PendingCount), candidates.Count);
        int summoned = 0;
        for (int i = 0; i < count; i++)
        {
            EnemyBase prefab = PickEnemy(IsPhaseTwo ? phaseTwoEnemies : phaseOneEnemies);
            if (prefab == null) break;

            // removed so two minions never share a point in the same summon
            int index = Random.Range(0, candidates.Count);
            EnemySpawnPoint chosen = candidates[index];
            candidates.RemoveAt(index);
            effectSpawner.Spawn(prefab, chosen.Position, minion => aliveMinions.Add(minion.gameObject));
            summoned++;
        }
        // fired once per summon with the smoke, not when the minions appear
        if (summoned > 0) Summoned?.Invoke();
    }

    // Light damage so armor-like Health subclasses can't block it and each minion plays its own death
    private void KillAllMinions()
    {
        // minions still behind their smoke would appear after the boss is dead
        effectSpawner.CancelAll();

        foreach (GameObject minion in aliveMinions)
        {
            if (minion == null) continue;
            Health minionHealth = minion.GetComponent<Health>();
            if (minionHealth != null) minionHealth.TakeLightDamage(minionHealth.CurrentHp);
        }
        aliveMinions.Clear();
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
