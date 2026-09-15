using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -25f);
    [SerializeField] private float followSpeed = 12f;

    [Header("Vertical Follow")]
    [SerializeField] private float verticalFollowSpeed = 18f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        FollowPosition();
        FollowRotation();
    }

    private void FollowPosition()
    {
        Vector3 targetPosition = target.position + offset;

        float x = Mathf.Lerp(
            transform.position.x,
            targetPosition.x,
            followSpeed * Time.deltaTime
        );

        float y = Mathf.Lerp(
            transform.position.y,
            targetPosition.y,
            verticalFollowSpeed * Time.deltaTime
        );

        float z = targetPosition.z;

        transform.position = new Vector3(x, y, z);
    }

    private void FollowRotation()
    {
        Quaternion targetRotation = Quaternion.identity;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}