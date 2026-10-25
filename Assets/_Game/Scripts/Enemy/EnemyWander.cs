using UnityEngine;

// Picks random walk directions and notices when the enemy stops making progress.
// After getting stuck it detours in a new random direction for a while.
public class EnemyWander : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float changeDirectionInterval = 3f;
    [SerializeField, Min(0.1f)] private float stuckTime = 1f;
    // moving less than this within stuckTime counts as stuck
    [SerializeField, Min(0f)] private float stuckDistance = 0.1f;
    [SerializeField, Min(0f)] private float detourTime = 1f;

    private Vector2 direction;
    private float directionTimer;
    private Vector2 progressPosition;
    private float stuckTimer;
    private float detourTimer;

    public Vector2 Direction => direction;
    public bool IsDetouring => detourTimer > 0f;

    private void OnEnable()
    {
        progressPosition = transform.position;
        stuckTimer = 0f;
        detourTimer = 0f;
        PickNewDirection();
    }

    // Call once per physics step while the enemy is trying to move.
    public void Tick(Vector2 position, float deltaTime)
    {
        detourTimer -= deltaTime;
        directionTimer -= deltaTime;
        if (directionTimer <= 0f) PickNewDirection();

        if ((position - progressPosition).sqrMagnitude >= stuckDistance * stuckDistance)
        {
            progressPosition = position;
            stuckTimer = 0f;
            return;
        }

        stuckTimer += deltaTime;
        if (stuckTimer < stuckTime) return;

        stuckTimer = 0f;
        detourTimer = detourTime;
        PickNewDirection();
    }

    private void PickNewDirection()
    {
        direction = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) * Vector2.right;
        directionTimer = changeDirectionInterval;
    }
}
