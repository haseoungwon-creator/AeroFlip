using System.Collections;
using UnityEngine;

public class DimensionTransition : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerTransitionMovement transitionMovement;
    [SerializeField] CameraController cameraController;
    [SerializeField] PlayerMode playerMode;
    [SerializeField] WallPool wallPool;
    [SerializeField] ScoreManager scoreManager;

    [SerializeField] float wallDistance = 30f;

    private bool isTransitioning;
    private GameObject currentWall;

    public bool IsTransitioning => isTransitioning;

    public void TransitionTo2D()
    {
        if (isTransitioning || playerMode == null || !playerMode.Is3D()) return;

        StartCoroutine(TransitionTo2DSequence());
    }

    private IEnumerator TransitionTo2DSequence()
    {
        isTransitioning = true;
        playerController.SetControlEnabled(false);
        scoreManager.StopScoring();
        cameraController.SetTransitioning(true);

        yield return transitionMovement.MoveToCenter();

        SpawnWall();

        yield return transitionMovement.Rise();

        cameraController.Set2DView();
        playerMode.SetMode(PlayerModes.Mode2D);

        ReturnWall();

        cameraController.Set2DView();
        cameraController.SetTransitioning(false);
        playerMode.SetMode(PlayerModes.Mode2D);
        ReturnWall();

        scoreManager.StartScoring();
        playerController.SetControlEnabled(true);
        isTransitioning = false;
    }

    private void SpawnWall()
    {
        if (wallPool == null) return;

        currentWall = wallPool.GetWall();

        if(currentWall == null) return;

        currentWall.transform.position = transform.position + Vector3.forward * wallDistance;
    }

    private void ReturnWall()
    {
        if(currentWall == null) return;

        wallPool.ReturnWall();
        currentWall = null;
    }
}
