using UnityEngine;

// Stationary boss: killing it wins the game.
[RequireComponent(typeof(BossHealth))]
public class Boss : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<BossHealth>();
    }

    private void OnEnable()
    {
        health.Died += HandleDied;
    }

    private void OnDisable()
    {
        health.Died -= HandleDied;
    }

    private void HandleDied()
    {
        Debug.Log($"[Boss] Defeated: {name}");
        if (GameManager.Instance != null) GameManager.Instance.BossDefeated();
        else Debug.LogWarning($"[Boss] No GameManager in scene: {name}", this);
    }
}
