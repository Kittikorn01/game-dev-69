using UnityEngine;

// Plays Idle / Walk / Attack / Death facing left or right, like PlayerBodyAnimator.
[RequireComponent(typeof(Animator), typeof(EnemyBase))]
public class EnemyBodyAnimator : MonoBehaviour
{
    [Tooltip("Length of the attack clip; Walk resumes after it")]
    [SerializeField, Min(0f)] private float attackDuration = 0.33f;
    [Tooltip("Horizontal speed needed to turn around, so separation jitter doesn't flip the sprite")]
    [SerializeField, Min(0f)] private float turnSpeedThreshold = 0.1f;
    [Tooltip("Speed needed to play Walk instead of Idle")]
    [SerializeField, Min(0f)] private float moveSpeedThreshold = 0.1f;

    private Animator animator;
    private EnemyBase enemy;
    private Rigidbody2D body;
    private Health health;
    private string currentState;
    private bool facingLeft;
    private bool isDead;
    private float attackEndTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<EnemyBase>();
        body = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        enemy.Attacked += OnAttacked;
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        enemy.Attacked -= OnAttacked;
        health.Died -= OnDied;
    }

    private void Update()
    {
        if (isDead) return;

        Vector2 velocity = body.linearVelocity;
        if (Mathf.Abs(velocity.x) > turnSpeedThreshold) facingLeft = velocity.x < 0f;

        if (Time.time < attackEndTime) PlayState("Attack");
        else PlayState(velocity.sqrMagnitude > moveSpeedThreshold * moveSpeedThreshold ? "Walk" : "Idle");
    }

    private void OnAttacked()
    {
        attackEndTime = Time.time + attackDuration;
        // restart the swing even if the previous one is still playing
        currentState = null;
    }

    private void OnDied()
    {
        isDead = true;
        PlayState("Death");
    }

    // State names in Crawler.controller: Idle_Left, Idle_Right, Walk_Left, Attack_Left, ...
    private void PlayState(string action)
    {
        string state = action + (facingLeft ? "_Left" : "_Right");
        if (state == currentState) return;

        currentState = state;
        animator.Play(state, 0, 0f);
    }
}
