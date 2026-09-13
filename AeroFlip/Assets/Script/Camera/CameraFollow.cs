using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -25f);
    [SerializeField] private float positionSmooth = 15f;

    [Header("Rotation Follow")]
    [SerializeField] private float rotationSmoothTime = 0.35f;
    [SerializeField] private float maxRoll = 15f;
    [SerializeField] private float maxPitch = 8f;

    private Vector3 positionVelocity;
    private float currentPitch;
    private float currentRoll;
    private float pitchVelocity;
    private float rollVelocity;

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
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity, 1f / positionSmooth);
    }

    private void FollowRotation()
    {
        float targetPitch = Mathf.Clamp(Mathf.DeltaAngle(0f, target.eulerAngles.x), -maxPitch, maxPitch);
        float targetRoll = Mathf.Clamp(Mathf.DeltaAngle(0f, target.eulerAngles.z), -maxRoll, maxRoll);

        currentPitch = Mathf.SmoothDampAngle(currentPitch, targetPitch, ref pitchVelocity, rotationSmoothTime);
        currentRoll = Mathf.SmoothDampAngle(currentRoll, targetRoll, ref rollVelocity, rotationSmoothTime);

        transform.rotation = Quaternion.Euler(currentPitch, 0f, currentRoll);
    }
}