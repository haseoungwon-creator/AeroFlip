using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 cameraOffset3D = new Vector3(0, 8, -12);
    [SerializeField] Vector3 cameraRotation3D = new Vector3(20, 0, 0);
    [SerializeField] Vector3 cameraOffset2D = new Vector3(0, 90, 0);
    [SerializeField] Vector3 cameraRotation2D = new Vector3(90, 0, 0);

    private bool is2D;
    private bool isCameraLocked;
    private void LateUpdate()
    {
        if (player == null) return;

        if (isCameraLocked)
        {
            FollowPlayer();
            return;
        }

        if (is2D)
            Update2DCamera();
        else
            Update3DCamera();
    }

    public void Set3DView()
    {
        is2D = false;
        isCameraLocked = false;
        Apply3DView();
    }

    public void Set2DView()
    {
        is2D=true;
        isCameraLocked = false;
        Apply2DView();
    }

    private void Update3DCamera()
    {
        transform.position = player.position + cameraOffset3D;
        transform.rotation = Quaternion.Euler(cameraRotation3D);
    }

    private void Update2DCamera()
    {
        transform.position = new Vector3(0, 90, 0);
        transform.rotation = Quaternion.Euler(cameraRotation2D);
    }

    private void FollowPlayer()
    {
        transform.LookAt(player);
    }

    public void LockCamera()
    {
        isCameraLocked = true;
    }

    private void Apply3DView()
    {
        transform.position = player.position + cameraOffset3D;
        transform.rotation = Quaternion.Euler(cameraRotation3D);
    }

    private void Apply2DView()
    {
        transform.position = new Vector3(0, 90, 0);
        transform.rotation = Quaternion.Euler(cameraRotation2D);
    }
}
