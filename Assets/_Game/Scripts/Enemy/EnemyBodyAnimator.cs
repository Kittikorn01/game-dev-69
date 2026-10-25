using System.Linq;
using UnityEngine;

// Plays Idle / Walk / Attack / Death facing left or right, like PlayerBodyAnimator.
[RequireComponent(typeof(Animator), typeof(EnemyBase))]
public class EnemyBodyAnimator : MonoBehaviour
{
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
    private float attackClipLength;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<EnemyBase>();
        body = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();

        // shortcut: finds the attack clip by name, switch to a serialized clip if a name lacks "Attack"
        attackClipLength = animator.runtimeAnimatorController.animationClips
            .Where(clip => clip.name.Contains("Attack"))
            .Select(clip => clip.length)
            .DefaultIfEmpty(0f)
            .Max();
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

        if (Time.time < attackEndTime)
        {
            PlayState("Attack");
            return;
        }

        animator.speed = 1f;
        PlayState(velocity.sqrMagnitude > moveSpeedThreshold * moveSpeedThreshold ? "Walk" : "Idle");
    }

    private void OnAttacked()
    {
        // Speed the swing up when it is longer than the cooldown, so it always finishes before the next one
        float duration = Mathf.Min(attackClipLength, enemy.AttackCooldown);
        animator.speed = duration > 0f ? attackClipLength / duration : 1f;
        attackEndTime = Time.time + duration;
        // restart the swing even if the previous one is still playing
        currentState = null;
    }

    private void OnDied()
    {
        isDead = true;
        animator.speed = 1f;
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
