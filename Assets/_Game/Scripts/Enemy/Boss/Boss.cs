using UnityEngine;

// Stationary boss: killing it wins the game.
[RequireComponent(typeof(BossHealth))]
public class Boss : MonoBehaviour
{
    [Tooltip("Seconds between death and the win screen, so the death animation can finish")]
    [SerializeField, Min(0f)] private float defeatDelay = 0.6f;

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
        Invoke(nameof(ReportDefeat), defeatDelay);
    }

    private void ReportDefeat()
    {
        if (GameManager.Instance != null) GameManager.Instance.BossDefeated();
        else Debug.LogWarning($"[Boss] No GameManager in scene: {name}", this);
    }
}
