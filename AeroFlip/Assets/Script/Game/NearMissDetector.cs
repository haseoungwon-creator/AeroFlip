using System.Collections.Generic;
using UnityEngine;

public class NearMissDetector : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] float rayDistance = 20f;
    [SerializeField] float nearMissThreshold = 3f;

    private readonly HashSet<Collider> nearMissCandidates = new HashSet<Collider>();
    private readonly List<Collider> removeBuffer = new List<Collider>();

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameStates.Playing)
            return;

        DetectObstacles();
        CheckPassedObstacles();
    }

    private void DetectObstacles()
    {
        Vector3 position = transform.position;

        CheckRay(position, Vector3.forward);
        CheckRay(position, Vector3.up);
        CheckRay(position, Vector3.down);
        CheckRay(position, Vector3.left);
        CheckRay(position, Vector3.right);
    }

    private void CheckRay(Vector3 origin, Vector3 direction)
    {
        if (!Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, obstacleLayer))
            return;

        Debug.Log($"[NearMiss 감지] {hit.collider.name} / 방향: {direction} / 거리: {hit.distance:F2}");

        nearMissCandidates.Add(hit.collider);
    }

    private void CheckPassedObstacles()
    {
        if (nearMissCandidates.Count == 0)
            return;

        removeBuffer.Clear();

        foreach (Collider obstacle in nearMissCandidates)
        {
            if (obstacle == null)
            {
                removeBuffer.Add(obstacle);
                continue;
            }

            if (obstacle.bounds.max.z < transform.position.z)
            {
                float distance = Vector3.Distance(
                    obstacle.ClosestPoint(transform.position),
                    transform.position);

                if (distance <= nearMissThreshold)
                {
                    scoreManager.AddNearMiss();
                    Debug.Log($"Near Miss! 거리: {distance:F2}");
                }

                removeBuffer.Add(obstacle);
            }
        }

        foreach (Collider obstacle in removeBuffer)
            nearMissCandidates.Remove(obstacle);
    }

    public void ResetDetector()
    {
        nearMissCandidates.Clear();
        removeBuffer.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 position = transform.position;

        Gizmos.DrawRay(position, Vector3.forward * rayDistance);
        Gizmos.DrawRay(position, Vector3.up * rayDistance);
        Gizmos.DrawRay(position, Vector3.down * rayDistance);
        Gizmos.DrawRay(position, Vector3.left * rayDistance);
        Gizmos.DrawRay(position, Vector3.right * rayDistance);
    }
}