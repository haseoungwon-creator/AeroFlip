using UnityEngine;

public class PlayerMovement3D1 : MonoBehaviour
{
    [SerializeField] PlayerInputHandler input;
    [SerializeField] PlayerMode playerMode;

    [SerializeField] float moveSpeed = 25f;
    [SerializeField] float verticalMultiplier = 2f;
    [SerializeField] float moveSmooth = 15f;
    [SerializeField] float clickMoveMultiplier = 0.2f;

    [SerializeField] float minX = -40f;
    [SerializeField] float maxX = 40f;
    [SerializeField] float minY = 0f;
    [SerializeField] float maxY = 50f;

    [SerializeField] float maxPitch = 20f;
    [SerializeField] float maxRoll = 18f;
    [SerializeField] float clickMaxRoll = 200f;
    [SerializeField] float pitchRotationSmooth = 25f;
    [SerializeField] float rollRotationSmooth = 8f;

    [SerializeField] float inputSensitivity = 1.5f;
    [SerializeField] float inputSmooth = 8f;

    Transform t;
    Vector2 currentInput;
    Vector3 targetPosition;
    float rawMouseX;

    void Awake()
    {
        t = transform;
    }

    void Start()
    {
        targetPosition = t.position;
    }

    void Update()
    {
        if (playerMode == null || !playerMode.Is3D()) return;

        ReadInput();
        Move();
        Rotate();
        KeepZPosition();
    }

    void ReadInput()
    {
        Vector2 pointer = input.PointerPosition;
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 rawInput = new Vector2((pointer.x - center.x) / center.x, (pointer.y - center.y) / center.y) * inputSensitivity;

        rawMouseX = Mathf.Clamp(rawInput.x, -1f, 1f);
        rawInput.x = rawMouseX;
        rawInput.y = Mathf.Clamp(rawInput.y, -1f, 1f);

        currentInput = Vector2.Lerp(currentInput, rawInput, inputSmooth * Time.deltaTime);
    }

    void Move()
    {
        Vector3 right = t.right;
        Vector3 up = t.up;
        right.z = 0f;
        up.z = 0f;

        if (right.sqrMagnitude > 0.001f) right.Normalize();
        if (up.sqrMagnitude > 0.001f) up.Normalize();

        float moveMultiplier = input.ClickHeld ? clickMoveMultiplier : 1f;
        Vector3 movement = (right * currentInput.x + up * currentInput.y) * verticalMultiplier;

        targetPosition += movement * moveSpeed * moveMultiplier * Time.deltaTime;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        t.position = Vector3.Lerp(t.position, targetPosition, moveSmooth * Time.deltaTime);
    }

    void Rotate()
    {
        float targetPitch = -currentInput.y * maxPitch;
        float targetRoll = -rawMouseX * (input.ClickHeld ? clickMaxRoll : maxRoll);

        Vector3 currentEuler = t.eulerAngles;
        float pitch = Mathf.LerpAngle(currentEuler.x, targetPitch, pitchRotationSmooth * Time.deltaTime);
        float roll = Mathf.LerpAngle(currentEuler.z, targetRoll, rollRotationSmooth * Time.deltaTime);

        t.rotation = Quaternion.Euler(pitch, 0f, roll);
    }

    void KeepZPosition()
    {
        targetPosition.z = t.position.z;
    }

    public void ResetMovement()
    {
        targetPosition = t.position;
        currentInput = Vector2.zero;
        rawMouseX = 0f;
        t.rotation = Quaternion.identity;
    }
}