using UnityEngine;

public class Fireball : MonoBehaviour, IPooledObject
{
    [SerializeField]
    private float moveSpeed = 10;

    public GameObject explosion;

    private bool hasHit;

    [SerializeField]
    float lifeTime = 5.0f;
    float timer;

    public void OnObjectSpawn()
    {
        hasHit = false;
        timer = 0;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;

        timer += Time.fixedDeltaTime;
        if (timer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit == false)
        {
            if (explosion != null 
                && !other.gameObject.GetComponent<PlayerManager>()
                && !other.gameObject.CompareTag("Projectile"))
            {
                ObjectPooling.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity);
                ObjectPooling.Instance.DespawnObject(this.gameObject);
                hasHit = true;
            }
        }

    }

}
