using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected int health;

    [SerializeField]
    protected int maxHealth = 20;

    protected float rotationSpeed = 10.0f;

    [SerializeField]   
    protected ParticleSystem bloodSplashEffect;

    protected bool isDead = false;

    //Components for flash effect:
    [SerializeField] private SkinnedMeshRenderer[] renderers;
    private Color originalColor;
    private Color flashColor = Color.red;
    private float flashDuration = 0.1f;

    #region Properties

    public int Health => health;

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

    protected abstract void Die();

    public abstract void Attack(int attackIndex);
}
