using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlight : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private float forwardSpeed = 30f;
    [SerializeField] private float moveSpeed = 12f;

    [Header("Mouse Control")]
    [SerializeField] private float mouseSensitivity = 1.5f;
    [SerializeField] private float mouseSmooth = 8f;

    [Header("Rotation")]
    [SerializeField] private float pitchAmount = 25f;
    [SerializeField] private float yawAmount = 20f;
    [SerializeField] private float rollAmount = 35f;
    [SerializeField] private float rotationSmooth = 8f;

    private Rigidbody rb;

    private Vector2 targetInput;
    private Vector2 currentInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ReadMouseInput();
    }

    private void FixedUpdate()
    {
        SmoothInput();
        Move();
        Rotate();
    }

    private void ReadMouseInput()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector2 screenCenter = new Vector2(
            Screen.width * 0.5f,
            Screen.height * 0.5f
        );

        Vector2 offset = mousePosition - screenCenter;

        Vector2 normalizedInput = new Vector2(
            offset.x / screenCenter.x,
            offset.y / screenCenter.y
        );

        normalizedInput = Vector2.ClampMagnitude(
            normalizedInput,
            1f
        );

        targetInput = normalizedInput * mouseSensitivity;

        targetInput = Vector2.ClampMagnitude(
            targetInput,
            1f
        );
    }

    private void SmoothInput()
    {
        currentInput = Vector2.Lerp(
            currentInput,
            targetInput,
            mouseSmooth * Time.fixedDeltaTime
        );
    }

    private void Move()
    {
        Vector3 velocity =
            transform.forward * forwardSpeed
            + transform.right * currentInput.x * moveSpeed
            + transform.up * currentInput.y * moveSpeed;

        rb.linearVelocity = velocity;
    }

    private void Rotate()
    {
        float pitch = -currentInput.y * pitchAmount;
        float yaw = currentInput.x * yawAmount;
        float roll = -currentInput.x * rollAmount;

        Quaternion targetRotation = Quaternion.Euler(
            pitch,
            yaw,
            roll
        );

        Quaternion newRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSmooth * Time.fixedDeltaTime
        );

        rb.MoveRotation(newRotation);
    }
}