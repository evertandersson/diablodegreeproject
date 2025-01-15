using UnityEngine;

namespace Game
{
    public class ProjectileSpawner : MonoBehaviour
    {
        public GameObject projectile;
        private Vector3 offset = new Vector3(0, 1.2f, 0);

        public void SpawnProjectileWithOffset(string tag)
        {
            SpawnProjectile(tag, true);
        }

        public void SpawnProjectileWithoutOffset(string tag)
        {
            SpawnProjectile(tag, false);
        }


        public void SpawnProjectile(string tag, bool offsetEnable)
        {
            if (offsetEnable)
            {
                projectile = ObjectPooling.Instance.SpawnFromPool(
                tag,
                transform.position + offset,
                PlayerManager.Instance.transform.rotation);
            }
            else
            {
                projectile = ObjectPooling.Instance.SpawnFromPool(
                tag,
                transform.position + transform.forward * 1f,
                PlayerManager.Instance.transform.rotation);
            }
        }
    }

}
