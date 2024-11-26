using Game;
using UnityEngine;

public class Explosion : MonoBehaviour, IPooledObject
{
    private ParticleSystem explosionParticle;

    void Awake()
    {
        explosionParticle = GetComponentInChildren<ParticleSystem>();
    } 

    public void OnObjectSpawn()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < 4)
            {
                enemy.TakeDamage(PlayerManager.Instance.Damage);
            }
        }
    }

    void Update()
    {
        if (!explosionParticle.isPlaying && explosionParticle.time >= explosionParticle.main.duration)
        {
            ObjectPooling.Instance.DespawnObject(this.gameObject);
        }
    }
}
