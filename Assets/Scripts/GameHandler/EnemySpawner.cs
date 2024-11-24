using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private int enemyCount = 5;
    private int currentEnemiesSpawned = 0;

    [SerializeField]
    private float spawnDelay = 1f;

    private void Start()
    {
        //StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnEnemy();
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (currentEnemiesSpawned < enemyCount)
        {
            yield return new WaitForSeconds(spawnDelay);

            SpawnEnemy();
        } 
    }

    void SpawnEnemy()
    {
        GameObject go = ObjectPooling.Instance.SpawnFromPool("Goblin", transform.position, transform.rotation);
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(transform.position, out closestHit, 500, 1))
        {
            go.transform.position = closestHit.position;
            go.AddComponent<NavMeshAgent>();
            currentEnemiesSpawned++;
            //TODO
        }
        else
        {
            Debug.Log("...");
        }
    }
}
