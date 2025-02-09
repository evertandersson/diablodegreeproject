using UnityEngine;
using Game;
using Unity.Cinemachine;

public class WallClippingHandler : MonoBehaviour
{
    private Camera thisCamera;

    private void Start()
    {
        thisCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleWallClipping();
    }

    private void HandleWallClipping()
    {
        if (thisCamera == null || PlayerManager.Instance == null) return;

        Vector3 cameraPos = transform.position;
        Vector3 playerPos = PlayerManager.Instance.transform.position;
        Vector3 direction = (playerPos - cameraPos).normalized;
        float maxDistance = Vector3.Distance(cameraPos, playerPos);

        // Raycast ONLY for walls
        int wallLayerMask = LayerMask.GetMask("Wall");

        if (Physics.Raycast(cameraPos, direction, out RaycastHit hit, maxDistance, wallLayerMask))
        {
            float newNearClip = Mathf.Clamp(Vector3.Distance(cameraPos, hit.point), 0.01f, 5f);
            thisCamera.nearClipPlane = newNearClip;
        }
        else
        {
            // Reset to default when no wall is blocking
            thisCamera.nearClipPlane = 0.1f;
        }
    }
}
