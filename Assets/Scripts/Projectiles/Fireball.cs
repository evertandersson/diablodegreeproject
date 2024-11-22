using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10;

    public GameObject explosion;

    private bool hasHit = false;

    [SerializeField]
    float lifeTime = 5.0f;
    float timer = 0;

    private void FixedUpdate()
    {
        transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;

        timer += Time.fixedDeltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit == false)
        {
            if (explosion != null && !other.gameObject.GetComponent<PlayerManager>())
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
                hasHit = true;
            }
        }

    }
}
