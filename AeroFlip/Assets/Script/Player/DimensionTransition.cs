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
    [SerializeField] MissileSpawner missileSpawner;

    private bool isTransitioning;
    private float transitionMapEndZ;

    public bool IsTransitioning => isTransitioning;

    public void TransitionTo2D()
    {
        if (isTransitioning || playerMode == null || !playerMode.Is3D()) return;

        StartCoroutine(TransitionTo2DSequence());
    }

    public void TransitionTo3D()
    {
        if (isTransitioning || playerMode == null || !playerMode.IsMode2D()) return;
        StartCoroutine(TransitionTo3DSequence());
    }

    private IEnumerator TransitionTo2DSequence()
    {
        isTransitioning = true;
        mapManager.SpawnTransitionSafeMaps();

        yield return WaitForTransitionMap();

        yield return new WaitForSeconds(2f);

        playerController.SetControlEnabled(false);

        cameraController.LockCamera();

        scoreManager.StopScoring();
        yield return transitionMovement.Rise();

        mapManager.HideMaps();

        cameraController.Set2DView();
        playerMode.SetMode(PlayerModes.Mode2D);
        playerController.Set2DInput();

        scoreManager.StartScoring();
        playerController.SetControlEnabled(true);

        missileSpawner.SetSpawning(true);

        mapManager.EndTransitionMap();
        isTransitioning = false;
        
    }

    private IEnumerator TransitionTo3DSequence()
    {
        isTransitioning = true;

        scoreManager.StopScoring();

        yield return missileSpawner.SpawnTransitionPattern();

        yield return transitionMovement.Dive();

        mapManager.ResetMaps();

        cameraController.Set3DView();
        playerMode.SetMode(PlayerModes.Mode3D);
        playerController.Set3DInput();

        scoreManager.StartScoring();

        isTransitioning = false;
    }

    private IEnumerator WaitForTransitionMap()
    {
        while(!mapManager.IsTransitionMapArrived(transitionMapEndZ))
            yield return null;
    }
}
