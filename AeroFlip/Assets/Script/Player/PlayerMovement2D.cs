using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] PlayerMode playerMode;

    [SerializeField] float moveSpeed = 25f;
    [SerializeField] float moveSmooth = 15f;
    [SerializeField] float minX = -40f;
    [SerializeField] float maxX = 40f;
    [SerializeField] float minY = 0;
    [SerializeField] float maxY = 50f;

    private Transform _t;
    private Vector2 currentInput;
    private Vector3 targetPosition;

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
        if (playerMode == null || !playerMode.Is2D())
            return;

        ReadKeyboard();
        Move2D();
        KeepZPosition();
    }

    private void ReadKeyboard()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.leftArrowKey.isPressed) horizontal = -1f;
        if (Keyboard.current.rightArrowKey.isPressed) horizontal = 1f;
        if (Keyboard.current.upArrowKey.isPressed) vertical = 1f;
        if (Keyboard.current.downArrowKey.isPressed) vertical = -1f;

        currentInput = Vector2.Lerp(currentInput, new Vector2(horizontal, vertical), 8f * Time.deltaTime);
    }

    private void Move2D()
    {
        Vector3 movement = new Vector3(currentInput.x, currentInput.y, 0f);
        targetPosition += movement * moveSpeed * Time.deltaTime;

        ClampPosition();
        _t.position = Vector3.Lerp(_t.position, targetPosition, moveSmooth * Time.deltaTime);
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
    }
}