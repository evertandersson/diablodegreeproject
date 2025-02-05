using UnityEngine;

public class EnemySpawnerTrigger : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;

    private void OnTriggerEnter(Collider other)
    {
        enemySpawner.StartCoroutine("SpawnEnemies");
    }
}
