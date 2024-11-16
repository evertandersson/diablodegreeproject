using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem explosionParticle;

    void Start()
    {
        explosionParticle = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!explosionParticle.isPlaying && explosionParticle.time >= explosionParticle.main.duration)
        {
            Destroy(gameObject);
        }
    }
}
