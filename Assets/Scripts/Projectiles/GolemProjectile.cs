using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GolemProjectile : EnemyProjectile
    {
        [SerializeField] private RawImage landingIndicator; // The UI indicator to show landing position
        [SerializeField] private LayerMask groundLayer; // LayerMask for detecting the ground
        private Vector3 offset = new Vector3(0, 0.1f, 0);
        [SerializeField] private Transform rockPosition;


        public override void OnObjectSpawn()
        {
            base.OnObjectSpawn();

            Quaternion randomRot = Quaternion.Euler(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
            rockPosition.rotation = randomRot;

            landingIndicator.transform.parent.SetParent(null);

            RaycastToGround();
        }

        protected override void FixedUpdate()
        {
            transform.position += -transform.up * moveSpeed * Time.fixedDeltaTime;

            timer += Time.fixedDeltaTime;
            if (timer >= lifeTime)
            {
                gameObject.SetActive(false);
            }
        }

        private void RaycastToGround()
        {
            // Perform a raycast downward from the projectile to detect the ground
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                // Move the indicator to the detected ground position
                landingIndicator.transform.position = hit.point + offset;
                landingIndicator.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("GolemProjectile: No ground detected for landing indicator.");
                DisableLandingIndicator();
            }
        }

        private void DisableLandingIndicator()
        {
            if (landingIndicator != null)
            {
                landingIndicator.gameObject.SetActive(false);
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (other.gameObject.CompareTag("Projectile")) return;
            
            landingIndicator.transform.parent.SetParent(this.transform);
        }
    }

}
