using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    public int health;

    #region Stats

    [Header("Stats")]
    [SerializeField] protected int level;
    [SerializeField] protected int maxHealth = 20;
    [SerializeField] protected int damage = 5;
    [SerializeField] protected int defense = 2;

    public int Level => level;
    public int MaxHealth => maxHealth;
    public int Damage => damage;
    public int Defense => defense;

    #endregion

    protected float rotationSpeed = 10.0f;

    [SerializeField]   
    protected ParticleSystem bloodSplashEffect;

    protected bool isDead = false;

    //Components for flash effect:
    [SerializeField] private SkinnedMeshRenderer[] renderers;
    private Color originalColor;
    private Color flashColor = Color.red;
    private float flashDuration = 0.1f;

    [SerializeField] private float visionAngle = 45f; // Half of the total field of view
    [SerializeField] public float visionRange = 10f; // Distance the character can see
    [SerializeField] private LayerMask detectionMask; // Layers the character can "see" (e.g., player)

    #region Properties

    public int Health
    {
        get => health;
        private set
        {
            health = Mathf.Clamp(value, 0, maxHealth); // Clamp to ensure within bounds
        }
    }

    public bool IsDead => isDead;

    public NavMeshAgent Agent { get; protected set; }

    #endregion

    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        health = maxHealth;
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        originalColor = renderers[0].material.color;
    }


    protected virtual void SetStats()
    {
        if (level > 1)
        {
            for (int i = 0; i < level - 1; i++)
            {
                float statsMultiplier = 1.5f;
                maxHealth = Mathf.RoundToInt(maxHealth * statsMultiplier);
                health = maxHealth;
                damage = Mathf.RoundToInt(damage * statsMultiplier);
                defense = Mathf.RoundToInt(defense * statsMultiplier);
            }
        }
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(FlashRoutine());
        if (health <= 0)
        {
            Die();
        }
    }

    protected IEnumerator FlashRoutine()
    {
        foreach (Renderer r in renderers)
        {
            if (r.enabled)
                r.material.color = flashColor;
        }
        yield return new WaitForSeconds(flashDuration);
        foreach (Renderer r in renderers)
        {
            if (r.enabled)
                r.material.color = originalColor;
        }
    }


    public virtual void HandleRotation(Vector3 destinationPosition)
    {
        destinationPosition.y = transform.position.y;

        Vector3 direction = destinationPosition - transform.position;

        if (direction.sqrMagnitude > 0.01f)  // Check if the direction is not too small (to avoid jittering)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        Agent.isStopped = true;
    }

    public bool CanSeeTarget(Transform target, Vector3 offset)
    {
        if (!target) return false;

        Vector3 directionToTarget = (target.position + offset - (transform.position + offset)).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Check if the target is within the vision range
        if (distanceToTarget > visionRange) return false;

        // Check if the target is within the vision angle
        float angleToPlayer = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToPlayer > visionAngle) return false;

        // Perform a raycast to ensure there are no obstacles
        if (Physics.Raycast(transform.position + offset, directionToTarget, out RaycastHit hit, visionRange, detectionMask))
        {
            return hit.transform == target; // Check if the hit object is the player
        }

        return false;
    }

    protected abstract void Die();

    public abstract void Attack(int attackIndex);
}
