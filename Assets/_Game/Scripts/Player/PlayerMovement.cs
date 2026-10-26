using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private PlayerInputReader inputReader;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        // Stop sliding when disabled on death
        body.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        // ClampMagnitude stops diagonal movement from being faster
        body.linearVelocity = Vector2.ClampMagnitude(inputReader.MoveInput, 1f) * moveSpeed;
    }
}
