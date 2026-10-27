using System.Collections;
using UnityEngine;

// Flashes the sprite solid white when Health blocks a hit, so the player sees the shot landed but did nothing.
[RequireComponent(typeof(Health))]
public class BlockedHitFlash : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to use the SpriteRenderer on this GameObject.")]
    [SerializeField] private SpriteRenderer target;
    // a solid-color shader such as GUI/Text Shader; a color tint can't turn a white sprite whiter
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration = 0.1f;

    private Health health;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (target == null) target = GetComponent<SpriteRenderer>();
        if (target != null) originalMaterial = target.sharedMaterial;
        if (target == null || flashMaterial == null) Debug.LogWarning($"[BlockedHitFlash] SpriteRenderer or Flash Material not set: {name}", this);
    }

    private void OnEnable()
    {
        health.DamageBlocked += OnDamageBlocked;
    }

    private void OnDisable()
    {
        health.DamageBlocked -= OnDamageBlocked;
        if (target != null) target.sharedMaterial = originalMaterial;
        flashRoutine = null;
    }

    private void OnDamageBlocked()
    {
        if (target == null || flashMaterial == null) return;

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        target.sharedMaterial = flashMaterial;
        // Realtime so the flash still ends while the game is paused on death
        yield return new WaitForSecondsRealtime(duration);
        target.sharedMaterial = originalMaterial;
        flashRoutine = null;
    }
}
