using System.Linq;
using UnityEngine;

// Plays Idle / Attack / Death facing the player; Attack plays on every summon.
[RequireComponent(typeof(BossSummoner), typeof(Health))]
public class BossBodyAnimator : MonoBehaviour
{
    // on the Visual child, so it can't be fetched from this GameObject
    [SerializeField] private Animator animator;

    private BossSummoner summoner;
    private Health health;
    private Transform player;
    private string currentState;
    private bool facingLeft;
    private bool isDead;
    private float attackEndTime;
    private float attackClipLength;

    private void Awake()
    {
        summoner = GetComponent<BossSummoner>();
        health = GetComponent<Health>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
        else Debug.LogWarning($"[BossBodyAnimator] No GameObject tagged 'Player' found: {name}", this);

        if (animator == null)
        {
            Debug.LogWarning($"[BossBodyAnimator] Animator not set: {name}", this);
            enabled = false;
            return;
        }

        // shortcut: finds the attack clip by name, switch to a serialized clip if a name lacks "Attack"
        attackClipLength = animator.runtimeAnimatorController.animationClips
            .Where(clip => clip.name.Contains("Attack"))
            .Select(clip => clip.length)
            .DefaultIfEmpty(0f)
            .Max();
    }

    private void OnEnable()
    {
        summoner.Summoned += OnSummoned;
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        summoner.Summoned -= OnSummoned;
        health.Died -= OnDied;
    }

    private void Update()
    {
        if (isDead) return;

        if (player != null) facingLeft = player.position.x < transform.position.x;
        PlayState(Time.time < attackEndTime ? "Attack" : "Idle");
    }

    private void OnSummoned()
    {
        attackEndTime = Time.time + attackClipLength;
        // restart the swing even if the previous one is still playing
        currentState = null;
    }

    private void OnDied()
    {
        isDead = true;
        PlayState("Death");
    }

    // State names in SuperBoss.controller: Idle_Left, Idle_Right, Attack_Left, Death_Right, ...
    private void PlayState(string action)
    {
        string state = action + (facingLeft ? "_Left" : "_Right");
        if (state == currentState) return;

        currentState = state;
        animator.Play(state, 0, 0f);
    }
}
