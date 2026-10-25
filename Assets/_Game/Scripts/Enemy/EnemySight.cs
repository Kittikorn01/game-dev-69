using UnityEngine;

// Tells whether this enemy can see the player and remembers where it last did.
public class EnemySight : MonoBehaviour
{
    [SerializeField, Min(0f)] private float sightRange = 8f;
    // colliders on these layers block sight; triggers never do
    [SerializeField] private LayerMask obstacleMask = 1;

    private readonly RaycastHit2D[] hits = new RaycastHit2D[1];
    private ContactFilter2D obstacleFilter;

    public bool HasLastSeenPosition { get; private set; }
    public Vector2 LastSeenPosition { get; private set; }

    private void Awake()
    {
        obstacleFilter = new ContactFilter2D { useTriggers = false };
        obstacleFilter.SetLayerMask(obstacleMask);
    }

    public bool CanSee(Transform player)
    {
        bool seen = IsMapLit() || HasLineOfSight(player.position);
        if (seen)
        {
            LastSeenPosition = player.position;
            HasLastSeenPosition = true;
        }
        return seen;
    }

    public void ForgetLastSeenPosition()
    {
        HasLastSeenPosition = false;
    }

    // All generators on lights the map, so every enemy sees the player.
    // AllGeneratorsOn is also true with zero generators, hence the count check.
    private static bool IsMapLit()
    {
        GameManager gameManager = GameManager.Instance;
        return gameManager != null && gameManager.TotalGenerators > 0 && gameManager.AllGeneratorsOn;
    }

    private bool HasLineOfSight(Vector2 target)
    {
        Vector2 origin = transform.position;
        if ((target - origin).sqrMagnitude > sightRange * sightRange) return false;
        return Physics2D.Linecast(origin, target, obstacleFilter, hits) == 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
