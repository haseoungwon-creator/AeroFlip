using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerMode playerMode;
    [SerializeField] DimensionTransition dimensionTransition;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerMovement3D playerMovement3D;
    [SerializeField] PlayerMovement2D playerMovement2D;

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
        else if (playerMode.IsMode2D())
            dimensionTransition.TransitionTo3D();
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        Debug.Log($"PlayerController OnMousePosition / Phase: {context.phase}");

        if (!canControl || playerMode == null || !playerMode.Is3D())
            return;

        playerMovement3D.OnMousePosition(context);
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!canControl || playerMode == null || !playerMode.Is3D())
            return;

        playerMovement3D.OnMouseClick(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!canControl || playerMode == null || !playerMode.IsMode2D())
            return;

        playerMovement2D.OnMove(context);
    }

    public void SetControlEnabled(bool value)
    {
        canControl = value;

        if (playerMode.Is3D())
            playerMovement3D.SetControlEnabled(value);
        else if (playerMode.IsMode2D())
            playerMovement2D.SetControlEnabled(value);
    }

    public void Set3DInput()
    {
        playerInput.actions.FindActionMap("Player3D").Enable();
        playerInput.actions.FindActionMap("Player2D").Disable();
        playerMovement2D.SetControlEnabled(false);
        playerMovement3D.SetControlEnabled(true);
    }

    public void Set2DInput()
    {
        playerInput.actions.FindActionMap("Player3D").Disable();
        playerInput.actions.FindActionMap("Player2D").Enable();
        playerMovement3D.SetControlEnabled(false);
        playerMovement2D.SetControlEnabled(true);
    }
}