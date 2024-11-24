using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public GameObject projectile;
    private Vector3 offset = new Vector3(0, 1.2f, 0);

    public void SpawnProjectile(string tag)
    {
        projectile = ObjectPooling.Instance.SpawnFromPool(
            tag, 
            transform.position + offset, 
            PlayerManager.Instance.transform.rotation);
    }
}
