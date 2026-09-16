using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public event Action ChangeModePressed;
    public event Action RewindPressed;
    public event Action TimeSlowPressed;

    public Vector2 Move2D { get; private set; }
    public Vector2 PointerPosition { get; private set; }
    public bool ClickHeld { get; private set; }

    InputAction move2DAction;
    InputAction changeModeAction;
    InputAction rewindAction;
    InputAction timeSlowAction;
    InputAction pointerPositionAction;
    InputAction clickAction;

    void Awake()
    {
        move2DAction = new InputAction("Move2D", InputActionType.Value, expectedControlType: "Vector2");
        move2DAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        changeModeAction = new InputAction("ChangeMode", InputActionType.Button, "<Keyboard>/leftShift");
        rewindAction = new InputAction("Rewind", InputActionType.Button, "<Keyboard>/q");
        timeSlowAction = new InputAction("TimeSlow", InputActionType.Button, "<Keyboard>/e");
        pointerPositionAction = new InputAction("PointerPosition", InputActionType.Value, "<Pointer>/position");
        clickAction = new InputAction("Click", InputActionType.Button, "<Mouse>/leftButton");

        changeModeAction.performed += _ => ChangeModePressed?.Invoke();
        rewindAction.performed += _ => RewindPressed?.Invoke();
        timeSlowAction.performed += _ => TimeSlowPressed?.Invoke();
    }

    void OnEnable()
    {
        move2DAction.Enable();
        changeModeAction.Enable();
        rewindAction.Enable();
        timeSlowAction.Enable();
        pointerPositionAction.Enable();
        clickAction.Enable();
    }

    void OnDisable()
    {
        move2DAction.Disable();
        changeModeAction.Disable();
        rewindAction.Disable();
        timeSlowAction.Disable();
        pointerPositionAction.Disable();
        clickAction.Disable();
    }

    void Update()
    {
        Move2D = move2DAction.ReadValue<Vector2>();
        PointerPosition = pointerPositionAction.ReadValue<Vector2>();
        ClickHeld = clickAction.IsPressed();
    }
}