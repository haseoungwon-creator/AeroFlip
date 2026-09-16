using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement3D : MonoBehaviour
{
    [SerializeField] float moveSpeed = 25f;
    [SerializeField] float verticalMultiplier = 2f;
    [SerializeField] float moveSmooth = 15f;
    [SerializeField] float clickMoveMultiplier = 0.2f;

    [SerializeField] float minX = -40f;
    [SerializeField] float maxX = 40f;
    [SerializeField] float minY = 0;
    [SerializeField] float maxY = 50f;

    [SerializeField] float maxPitch = 20f;
    [SerializeField] float maxRoll = 18f;
    [SerializeField] float clickMaxRoll = 200f;
    [SerializeField] float pitchRotationSmooth = 25f;
    [SerializeField] float rollRotationSmooth = 8f;

    [SerializeField] float inputSensitivity = 1.5f;
    [SerializeField] float inputSmooth = 8f;

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
        Move3D();
        Rotate3D();
        KeepZPosition();
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

        SmoothInput(input);
    }

    private void SmoothInput(Vector2 input)
    {
        currentInput = Vector2.Lerp(currentInput, input, inputSmooth * Time.deltaTime);
    }

    private void Move3D()
    {
        Vector3 right = _t.right;
        Vector3 up = _t.up;
        right.z = 0f;
        up.z = 0f;

        if (right.sqrMagnitude > 0.001f) right.Normalize();
        if (up.sqrMagnitude > 0.001f) up.Normalize();

        float moveMultiplier = clickHeld ? clickMoveMultiplier : 1f;
        Vector3 movement = (right * currentInput.x + up * currentInput.y) * verticalMultiplier;

        targetPosition += movement * moveSpeed * moveMultiplier * Time.deltaTime;

        ApplyMove();
    }

    private void ApplyMove()
    {
        ClampPosition();
        _t.position = Vector3.Lerp(_t.position, targetPosition, moveSmooth * Time.deltaTime);
    }

    private void Rotate3D()
    {
        float targetPitch = -currentInput.y * maxPitch;
        float targetRoll = -rawMouseX * (clickHeld ? clickMaxRoll : maxRoll);

        Vector3 currentEuler = _t.eulerAngles;
        float pitch = Mathf.LerpAngle(currentEuler.x, targetPitch, pitchRotationSmooth * Time.deltaTime);
        float roll = Mathf.LerpAngle(currentEuler.z, targetRoll, rollRotationSmooth * Time.deltaTime);

        _t.rotation = Quaternion.Euler(pitch, 0f, roll);
    }

    private void ClampPosition()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
    }

    private void KeepZPosition()
    {
        targetPosition.z = _t.position.z;
    }

    public void ResetMovement()
    {
        targetPosition = _t.position;
        currentInput = Vector2.zero;
        rawMouseX = 0f;
        clickHeld = false;
        _t.rotation = Quaternion.identity;
    }
}