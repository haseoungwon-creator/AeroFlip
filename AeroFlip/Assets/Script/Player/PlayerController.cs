using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerMode playerMode;

    [SerializeField] float shiftCooldown = 5f;

    private bool canChangeMode = true;
    private bool isTransitioning;

    private void Start()
    {
        ResetPlayer();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            TryChangeMode();
        }
    }

    private void TryChangeMode()
    {
        if (!canChangeMode)
            return;

        if (isTransitioning)
            return;

        if (playerMode.Is3D())
        {
            StartCoroutine(ChangeTo2D());
        }
        else
        {
            StartCoroutine(ChangeTo3D());
        }
    }

    private IEnumerator ChangeTo2D()
    {
        isTransitioning = true;
        canChangeMode = false;

        playerMode.SetMode(PlayerModes.Mode2D);

        yield return new WaitForSeconds(1f);

        isTransitioning = false;

        yield return new WaitForSeconds(shiftCooldown);
        canChangeMode = true;
    }

    private IEnumerator ChangeTo3D()
    {
        isTransitioning = true;
        canChangeMode = false;

        playerMode.SetMode(PlayerModes.Mode3D);

        yield return new WaitForSeconds(1f);

        isTransitioning = false;

        yield return new WaitForSeconds(shiftCooldown);
        canChangeMode = true;
    }

    public void ResetPlayer()
    {
        StopAllCoroutines();

        playerMode.ResetMode();

        canChangeMode = true;
        isTransitioning = false;
    }
}