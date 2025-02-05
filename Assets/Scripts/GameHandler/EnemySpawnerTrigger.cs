using UnityEngine;

namespace Game
{
    public class EnemySpawnerTrigger : MonoBehaviour
    {
        [SerializeField] private EnemySpawner enemySpawner;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<PlayerManager>())
            {
                enemySpawner.StartCoroutine("SpawnEnemies");
                gameObject.SetActive(false);
            }
        }
    }
}
