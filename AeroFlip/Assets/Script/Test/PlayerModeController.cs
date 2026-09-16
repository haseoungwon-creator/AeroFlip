using System;
using System.Collections;
using UnityEngine;

public class PlayerModeController : MonoBehaviour
{
    [SerializeField] PlayerInputHandler input;
    [SerializeField] PlayerMode playerMode;
    [SerializeField] float shiftCooldown = 5f;

    [SerializeField] float wallSpawnDelay = 0.3f;
    [SerializeField] float dodgeWindowUp = 1.2f;
    [SerializeField] float cloudTransitionDurationUp = 1.5f;
    [SerializeField] int safeZoneCountUp = 3;

    [SerializeField] float mode2DAutoReturnTime = 15f;

    [SerializeField] float missileWarningLeadTime = 1.5f;
    [SerializeField] float dodgeWindowDown = 1.2f;
    [SerializeField] float cloudTransitionDurationDown = 1.5f;
    [SerializeField] int safeZoneCountDown = 6;
    [SerializeField] float postLandingSafeDuration = 2.5f;

    public event Action<int> OnSafeZoneSpawnRequested;
    public event Action OnWallSpawnRequested;
    public event Action OnDodgeUpStarted;
    public event Action OnCloudTransitionUpStarted;
    public event Action OnEnteredMode2D;
    public event Action OnMissileWarningRequested;
    public event Action OnDodgeDownStarted;
    public event Action OnCloudTransitionDownStarted;
    public event Action OnEnteredMode3D;
    public event Action OnObstacleSpawnResumeRequested;

    bool canChangeMode = true;
    bool isTransitioning;
    Coroutine mode2DAutoReturnRoutine;
    Coroutine activeSequenceRoutine;

    void OnEnable()
    {
        input.ChangeModePressed += TryChangeMode;
        input.RewindPressed += UseRewind;
        input.TimeSlowPressed += UseTimeSlow;
    }

    void OnDisable()
    {
        input.ChangeModePressed -= TryChangeMode;
        input.RewindPressed -= UseRewind;
        input.TimeSlowPressed -= UseTimeSlow;
    }

    void TryChangeMode()
    {
        if (!canChangeMode || isTransitioning) return;

        isTransitioning = true;
        canChangeMode = false;

        if (playerMode.Is3D())
        {
            activeSequenceRoutine = StartCoroutine(GoTo2DSequence());
        }
        else
        {
            if (mode2DAutoReturnRoutine != null)
            {
                StopCoroutine(mode2DAutoReturnRoutine);
                mode2DAutoReturnRoutine = null;
            }
            activeSequenceRoutine = StartCoroutine(GoTo3DSequence());
        }
    }

    IEnumerator GoTo2DSequence()
    {
        OnSafeZoneSpawnRequested?.Invoke(safeZoneCountUp);
        yield return new WaitForSeconds(wallSpawnDelay);
        OnWallSpawnRequested?.Invoke();
        OnDodgeUpStarted?.Invoke();
        yield return new WaitForSeconds(dodgeWindowUp);
        OnCloudTransitionUpStarted?.Invoke();
        yield return new WaitForSeconds(cloudTransitionDurationUp);
        playerMode.SetMode(PlayerModes.Mode2D);
        OnEnteredMode2D?.Invoke();
        isTransitioning = false;
        mode2DAutoReturnRoutine = StartCoroutine(Mode2DAutoReturnTimer());
        yield return new WaitForSeconds(shiftCooldown);
        canChangeMode = true;
    }

    IEnumerator Mode2DAutoReturnTimer()
    {
        yield return new WaitForSeconds(mode2DAutoReturnTime);
        mode2DAutoReturnRoutine = null;
        if (!isTransitioning)
        {
            isTransitioning = true;
            canChangeMode = false;
            activeSequenceRoutine = StartCoroutine(GoTo3DSequence());
        }
    }

    IEnumerator GoTo3DSequence()
    {
        OnMissileWarningRequested?.Invoke();
        yield return new WaitForSeconds(missileWarningLeadTime);
        OnDodgeDownStarted?.Invoke();
        yield return new WaitForSeconds(dodgeWindowDown);
        OnCloudTransitionDownStarted?.Invoke();
        yield return new WaitForSeconds(cloudTransitionDurationDown);
        playerMode.SetMode(PlayerModes.Mode3D);
        OnEnteredMode3D?.Invoke();
        isTransitioning = false;
        OnSafeZoneSpawnRequested?.Invoke(safeZoneCountDown);
        yield return new WaitForSeconds(postLandingSafeDuration);
        OnObstacleSpawnResumeRequested?.Invoke();
        yield return new WaitForSeconds(shiftCooldown);
        canChangeMode = true;
    }

    void UseRewind()
    {
        Debug.Log("시간 역행 스킬");
    }

    void UseTimeSlow()
    {
        Debug.Log("시간 정지 스킬");
    }

    public void ResetPlayer()
    {
        if (activeSequenceRoutine != null) StopCoroutine(activeSequenceRoutine);
        if (mode2DAutoReturnRoutine != null) StopCoroutine(mode2DAutoReturnRoutine);
        StopAllCoroutines();
        activeSequenceRoutine = null;
        mode2DAutoReturnRoutine = null;
        playerMode.ResetMode();
        canChangeMode = true;
        isTransitioning = false;
    }
}