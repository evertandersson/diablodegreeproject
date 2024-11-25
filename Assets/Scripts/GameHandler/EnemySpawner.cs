using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    private class EnemyToSpawn
    {
        [SerializeField] private string enemyTag;
        [SerializeField] private int count; 

        public string EnemyTag => enemyTag;
        public int Count => count;
    }

    [SerializeField] private EnemyToSpawn[] enemiesToSpawn; // Array of enemies to spawn
    [SerializeField] private float spawnInterval = 1f; // Delay between each spawn

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        foreach (var enemyToSpawn in enemiesToSpawn)
        {
            for (int i = 0; i < enemyToSpawn.Count; i++)
            {
                yield return new WaitForSeconds(spawnInterval);

                // Spawn enemy from pool
                GameObject go = ObjectPooling.Instance.SpawnFromPool(enemyToSpawn.EnemyTag, transform.position, Quaternion.identity);

                NavMeshHit closestHit;
                if (NavMesh.SamplePosition(transform.position, out closestHit, 500, 1))
                {
                    go.transform.position = closestHit.position;
                }
            }
        }
    }
}
