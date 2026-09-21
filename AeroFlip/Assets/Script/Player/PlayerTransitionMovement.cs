using UnityEngine;
using System.Collections;
public class PlayerTransitionMovement : MonoBehaviour
{
    [SerializeField] float centerMoveDuration = 1f;
    [SerializeField] float transitionHeight = 30f;
    [SerializeField] float transitionDuration = 1.5f;
    [SerializeField] float riseRotationX = -70f;
    [SerializeField] float diveRotationX = 70f;

    public bool Istransitioning {  get; private set; }

    public IEnumerator MoveToCenter()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = Vector3.zero;

        float elapsedTime = 0f;

        while (elapsedTime < centerMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / centerMoveDuration;
            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }
        transform.position = targetPosition;
    }

    public IEnumerator Rise()
    {
        Istransitioning = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * transitionHeight;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(riseRotationX,startRotation.eulerAngles.y,startRotation.eulerAngles.z);

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / transitionDuration;
            t= Mathf.SmoothStep(0, 1,t);

            transform.position = Vector3.Lerp(startPosition,targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation,targetRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        Istransitioning = false;
    }

    public IEnumerator Dive()
    {
        Istransitioning = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition - Vector3.up * transitionHeight;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(diveRotationX, startRotation.eulerAngles.y, startRotation.eulerAngles.z);

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        Istransitioning = false;
    }
}
