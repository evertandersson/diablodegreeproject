using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    PlayerManager playerManager;
    public GameObject projectile;
    private Vector3 offset = new Vector3(0, 1.2f, 0);

    private void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
    }

    public void SpawnProjectile()
    {
        Debug.Log("Spawn projectile");
        projectile = Instantiate(projectile, transform.position + offset, playerManager.transform.rotation);
    }
}
