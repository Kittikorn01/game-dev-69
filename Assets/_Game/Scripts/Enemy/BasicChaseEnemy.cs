using UnityEngine;

// Chases the player while it is in sight, then checks where it was last seen, otherwise wanders.
// Attacks in range. Uses the existing EnemyBase and Health unchanged.
[DisallowMultipleComponent]
public class BasicChaseEnemy : EnemyBase
{
    // close enough to the last seen position to stop searching there
    private const float ArriveDistance = 0.1f;

    [Tooltip("Optional. Leave empty to find the object tagged Player.")]
    [SerializeField] private Transform target;

    private EnemySight sight;
    private EnemyWander wander;
    private EnemySeparation separation;

    protected override void Awake()
    {
        base.Awake();
        if (target != null) player = target;
        body.gravityScale = 0f;
        body.freezeRotation = true;

        // older prefabs that are not Enemy_Base variants lack these, so they get the defaults
        sight = GetComponent<EnemySight>();
        if (sight == null) sight = gameObject.AddComponent<EnemySight>();
        wander = GetComponent<EnemyWander>();
        if (wander == null) wander = gameObject.AddComponent<EnemyWander>();
        separation = GetComponent<EnemySeparation>();
        if (separation == null) separation = gameObject.AddComponent<EnemySeparation>();
    }

    protected override void FixedUpdate()
    {
        if (player == null || !player.gameObject.activeInHierarchy || health.CurrentHp <= 0)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }
        base.FixedUpdate();
    }

    protected override void Move()
    {
        body.linearVelocity = GetMoveVelocity() + separation.GetPush(body);
    }

    // keep spreading out while attacking, since enemies do not collide with each other
    protected override Vector2 GetInRangeVelocity()
    {
        return separation.GetPush(body);
    }

    private Vector2 GetMoveVelocity()
    {
        wander.Tick(body.position, Time.fixedDeltaTime);
        if (wander.IsDetouring)
        {
            // stuck on the way, so the last seen position may be unreachable
            sight.ForgetLastSeenPosition();
            return wander.Direction * Mathf.Max(0f, moveSpeed);
        }

        if (sight.CanSee(player)) return VelocityToward(player.position, attackRange);

        if (sight.HasLastSeenPosition)
        {
            if (Vector2.Distance(body.position, sight.LastSeenPosition) > ArriveDistance)
            {
                return VelocityToward(sight.LastSeenPosition, 0f);
            }
            sight.ForgetLastSeenPosition();
        }

        return wander.Direction * Mathf.Max(0f, moveSpeed);
    }

    private Vector2 VelocityToward(Vector2 destination, float stopDistance)
    {
        Vector2 offset = destination - body.position;
        // Avoid overshooting the stopping distance during a physics step.
        float remaining = Mathf.Max(0f, offset.magnitude - Mathf.Max(0f, stopDistance));
        float speed = Mathf.Min(Mathf.Max(0f, moveSpeed), remaining / Time.fixedDeltaTime);
        return offset.normalized * speed;
    }

    // Range attack like DemoEnemy: EnemyBase calls this once per cooldown inside attackRange.
    protected override void Attack()
    {
        player.GetComponent<IDamageable>()?.TakeDamage(damage);
    }

    protected override void OnDisable()
    {
        body.linearVelocity = Vector2.zero;
        base.OnDisable();
    }
}
