using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class BatteryPickup : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float amount = 25f;
    private bool collected;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCollect(other);
    }

    // If battery was full on entry, allow collection once it drains while
    // the player is still standing on this item.
    private void OnTriggerStay2D(Collider2D other)
    {
        TryCollect(other);
    }

    private void TryCollect(Collider2D other)
    {
        if (collected || amount <= 0f) return;
        PlayerBattery battery = other.GetComponentInParent<PlayerBattery>();
        if (battery == null || !battery.isActiveAndEnabled) return;
        if (battery.Current >= battery.Max) return;

        collected = true;
        battery.Add(amount);
        Destroy(gameObject);
    }
}
