using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlight : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float verticalMultiplier = 2f;
    [SerializeField] private float moveSmooth = 15f;

    [Header("Click Movement")]
    [SerializeField] private float clickMoveMultiplier = 0.2f;

    [Header("Movement Limit")]
    [SerializeField] private float minX = -40f;
    [SerializeField] private float maxX = 40f;
    [SerializeField] private float minY = -50f;
    [SerializeField] private float maxY = 50f;

    [Header("Rotation")]
    [SerializeField] private float maxPitch = 10f;
    [SerializeField] private float maxRoll = 18f;
    [SerializeField] private float clickMaxRoll = 200f;
    [SerializeField] private float rotationSmooth = 5f;

    [Header("Mouse")]
    [SerializeField] private float inputSensitivity = 1.5f;
    [SerializeField] private float inputSmooth = 8f;

    private Transform _t;
    private Vector2 currentInput;
    private Vector3 targetPosition;
    private float rawMouseX;
    private bool clickHeld;

    private void Awake()
    {
        _t = transform;
    }

    private void Start()
    {
        targetPosition = _t.position;
    }

    private void Update()
    {
        ReadMouse();
        Move();
        Rotate();
    }

    private void ReadMouse()
    {
        clickHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 input = new Vector2((mousePos.x - center.x) / center.x, (mousePos.y - center.y) / center.y) * inputSensitivity;

        rawMouseX = Mathf.Clamp(input.x, -1f, 1f);
        input.x = rawMouseX;
        input.y = Mathf.Clamp(input.y, -1f, 1f);

        currentInput = Vector2.Lerp(currentInput, input, inputSmooth * Time.deltaTime);
    }

    private void Move()
    {
        Vector3 right = _t.right;
        Vector3 up = _t.up;
        right.z = 0f;
        up.z = 0f;

        if (right.sqrMagnitude > 0.001f)
            right.Normalize();

        if (up.sqrMagnitude > 0.001f)
            up.Normalize();

        float moveMultiplier = 1f;
        if (clickHeld)
            moveMultiplier = clickMoveMultiplier;

        Vector3 movement = right * currentInput.x * verticalMultiplier + up * currentInput.y * verticalMultiplier;
        targetPosition += movement * moveSpeed * moveMultiplier * Time.deltaTime;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        _t.position = Vector3.Lerp(_t.position, targetPosition, moveSmooth * Time.deltaTime);
    }

    private void Rotate()
    {
        float targetPitch = -currentInput.y * maxPitch;

        float rollLimit = maxRoll;
        if (clickHeld)
            rollLimit = clickMaxRoll;

        Quaternion targetRotation = Quaternion.Euler(targetPitch, 0f, -rawMouseX * rollLimit);
        _t.rotation = Quaternion.Slerp(_t.rotation, targetRotation, rotationSmooth * Time.deltaTime);
    }
}