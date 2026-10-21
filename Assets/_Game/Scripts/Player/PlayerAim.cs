using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAim : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Camera targetCamera;
    // sprite art faces up by default; change to 0 if the art faces right
    [SerializeField] private float spriteAngleOffset = -90f;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 mouseWorld = targetCamera.ScreenToWorldPoint(inputReader.PointerScreenPosition);
        Vector2 direction = mouseWorld - body.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        body.MoveRotation(angle + spriteAngleOffset);
    }
}
