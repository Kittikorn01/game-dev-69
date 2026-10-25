using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerGunVisual : MonoBehaviour
{
    private static readonly int ShootHash = Animator.StringToHash("Shoot");

    [SerializeField] private PlayerAim aim;
    [SerializeField] private PlayerShooter shooter;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        shooter.Fired += PlayShoot;
    }

    private void OnDisable()
    {
        shooter.Fired -= PlayShoot;
    }

    private void LateUpdate()
    {
        // Gun art faces right, so mirror it when aiming left or it renders upside down.
        // Scale (not flipY) so the muzzle children (FirePoint, flash) mirror with it
        Vector3 scale = transform.localScale;
        scale.y = aim.AimDirection.x < 0f ? -1f : 1f;
        transform.localScale = scale;
    }

    private void PlayShoot()
    {
        animator.SetTrigger(ShootHash);
    }
}
