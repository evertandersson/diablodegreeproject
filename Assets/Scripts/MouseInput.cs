using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

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
        private bool highlightEnabled = true;

        [SerializeField] private Texture2D cursorTexture;
        private Vector2 hotspot = Vector2.zero;

        [SerializeField] private CinemachineCamera dialougeCamera;

        private void Awake()
        {
            followComponent = GetComponent<CinemachineFollow>();
            followOffset = followComponent.FollowOffset;
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }

        private void Start()
        {
            PlayerInput.HighlightItems += DisableHighlight;
            PlayerInput.HideItems += EnableHighlight;
        }


        private void OnDisable()
        {
            PlayerInput.HighlightItems -= DisableHighlight;
            PlayerInput.HideItems -= EnableHighlight;
        }


        private void EnableHighlight()
        {
            highlightEnabled = true;
        }
        private void DisableHighlight()
        {
            highlightEnabled = false;
        }

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, float.MaxValue, raycastLayers))
            {
                mouseInputPosition = hit.point;

                // Check if the raycast hit a Door
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable") && highlightEnabled)
                {
                    Outline outline = hit.transform.GetComponent<Outline>();

                    // If it's a new object, enable the outline
                    if (outline != highlightedObject)
                    {
                        // Disable the previous outline
                        if (highlightedObject != null)
                        {
                            highlightedObject.enabled = false;
                        }

                        // Update the highlighted object and enable its outline
                        highlightedObject = outline;
                        highlightedObject.enabled = true;
                    }
                }
                else
                {
                    // If no object is hit, reset the outline
                    if (highlightedObject != null && highlightEnabled)
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
            if (EventHandler.Main.CurrentEvent is DialougeManager)
            {
                if (DialougeManager.Instance.currentNPC != null)
                {
                    SwitchToDialogueCamera(true);
                }
            }
            else
            {
                SwitchToDialogueCamera(false);
            }

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

        private void SwitchToDialogueCamera(bool isDialogueActive)
        {
            if (isDialogueActive)
            {
                dialougeCamera.Priority = 3; // Higher priority to take control
            }
            else
            {
                dialougeCamera.Priority = 0; // Lower priority, so normal camera takes over
            }
        }

    }
}   