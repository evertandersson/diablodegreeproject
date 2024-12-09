using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    public class MouseInput : MonoBehaviour
    {
        public Vector3 mouseInputPosition;

        [SerializeField]
        private LayerMask raycastLayers;
        public RaycastHit hit;

        [SerializeField] private CinemachineFollow followComponent;
        private Vector3 followOffset;
        [SerializeField] float followOffsetMinY = 10f;
        [SerializeField] float followOffsetMaxY = 50f;

        Outline highlightedObject = null;

        private void Awake()
        {
            followComponent = GetComponent<CinemachineFollow>();
            followOffset = followComponent.FollowOffset;
        }

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, float.MaxValue, raycastLayers))
            {
                mouseInputPosition = hit.point;

                // Check if the raycast hit a Door
                if (hit.transform.CompareTag("Door"))
                {
                    var doorOutline = hit.transform.GetComponent<Outline>();

                    // If it's a new object, enable the outline
                    if (doorOutline != highlightedObject)
                    {
                        // Disable the previous outline
                        if (highlightedObject != null)
                        {
                            highlightedObject.enabled = false;
                        }

                        // Update the highlighted object and enable its outline
                        highlightedObject = doorOutline;
                        highlightedObject.enabled = true;
                    }
                }
                else
                {
                    // If no Door is hit, reset the outline
                    if (highlightedObject != null)
                    {
                        highlightedObject.enabled = false;
                        highlightedObject = null;
                    }
                }
            }
            else
            {
                // If the raycast hits nothing, reset the outline
                if (highlightedObject != null)
                {
                    highlightedObject.enabled = false;
                    highlightedObject = null;
                }
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