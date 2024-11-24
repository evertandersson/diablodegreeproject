using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class Enemy : Character, IPooledObject
    {
        [SerializeField] private float visionAngle = 45f; // Half of the total field of view
        [SerializeField] private float visionRange = 10f; // Distance the enemy can see
        [SerializeField] private LayerMask detectionMask; // Layers the enemy can "see" (e.g., player)
        private Transform player; // Reference to the player
        private CapsuleCollider capsuleCollider;

        // Animation names:
        public string damageAnimName = "damage";
        public string[] attackAnimNames = { "Attack1", "Attack2" };

        [SerializeField] public NavMeshAgent Agent { get; private set; }
        public Animator Animator { get; private set; }
        public EventHandler EnemyEventHandler { get; private set; }
        public List<EnemyEvent> Events { get; private set; } 
        public Transform Player => player;

        [SerializeField] private EnemyHealthBar healthBar;

        public void OnObjectSpawn()
        {
            health = maxHealth;
            healthBar.SetMaxHealth(maxHealth);

            RemoveAllEvents();
            EnableRagdoll(false);

            Agent = gameObject.AddComponent<NavMeshAgent>();

            if (Agent != null)
            {
                Agent.enabled = true; // Ensure it's active
                Agent.speed = 3.5f; // Set desired speed or other properties
                Agent.angularSpeed = 120f; // Example property, adjust as needed
                SetNewEvent<EnemyIdle>(); // Assign the first event
            }
            else
            {
                Debug.LogError("Failed to add NavMeshAgent to the enemy!");
            }
        }

        private void Awake()
        {
            // Cache components on Awake
            Animator = GetComponent<Animator>();
            EnemyEventHandler = EventHandler.CreateEventHandler();
            capsuleCollider = GetComponent<CapsuleCollider>();

            if (EnemyEventHandler == null)
            {
                Debug.LogError("EventHandler not found on Enemy.");
            }

            Events = new List<EnemyEvent>(GetComponents<EnemyEvent>());
        }

        protected override void Start()
        {
            base.Start();
            player = PlayerManager.Instance.gameObject.transform;
        }

        public override void TakeDamage(int damage)
        {
            health -= damage;
            StartCoroutine(FlashRoutine());
            if (health <= 0)
            {
                Die();
            }
            else
            {
                SetNewEvent<EnemyTakeDamage>();
            }
            healthBar.SetHealth(health);
        }

        public void EnableRagdoll(bool enable)
        {
            if (Animator != null)
                Animator.enabled = !enable;

            capsuleCollider.enabled = !enable;

            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = !enable;
                rb.useGravity = enable;
            }

            DetachWeapons(enable);
        }

        public void DetachWeapons(bool enable)
        {
            // Locate the sword and shield GameObjects (adjust the names based on your hierarchy)
            Transform sword = transform.Find("Group/Geometry/geo/sword_low");
            Transform shield = transform.Find("Group/Geometry/geo/shield_low");

            if (sword != null)
            {
                sword.gameObject.SetActive(!enable);
            }
            if (shield != null)
            {
                shield.gameObject.SetActive(!enable);
            }
        }

        public bool CanSeePlayer()
        {
            if (!player) return false;

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the player is within the vision range
            if (distanceToPlayer > visionRange) return false;

            // Check if the player is within the vision angle
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer > visionAngle) return false;

            // Perform a raycast to ensure there are no obstacles
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, visionRange, detectionMask))
            {
                return hit.transform == player; // Check if the hit object is the player
            }

            return false;
        }

        public void SetNewEvent<T>() where T : EnemyEvent
        {
            EnemyEvent newEvent = Events.FirstOrDefault(e => e is T);
            if (newEvent != null)
            {
                EnemyEventHandler.PushEvent(newEvent);
            }
        }

        public override void Attack(int attackIndex)
        {
            throw new System.NotImplementedException();
        }

        protected override void Die()
        {
            RemoveAllEvents();
            EnableRagdoll(true);
        }

        private void RemoveAllEvents()
        {
            if (EnemyEventHandler.EventStack.Count > 0)
            {
                // Create a copy of the EventStack to avoid modifying it during iteration
                var eventsCopy = new List<EventHandler.IEvent>(EnemyEventHandler.EventStack);
                foreach (var ev in eventsCopy)
                {
                    EnemyEventHandler.RemoveEvent(ev);
                }
            }

            // Destroy NavMeshAgent
            if (Agent != null)
            {
                Destroy(Agent);
                Agent = null; // Clear the reference
            }
        }


        private void OnGUI()
        {

#if UNITY_EDITOR
            const float LINE_HEIGHT = 32.0f;
            GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.7f);
            Rect r = new Rect(0, 0, 250.0f, LINE_HEIGHT * EnemyEventHandler.EventStack.Count);
            GUI.DrawTexture(r, Texture2D.whiteTexture);

            Rect line = new Rect(10, 0, r.width - 20, LINE_HEIGHT);
            for (int i = 0; i < EnemyEventHandler.EventStack.Count; i++)
            {
                GUI.color = EnemyEventHandler.EventStack[i] == EnemyEventHandler.CurrentEvent ? Color.green : Color.white;
                GUI.Label(line, "#" + i + ": " + EnemyEventHandler.EventStack[i].ToString(), i == 0 ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label);
                line.y += line.height;
            }
#endif
        }

        private void OnDrawGizmos()
        {
            // Draw the vision range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, visionRange);

            // Draw the vision cone
            Vector3 forward = transform.forward * visionRange;
            Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle, 0) * forward;
            Vector3 rightBoundary = Quaternion.Euler(0, visionAngle, 0) * forward;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, leftBoundary);
            Gizmos.DrawRay(transform.position, rightBoundary);
        }

    }
}
