using UnityEngine;

// Kills every enemy in the scene once all generators are on, so the boss fight starts clean.
public class EnemyWipe : MonoBehaviour
{
    // Start, not OnEnable: GameManager.Instance is only set after every Awake
    private void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.AllGeneratorsActivated += KillAll;
        else Debug.LogWarning($"[EnemyWipe] No GameManager in scene: {name}", this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.AllGeneratorsActivated -= KillAll;
    }

    // Light damage so Health subclasses can't block it and each enemy plays its own death
    private void KillAll()
    {
        foreach (EnemyBase enemy in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            Health health = enemy.GetComponent<Health>();
            health.TakeLightDamage(health.CurrentHp);
        }
        Debug.Log("[EnemyWipe] All generators on, every enemy killed");
    }
}
