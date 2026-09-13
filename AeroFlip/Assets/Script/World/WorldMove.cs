using UnityEngine;

public class WorldMovement : MonoBehaviour
{
    [SerializeField] private float worldSpeed = 30f;

    private Transform _t;

    private void Awake()
    {
        _t = transform;
    }

    private void Update()
    {
        _t.position += Vector3.back * worldSpeed * Time.deltaTime;
    }
}