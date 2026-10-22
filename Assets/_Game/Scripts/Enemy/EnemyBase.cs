using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackCooldown = 1f;

    protected Transform player;
    protected Rigidbody2D body;
    protected Health health;

    private float nextAttackTime;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
        else Debug.LogWarning($"{name}: no GameObject tagged 'Player' found", this);
    }

    protected virtual void OnEnable()
    {
        health.Died += OnDied;
    }

    protected virtual void OnDisable()
    {
        health.Died -= OnDied;
    }

    protected virtual void FixedUpdate()
    {
        if (player == null) return;

        if (Vector2.Distance(body.position, player.position) > attackRange)
        {
            Move();
            return;
        }

        body.linearVelocity = Vector2.zero;
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            Attack();
        }
    }

    protected abstract void Move();
    protected abstract void Attack();

    protected virtual void OnDied()
    {
        Destroy(gameObject);
    }
}
