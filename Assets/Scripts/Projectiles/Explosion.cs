using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem explosionParticle;

    void Start()
    {
        explosionParticle = GetComponent<ParticleSystem>();

        Character[] characters = FindObjectsByType<Character>(FindObjectsSortMode.None);

        foreach (Character character in characters)
        {
            if (Vector3.Distance(transform.position, character.transform.position) < 4)
            {
                character.TakeDamage(2);
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
