using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Player stands in the trigger and holds Interact (E) to turn the generator on.
// Needs a Collider2D with "Is Trigger" checked on the same GameObject.
public class Generator : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField, Min(0.1f)] private float holdDuration = 2f;
    // true = progress goes back to 0 when E is released or the player walks away
    [SerializeField] private bool resetWhenReleased = false;

    [Header("Sprites (optional)")]
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    [Header("Visuals (optional)")]
    // shown while the player is in range and the generator is still off, e.g. "Hold E"
    [SerializeField] private GameObject prompt;
    // scaled on X from 0 to 1 while holding
    [SerializeField] private Transform progressFill;
    // turned on once the generator is running, e.g. a light or a "running" sprite
    [SerializeField] private GameObject runningVisual;

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startClip;
    // looped after the generator starts
    [SerializeField] private AudioClip runningLoopClip;

    public event Action<Generator> Activated;

    public bool IsActivated { get; private set; }
    // true while the player is in range and holding Interact
    public bool IsHolding { get; private set; }
    public float Progress => Mathf.Clamp01(holdTime / holdDuration);

    private float holdTime;
    private int playerColliders;

    private bool PlayerInRange => playerColliders > 0;

    private void Start()
    {
        if (runningVisual != null) runningVisual.SetActive(false);
        SetBodySprite(offSprite);
        RefreshVisuals();

        // Start, not OnEnable: GameManager.Instance is only set after every Awake
        if (GameManager.Instance != null) GameManager.Instance.RegisterGenerator();
        else Debug.LogWarning($"[Generator] No GameManager in scene: {name}");
    }

    private void OnEnable()
    {
        if (interactAction == null)
        {
            Debug.LogError($"[Generator] Interact Action not set: {name}");
            enabled = false;
            return;
        }
        interactAction.action.Enable();
    }

    private void Update()
    {
        if (IsActivated) return;

        bool holding = PlayerInRange && interactAction.action.IsPressed();
        IsHolding = holding;
        if (holding)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= holdDuration) Activate();
        }
        else if (resetWhenReleased)
        {
            holdTime = 0f;
        }

        RefreshVisuals();
    }

    private void Activate()
    {
        IsActivated = true;
        // Update returns early once activated, so clear it here
        IsHolding = false;
        holdTime = holdDuration;

        if (runningVisual != null) runningVisual.SetActive(true);
        SetBodySprite(onSprite);

        if (audioSource != null)
        {
            if (startClip != null) audioSource.PlayOneShot(startClip);
            if (runningLoopClip != null)
            {
                audioSource.clip = runningLoopClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        Debug.Log($"Generator ON: {name}");
        if (GameManager.Instance != null) GameManager.Instance.ActivateGenerator();
        Activated?.Invoke(this);
        RefreshVisuals();
    }

    private void SetBodySprite(Sprite sprite)
    {
        if (body != null && sprite != null) body.sprite = sprite;
    }

    private void RefreshVisuals()
    {
        if (prompt != null) prompt.SetActive(PlayerInRange && !IsActivated);

        if (progressFill != null)
        {
            Vector3 scale = progressFill.localScale;
            scale.x = Progress;
            progressFill.localScale = scale;
            progressFill.gameObject.SetActive(!IsActivated && Progress > 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayer(other)) playerColliders++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        playerColliders = Mathf.Max(0, playerColliders - 1);
        if (!PlayerInRange && resetWhenReleased && !IsActivated) holdTime = 0f;
    }

    // the player may have extra child colliders, so also check the Rigidbody2D
    private static bool IsPlayer(Collider2D other)
    {
        if (other.CompareTag("Player")) return true;
        return other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player");
    }
}
