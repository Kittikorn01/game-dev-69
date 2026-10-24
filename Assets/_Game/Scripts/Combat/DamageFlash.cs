using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageFlash : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to use the SpriteRenderer on this GameObject.")]
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float duration = 0.1f;

    private Health health;
    private Color originalColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (target == null) target = GetComponent<SpriteRenderer>();
        if (target != null) originalColor = target.color;
    }

    private void OnEnable()
    {
        health.HealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        health.HealthChanged -= OnHealthChanged;
        if (target != null) target.color = originalColor;
        flashRoutine = null;
    }

    // shortcut: flashes on every HealthChanged, check that HP went down once healing exists
    private void OnHealthChanged(int currentHp, int maxHp)
    {
        if (target == null) return;

        // Restart so a hit during a flash never captures the flash color as the original
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        target.color = flashColor;
        // Realtime so the flash still ends while the game is paused on death
        yield return new WaitForSecondsRealtime(duration);
        target.color = originalColor;
        flashRoutine = null;
    }
}
