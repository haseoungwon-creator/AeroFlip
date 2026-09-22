using System.Collections;
using UnityEngine;

public class DimensionTransition : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerTransitionMovement transitionMovement;
    [SerializeField] CameraController cameraController;
    [SerializeField] PlayerMode playerMode;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] MapManager mapManager;

    private bool isTransitioning;
    private float transitionMapEndZ;

    public bool IsTransitioning => isTransitioning;

    public void TransitionTo2D()
    {
        if (isTransitioning || playerMode == null || !playerMode.Is3D()) return;

        StartCoroutine(TransitionTo2DSequence());
    }

    private IEnumerator TransitionTo2DSequence()
    {
        isTransitioning = true;
        cameraController.SetTransitioning(true);

        transitionMapEndZ = mapManager.SpawnTransitionSafeMaps();

        yield return WaitForTransitionMap();

        yield return transitionMovement.MoveToCenter();


        scoreManager.StopScoring();
        yield return transitionMovement.Rise();

        cameraController.Set2DView();
        playerMode.SetMode(PlayerModes.Mode2D);
        playerController.Set3DInput();


        cameraController.SetTransitioning(false);
        scoreManager.StartScoring();
        playerController.SetControlEnabled(true);
        isTransitioning = false;
    }

    private IEnumerator WaitForTransitionMap()
    {
        while(!mapManager.IsTransitionMapArrived(transitionMapEndZ))
            yield return null;

        playerController.SetControlEnabled(false);
    }
}
