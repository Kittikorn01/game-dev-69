using UnityEngine;

// Pushes this enemy away from nearby enemies so a group spreads out instead of shoving.
public class EnemySeparation : MonoBehaviour
{
    // centers closer than this push apart; enemy bodies are 1 unit wide
    [SerializeField, Min(0.1f)] private float radius = 1.2f;
    // push speed added on top of movement when fully overlapping
    [SerializeField, Min(0f)] private float strength = 2f;
    [SerializeField] private LayerMask enemyMask = 1 << 8;

    private readonly Collider2D[] neighbours = new Collider2D[8];
    private ContactFilter2D enemyFilter;

    private void Awake()
    {
        // hurtboxes are triggers, so only solid bodies count
        enemyFilter = new ContactFilter2D { useTriggers = false };
        enemyFilter.SetLayerMask(enemyMask);
    }

    public Vector2 GetPush(Rigidbody2D body)
    {
        Vector2 position = body.position;
        int count = Physics2D.OverlapCircle(position, radius, enemyFilter, neighbours);
        Vector2 push = Vector2.zero;

        for (int i = 0; i < count; i++)
        {
            if (neighbours[i].attachedRigidbody == body) continue;

            Vector2 away = position - (Vector2)neighbours[i].transform.position;
            float distance = away.magnitude;
            // exactly on top of each other has no direction, so pick one
            Vector2 direction = distance > 0.0001f ? away / distance : Random.insideUnitCircle.normalized;
            push += direction * (1f - Mathf.Clamp01(distance / radius));
        }

        return Vector2.ClampMagnitude(push, 1f) * strength;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
