using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningEffect : MonoBehaviour, IPooledObject
{
    [SerializeField] private int maxChains = 5; // Maximum number of enemies to chain to
    [SerializeField] private Material lightningMaterial; // Material for the lightning effect
    [SerializeField] private float flickerDuration = 0.5f; // Duration of the lightning flicker

    private LineRenderer lineRenderer;
    private List<Enemy> chainedEnemies = new List<Enemy>(); // Track chained enemies
    private Vector3 offset = new Vector3(0, 1.2f, 0);
    private GameObject[] lightningEffects;

    private void Awake()
    {
        // Initialize LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lightningMaterial;
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 0.8f;
        lineRenderer.useWorldSpace = true;

        // Set up gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0.0f),
                new GradientColorKey(Color.blue, 0.5f),
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f)
            }
        );
        lineRenderer.colorGradient = gradient;
    }

    public void OnObjectSpawn()
    {
        chainedEnemies.Clear();
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, PlayerManager.Instance.transform.position + offset);
        }

        // Start the lightning chain
        ChainToClosestEnemy(PlayerManager.Instance.transform.position);
        StartCoroutine(DespawnTimer());
    }

    private void ChainToClosestEnemy(Vector3 startPosition)
    {
        lightningEffects = new GameObject[maxChains];
        for (int i = 0; i < maxChains; i++)
        {
            // Find the closest enemy that hasn't been chained yet
            Enemy closestEnemy = FindClosestEnemy(startPosition, i == 0);

            if (closestEnemy == null)
            {
                break; // No more enemies to chain to
            }

            // Add the enemy to the chain
            chainedEnemies.Add(closestEnemy);

            // Update the lightning position to the new enemy
            startPosition = closestEnemy.transform.position;

            // Move the lightning to the enemy (optional visual adjustment)
            transform.position = closestEnemy.transform.position;

            // Add a point to the line renderer for the lightning visual
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = chainedEnemies.Count + 1;
                lineRenderer.SetPosition(chainedEnemies.Count, closestEnemy.transform.position + offset);
                lightningEffects[i] = ObjectPooling.Instance.SpawnFromPool("Lightning", transform.position, Quaternion.identity);
                closestEnemy.TakeDamage(PlayerManager.Instance.Damage);
            }
        }
    }

    private Enemy FindClosestEnemy(Vector3 position, bool firstTime)
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Enemy closestEnemy = null;
        float closestDistance = PlayerManager.Instance.visionRange;

        foreach (Enemy enemy in enemies)
        {
            // Skip already chained or dead enemies
            if (chainedEnemies.Contains(enemy) || enemy.IsDead) continue;

            float distance = Vector3.Distance(position, enemy.transform.position);
            // Check if this enemy is closer and visible
            if (distance < closestDistance)
            {
                if (firstTime && !PlayerManager.Instance.CanSeeTarget(enemy.transform, offset))
                {
                    continue;
                }
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }


    private IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(0.3f);
        
        //Remove all lightning effects
        foreach(GameObject lightningEffect in lightningEffects)
        {
            if (lightningEffect != null) ObjectPooling.Instance.DespawnObject(lightningEffect);
        }

        //Remove lightning trail
        ObjectPooling.Instance.DespawnObject(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        // Visualize chain distance in the editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, PlayerManager.Instance.visionRange);
    }
}
