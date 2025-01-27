using Game;
using UnityEngine;

public class Explosion : MonoBehaviour, IPooledObject
{
    private ParticleSystem explosionParticle;
    private SphereCollider sphereCollider;
    protected float damageRange;

    void Awake()
    {
        explosionParticle = GetComponentInChildren<ParticleSystem>();
        sphereCollider = GetComponent<SphereCollider>();
        damageRange = sphereCollider.radius + 0.5f;
    } 

    public virtual void OnObjectSpawn()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        SoundManager.PlaySound(SoundType.EXPLOSION);

        foreach (Enemy enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < damageRange)
            {
                StatsCalculator.CalculateDamage(PlayerManager.Instance.Damage, enemy);
            }
        }
    }

    void Update()
    {
        if (explosionParticle != null)
        {
            if (!explosionParticle.IsAlive(true))
            {
                ObjectPooling.Instance.DespawnObject(this.gameObject);
            }
        }
    }
}
