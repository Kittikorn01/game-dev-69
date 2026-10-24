using UnityEngine;

// Chases the player and attacks in range. Uses the existing EnemyBase and Health unchanged.
[DisallowMultipleComponent]
public class BasicChaseEnemy : EnemyBase
{
    [Tooltip("Optional. Leave empty to find the object tagged Player.")]
    [SerializeField] private Transform target;

    protected override void Awake()
    {
        base.Awake();
        if (target != null) player = target;
        body.gravityScale = 0f;
        body.freezeRotation = true;
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
        Vector2 offset = (Vector2)player.position - body.position;
        // Avoid overshooting the stopping distance during a physics step.
        float remaining = Mathf.Max(0f, offset.magnitude - Mathf.Max(0f, attackRange));
        float speed = Mathf.Min(Mathf.Max(0f, moveSpeed), remaining / Time.fixedDeltaTime);
        body.linearVelocity = offset.normalized * speed;
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
