using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    public class MouseInput : MonoBehaviour
    {
        public Vector3 mouseInputPosition;

        [SerializeField]
        private LayerMask raycastLayers;

        [SerializeField] private CinemachineFollow followComponent;
        private Vector3 followOffset;
        [SerializeField] float followOffsetMinY = 10f;
        [SerializeField] float followOffsetMaxY = 50f;

        private void Awake()
        {
            followComponent = GetComponent<CinemachineFollow>();
            followOffset = followComponent.FollowOffset;
        }

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform raycast with LayerMask
            if (Physics.Raycast(ray, out hit, float.MaxValue, raycastLayers))
            {
                mouseInputPosition = hit.point;
            }

            HandleCameraZoom();
        }

        private void HandleCameraZoom()
        {
            float zoomAmount = 2f;
            if (Input.mouseScrollDelta.y > 0)
            {
                followOffset.y -= zoomAmount;
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                followOffset.y += zoomAmount;
            }

            followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);

            float zoomSpeed = 10f;
            followComponent.FollowOffset = Vector3.Lerp(followComponent.FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
        }
    }
}   