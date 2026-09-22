using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerMode playerMode;
    [SerializeField] DimensionTransition dimensionTransition;
    [SerializeField] PlayerInput playerInput;

    private bool canControl = true;

    public bool CanControl => canControl;

    private void Awake()
    {
        playerInput.actions.FindActionMap("System").Enable();
        playerInput.actions.FindActionMap("Player3D").Enable();
        playerInput.actions.FindActionMap("Player2D").Disable();
    }

    public void OnShift(InputAction.CallbackContext context)
    {
        if (!canControl || !context.performed)
            return;

        if (playerMode.Is3D())
            dimensionTransition.TransitionTo2D();
    }

    public void SetControlEnabled(bool value)
    {
        canControl = value;
    }

    public void Set3DInput()
    {
        playerInput.actions.FindActionMap("Player3D").Enable();
        playerInput.actions.FindActionMap("Player2D").Disable();
    }

    public void Set2DInput()
    {
        playerInput.actions.FindActionMap("Player3D").Disable();
        playerInput.actions.FindActionMap("Player2D").Enable();
    }
}
