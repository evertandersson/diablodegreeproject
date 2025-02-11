using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private LayerMask wallMask;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main; // Ensure the correct camera is assigned
    }

    private void Update()
    {
        Vector3 directionToPlayer = targetObject.position - transform.position;

        // Check if the player is actually behind something
        if (Vector3.Dot(transform.forward, directionToPlayer.normalized) > 0)
        {
            return; // Skip cutout if the player is in front
        }

        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, directionToPlayer, directionToPlayer.magnitude, wallMask);

        foreach (RaycastHit hit in hitObjects)
        {
            if (hit.transform.TryGetComponent<Renderer>(out Renderer renderer))
            {
                Material[] materials = renderer.materials;

                foreach (Material mat in materials)
                {
                    mat.SetVector("_CutOutPos", cutoutPos);
                    mat.SetFloat("_CutOutSize", 0.1f);
                    mat.SetFloat("_FalloffSize", 0.05f);
                }
            }
        }
    }
}
