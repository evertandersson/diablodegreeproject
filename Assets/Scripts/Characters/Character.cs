using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour, IPausable
{
    [SerializeField]
    public float health;

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

    protected CapsuleCollider capsuleCollider;
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;


    [SerializeField] protected float visionAngle = 45f; // Half of the total field of view
    [SerializeField] public float visionRange = 10f; // Distance the character can see
    [SerializeField] public LayerMask detectionMask; // Layers the character can "see" (e.g., player)

    private int runSpeedHash = Animator.StringToHash("RunSpeed");

    #region Properties

    public float Health
    {
        get => health;
        private set
        {
            health = Mathf.Clamp(value, 0, maxHealth); // Clamp to ensure within bounds
        }
    }

    public bool IsDead => isDead;

    public NavMeshAgent Agent { get; protected set; }

    public Animator CharacterAnimator { get; protected set; }

    #endregion

    public void Initialize()
    {
        if (!Agent) Agent = GetComponent<NavMeshAgent>();
        if (!capsuleCollider) capsuleCollider = GetComponent<CapsuleCollider>();
        if (rigidbodies == null) rigidbodies = GetComponentsInChildren<Rigidbody>(true);
        if (colliders == null) colliders = GetComponentsInChildren<Collider>(true);
    }

    protected virtual void Start()
    {
        health = maxHealth;
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        originalColor = renderers[0].material.color;
    }

    protected virtual void OnEnable()
    {
        Popup.Pause += Pause;
        Popup.UnPause += UnPause;
    }

    protected virtual void OnDisable()
    {
        Popup.Pause -= Pause;
        Popup.UnPause -= UnPause;
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

    }

    public bool CanSeeTarget(Transform target, Vector3 offset)
    {
        if (!target) return false;

        // Calculate direction and distance to the target
        Vector3 directionToTarget = (target.position + offset - (transform.position + offset)).normalized;
        float distanceToTarget = Vector3.Distance(transform.position + offset, target.position + offset);
        
        Debug.DrawRay(transform.position + offset, directionToTarget * visionRange, Color.red);

        // Check if the target is within the vision range
        if (distanceToTarget > visionRange) return false;

        // Check if the target is within the vision angle
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > visionAngle / 2f) return false;

        // Perform a raycast to ensure there are no obstacles
        if (Physics.Raycast(transform.position + offset, directionToTarget, out RaycastHit hit, visionRange, detectionMask))
        {
            // Check if the raycast hit the actual target
            return hit.transform == target;
        }

        return false;
    }

    public bool IsAnimationPlayingStrict(int animationHash)
    {
        var currentState = CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        var nextState = CharacterAnimator.GetNextAnimatorStateInfo(0);

        return currentState.shortNameHash == animationHash || nextState.shortNameHash == animationHash;
    }

    public bool IsAnimationPlaying(int animationHash)
    {
        var currentState = CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        return currentState.shortNameHash == animationHash;
    }

    public virtual void EnableRagdoll(bool enable)
    {
        if (CharacterAnimator != null)
            CharacterAnimator.enabled = !enable;

        Agent.enabled = !enable;

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !enable;
            rb.useGravity = enable;
        }

        foreach (Collider c in colliders)
        {
            c.enabled = enable;
        }
        capsuleCollider.enabled = !enable;
    }

    public void SetFloatRunSpeed()
    {
        float speed = Agent.velocity.magnitude / Agent.speed;
        float currentSpeed = CharacterAnimator.GetFloat(runSpeedHash);
        float targetSpeed = Mathf.Clamp01(speed);
        float smoothSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);
        CharacterAnimator.SetFloat(runSpeedHash, smoothSpeed);
    }

    protected abstract void Die();

    public abstract void Attack(int attackIndex);

    public abstract void Pause();
    public abstract void UnPause();
}
