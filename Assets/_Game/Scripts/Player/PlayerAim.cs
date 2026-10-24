using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform aimPivot;
    // pivot children face up by default; change to 0 if they face right
    [SerializeField] private float spriteAngleOffset = -90f;

    public Vector2 AimDirection { get; private set; } = Vector2.right;

    // Update, not FixedUpdate: the pivot is a plain transform, so rotate it every rendered frame
    private void Update()
    {
        Vector2 mouseWorld = targetCamera.ScreenToWorldPoint(inputReader.PointerScreenPosition);
        Vector2 direction = mouseWorld - (Vector2)aimPivot.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        AimDirection = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        aimPivot.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
    }
}
