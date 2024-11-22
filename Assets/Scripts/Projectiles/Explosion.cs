using Game;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem explosionParticle;
    private SphereCollider sphereCollider;

    void Start()
    {
        explosionParticle = GetComponent<ParticleSystem>();
        sphereCollider = GetComponent<SphereCollider>();

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < sphereCollider.radius)
            {
                enemy.TakeDamage(2);

                // Scale explosion force based on the distance to the enemy
                Vector3 explosionDirection = (enemy.transform.position - transform.position).normalized;
                explosionDirection.y = 0; // Prevent vertical flipping

                // Calculate force based on distance
                float force = Mathf.Lerp(10, 5, distance / sphereCollider.radius);
                enemy.RB.AddForce(explosionDirection * force, ForceMode.Impulse);
            }
        }

    }


    void Update()
    {
        if (!explosionParticle.isPlaying && explosionParticle.time >= explosionParticle.main.duration)
        {
            Destroy(gameObject);
        }
    }
}
