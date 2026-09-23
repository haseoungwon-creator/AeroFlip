using UnityEngine;
using System.Collections;
public class PlayerTransitionMovement : MonoBehaviour
{
    [SerializeField] float transitionHeight = 150f;
    [SerializeField] float transitionDuration = 1.5f;
    [SerializeField] float riseRotationX = -70f;
    [SerializeField] float diveRotationX = 70f;

    public bool Istransitioning {  get; private set; }

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
            t= Mathf.SmoothStep(0f, 1f,t);

            transform.position = Vector3.Lerp(startPosition,targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation,targetRotation, t);

            yield return null;
        }

        transform.position = new Vector3(0,10,0);
        transform.rotation = Quaternion.Euler(0,0,0);

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
    
    public void SetPlayer()
    {
        transform.position = new Vector3(0,10,0);
    }
}
