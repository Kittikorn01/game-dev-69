using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 2f;

    private bool hasHit;

    private void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.up * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Guard so two overlaps in one step deal damage once
        if (hasHit) return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        // Hurtboxes are triggers; pass through any other trigger such as pickups
        if (other.isTrigger && damageable == null) return;

        hasHit = true;
        damageable?.TakeDamage(damage);
        Destroy(gameObject);
    }
}
