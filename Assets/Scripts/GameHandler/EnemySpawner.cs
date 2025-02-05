using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemySpawner : MonoBehaviour
    {
        [Serializable]
        private class EnemyToSpawn
        {
            [SerializeField] private string enemyTag;
            [SerializeField] public int level;
            [SerializeField] private int count;

            public string EnemyTag => enemyTag;
            public int Count => count;
        }

        [SerializeField] private EnemyToSpawn[] enemiesToSpawn; // Array of enemies to spawn
        [SerializeField] private float spawnInterval = 1f; // Delay between each spawn

        public IEnumerator SpawnEnemies()
        {
            foreach (var enemyToSpawn in enemiesToSpawn)
            {
                for (int i = 0; i < enemyToSpawn.Count; i++)
                {
                    yield return new WaitForSeconds(spawnInterval);

                    // Spawn enemy from pool
                    GameObject go = ObjectPooling.Instance.SpawnFromPool(enemyToSpawn.EnemyTag, transform.position, Quaternion.identity);
                    Enemy enemy = go.GetComponent<Enemy>();
                    enemy.SetLevel(enemyToSpawn.level);

                    NavMeshHit closestHit;
                    if (NavMesh.SamplePosition(transform.position, out closestHit, 500, 1))
                    {
                        go.transform.position = closestHit.position;
                    }
                }
            }
        }
    }

}
