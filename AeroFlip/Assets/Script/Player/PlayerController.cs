using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerMode playerMode;
    [SerializeField] DimensionTransition dimensionTransition;

    private bool canControl = true;

    public bool CanControl => canControl;

    public void OnDimension(InputAction.CallbackContext context)
    {
        if (!canControl || !context.performed) return;

        if (playerMode.Is3D())
        {
            dimensionTransition.TransitionTo2D();
        }
        else
        {
            //dimensionTransition.TransitionTo3D();
        }
    }

    public void dOnSkill(InputAction.CallbackContext context)
    {
        if (!canControl || !context.performed) return;


    }

    public void SetControlEnabled(bool value)
    {
        canControl = value;
    }
}
