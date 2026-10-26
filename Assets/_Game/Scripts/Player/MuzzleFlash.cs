using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private PlayerShooter shooter;
    [SerializeField] private Light2D flashLight;
    [SerializeField] private float duration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float hideTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        SetVisible(false);
    }

    private void OnEnable()
    {
        shooter.Fired += Show;
    }

    private void OnDisable()
    {
        shooter.Fired -= Show;
    }

    private void Update()
    {
        if (spriteRenderer.enabled && Time.time >= hideTime) SetVisible(false);
    }

    private void Show()
    {
        hideTime = Time.time + duration;
        SetVisible(true);
        // Restart from frame 0 so rapid shots each show the full flash
        animator.Play(0, 0, 0f);
    }

    private void SetVisible(bool visible)
    {
        spriteRenderer.enabled = visible;
        flashLight.enabled = visible;
    }
}
