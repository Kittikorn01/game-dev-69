using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference attackAction;

    public event Action AttackPressed;

    public Vector2 MoveInput => moveAction.action.ReadValue<Vector2>();

    // shortcut: mouse only, add a gamepad aim action when gamepad support is needed
    public Vector2 PointerScreenPosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;

    private void OnEnable()
    {
        moveAction.action.Enable();
        attackAction.action.performed += OnAttackPerformed;
        attackAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        attackAction.action.performed -= OnAttackPerformed;
        attackAction.action.Disable();
    }

    // Button action fires performed once per press, so holding does not auto-fire
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        AttackPressed?.Invoke();
    }
}
