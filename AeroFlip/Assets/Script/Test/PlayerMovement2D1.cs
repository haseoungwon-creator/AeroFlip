using UnityEngine;

public class PlayerMovement2D1 : MonoBehaviour
{
    [SerializeField] PlayerInputHandler input;
    [SerializeField] PlayerMode playerMode;

    [SerializeField] float moveSpeed = 25f;
    [SerializeField] float moveSmooth = 15f;
    [SerializeField] float inputSmooth = 8f;
    [SerializeField] float rotationSmooth = 8f;

    [SerializeField] float minX = -40f;
    [SerializeField] float maxX = 40f;
    [SerializeField] float minY = 0f;
    [SerializeField] float maxY = 50f;

    Transform t;
    Vector2 currentInput;
    Vector3 targetPosition;

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
        if (playerMode == null || !playerMode.Is2D()) return;

        ReadInput();
        Move();
        Rotate();
        KeepZPosition();
    }

    void ReadInput()
    {
        currentInput = Vector2.Lerp(currentInput, input.Move2D, inputSmooth * Time.deltaTime);
    }

    void Move()
    {
        Vector3 movement = new Vector3(currentInput.x, currentInput.y, 0f);
        targetPosition += movement * moveSpeed * Time.deltaTime;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        t.position = Vector3.Lerp(t.position, targetPosition, moveSmooth * Time.deltaTime);
    }

    void Rotate()
    {
        t.rotation = Quaternion.Slerp(t.rotation, Quaternion.identity, rotationSmooth * Time.deltaTime);
    }

    void KeepZPosition()
    {
        targetPosition.z = t.position.z;
    }

    public void ResetMovement()
    {
        targetPosition = t.position;
        currentInput = Vector2.zero;
    }
}