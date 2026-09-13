using UnityEngine;

public class PropellerRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1500f;

    private void Update()
    {
        transform.Rotate(
            Vector3.forward,
            rotationSpeed * Time.deltaTime,
            Space.Self
        );
    }
}