using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerBodyAnimator : MonoBehaviour
{
    [SerializeField] private PlayerAim aim;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Health health;
    [SerializeField] private float runSpeedThreshold = 0.1f;

    private Animator animator;
    private string currentState;
    private bool isDead;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        health.Died -= OnDied;
    }

    private void Update()
    {
        // Stay on the death state even if the mouse crosses sides afterwards
        if (isDead) return;

        bool isRunning = body.linearVelocity.sqrMagnitude > runSpeedThreshold * runSpeedThreshold;
        PlayState(isRunning ? "Run" : "Idle");
    }

    private void OnDied()
    {
        isDead = true;
        PlayState("Death");
    }

    // State names in PlayerBody.controller: Idle_Left, Idle_Right, Run_Left, ...
    private void PlayState(string action)
    {
        string state = action + (aim.AimDirection.x < 0f ? "_Left" : "_Right");
        if (state == currentState) return;

        currentState = state;
        animator.Play(state);
    }
}
