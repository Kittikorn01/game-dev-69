using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class AmmoPickup : MonoBehaviour
{
    [SerializeField, Min(1)] private int amount = 10;
    private bool collected;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || amount <= 0) return;
        PlayerAmmo ammo = other.GetComponentInParent<PlayerAmmo>();
        if (ammo == null || !ammo.isActiveAndEnabled) return;

        // Mark first: multiple player colliders must not collect twice.
        collected = true;
        ammo.Add(amount);
        Destroy(gameObject);
    }
}
