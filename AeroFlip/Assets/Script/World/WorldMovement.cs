using UnityEngine;

public class WorldMovement : MonoBehaviour
{
    [SerializeField] float worldSpeed = 30f;
    [SerializeField] bool canMove;

    public float CurrentSpeed => worldSpeed;
    private void Update()
    {
        if (!canMove) return;

        transform.position += Vector3.back * worldSpeed * Time.deltaTime;
    }

    public void SetMovement(bool value)
    {
        canMove = value;
    }

    public void ResetPosition()
    {
        transform.position = Vector3.zero;
    }
}
