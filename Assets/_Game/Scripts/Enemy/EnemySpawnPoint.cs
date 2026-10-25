using System;
using UnityEngine;

// A place where EnemySpawner may spawn enemies. Put it as a child of the EnemySpawner.
public class EnemySpawnPoint : MonoBehaviour
{
    // drag prefabs from Prefabs/Enemy/Ready
    [SerializeField] private EnemyBase[] allowedEnemies;

    public Vector2 Position => transform.position;

    public bool Allows(EnemyBase enemy)
    {
        return enemy != null && Array.IndexOf(allowedEnemies, enemy) >= 0;
    }

    public EnemyBase GetRandomEnemy()
    {
        if (allowedEnemies.Length == 0) return null;
        return allowedEnemies[UnityEngine.Random.Range(0, allowedEnemies.Length)];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
