using UnityEngine;

namespace Game
{
    public class MouseInput : MonoBehaviour
    {
        public Vector3 mouseInputPosition;

        [SerializeField]
        private LayerMask raycastLayers;

        [SerializeField]
        private LayerMask raycastLayerWall;

        [SerializeField]
        private Vector3 offset;

        GameObject player;

        private void Start()
        {
            player = FindFirstObjectByType<PlayerMovement>().gameObject;
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

            Ray rayWall = Camera.main.ScreenPointToRay(player.transform.position);
            if (Physics.Raycast(ray, out hit))

            transform.position = player.transform.position + offset;
            transform.LookAt(player.transform.position);
        }
    }
}   