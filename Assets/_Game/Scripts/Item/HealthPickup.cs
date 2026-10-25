using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class HealthPickup : MonoBehaviour
{
    [SerializeField, Min(1)] private int amount = 2;
    private bool collected;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCollect(other);
    }

    // If HP was full on entry, allow collection once the player takes damage
    // while still standing on this item.
    private void OnTriggerStay2D(Collider2D other)
    {
        TryCollect(other);
    }

    private void TryCollect(Collider2D other)
    {
        if (collected || amount <= 0) return;
        // Enemies have Health too; only the player may collect
        if (other.GetComponentInParent<PlayerMovement>() == null) return;
        Health health = other.GetComponentInParent<Health>();
        if (health == null || !health.isActiveAndEnabled) return;
        if (health.CurrentHp <= 0 || health.CurrentHp >= health.MaxHp) return;

        collected = true;
        health.Heal(amount);
        Destroy(gameObject);
    }
}
