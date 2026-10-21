using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;

    public Vector2 MoveInput => moveAction.action.ReadValue<Vector2>();

    // shortcut: mouse only, add a gamepad aim action when gamepad support is needed
    public Vector2 PointerScreenPosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }
}
