using System;
using System.Collections;
using UnityEngine;

// Plays a spawn effect at a position, then spawns the enemy after a delay.
public class EffectSpawner : MonoBehaviour
{
    // optional, played at the spawn point before the enemy appears
    [SerializeField] private SpawnEffect spawnEffectPrefab;
    // seconds between the effect and the enemy, 0.33 = smoke at its biggest
    [SerializeField, Min(0f)] private float spawnDelay = 0.33f;

    // enemies waiting for spawnDelay, callers count them toward their own limit
    public int PendingCount { get; private set; }

    public void Spawn(EnemyBase prefab, Vector2 position, Action<EnemyBase> onSpawned)
    {
        StartCoroutine(SpawnAfterEffect(prefab, position, onSpawned));
    }

    // drops every enemy still waiting for its delay, the effect already played stays
    public void CancelAll()
    {
        StopAllCoroutines();
        PendingCount = 0;
    }

    // the enemy still comes out if the flashlight hits the point during the delay
    private IEnumerator SpawnAfterEffect(EnemyBase prefab, Vector2 position, Action<EnemyBase> onSpawned)
    {
        if (spawnEffectPrefab != null) Instantiate(spawnEffectPrefab, position, Quaternion.identity);

        PendingCount++;
        yield return new WaitForSeconds(spawnDelay);
        PendingCount--;

        EnemyBase enemy = Instantiate(prefab, position, Quaternion.identity);
        onSpawned?.Invoke(enemy);
    }
}
