using Game;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem explosionParticle;

    void Start()
    {
        explosionParticle = GetComponent<ParticleSystem>();

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < 4)
            {
                enemy.TakeDamage(2);
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
