using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerGunVisual : MonoBehaviour
{
    [SerializeField] private PlayerAim aim;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // Gun art faces right, so flip it when aiming left or it renders upside down
        spriteRenderer.flipY = aim.AimDirection.x < 0f;
    }
}
