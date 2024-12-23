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
        [SerializeField] private int experienceOnDeath = 20;

        private Transform player; // Reference to the player

        public bool standStill = false;
        public bool isAggro = false;

        // Animation names:
        public int damageAnim = Animator.StringToHash("damage");
        public int[] attackAnims = { Animator.StringToHash("Attack1"), Animator.StringToHash("Attack2") };

        // Trigger names:
        public int attackTrigger = Animator.StringToHash("Attack");
        public int isHitTrigger = Animator.StringToHash("IsHit");

        public EventHandler EnemyEventHandler { get; private set; }
        public List<EnemyEvent> Events { get; private set; } 
        public Transform Player => player;

        [SerializeField] private EnemyHealthBar healthBar;

        public void OnObjectSpawn()
        {
            isDead = false;
            health = maxHealth;
            healthBar.SetMaxHealth(maxHealth);

            Agent.enabled = false;

            EnemyEventHandler.EventStack.Clear();
            EnableRagdoll(false);

            SetNewEvent<EnemyIdle>();
        }

        protected void Awake()
        {
            // Cache components on Awake
            Initialize();
            CharacterAnimator = GetComponent<Animator>();
            EnemyEventHandler = EventHandler.CreateEventHandler();
            EnableRagdoll(false);

            if (EnemyEventHandler == null)
            {
                Debug.LogError("EventHandler not found on Enemy.");
            }

            Events = new List<EnemyEvent>(GetComponents<EnemyEvent>());
        }

        void OnEnable()
        {
            TriggerCutscene.StartCutsceneEvent += StartCutscene;
            TriggerCutscene.StopCutsceneEvent += StopCutscene;
        }

        void OnDisable()
        {
            TriggerCutscene.StartCutsceneEvent -= StartCutscene;
            TriggerCutscene.StopCutsceneEvent -= StopCutscene;
        }

        private void StartCutscene()
        {
            if (Agent.enabled)
                Agent.isStopped = true;

            EnemyEventHandler.EventStack.Clear();
            standStill = true;
            SetNewEvent<EnemyIdle>();
        }

        private void StopCutscene()
        {
            if (Agent.enabled)
            {
                Agent.isStopped = false;
                standStill = false;
            }
        }

        protected override void Start()
        {
            base.Start();
            healthBar.SetMaxHealth(maxHealth);

            player = PlayerManager.Instance.gameObject.transform;

            SetNewEvent<EnemyIdle>();

        }

        public override void TakeDamage(int damage)
        {
            if (!IsDead)
            {
                health -= damage;
                healthBar.SetHealth(health);
                StartCoroutine(FlashRoutine());
                bloodSplashEffect.Play();

                if (health <= 0)
                {
                    Die();
                }
                else
                {
                    SetNewEvent<EnemyTakeDamage>();
                }
            }


        }

        public override void EnableRagdoll(bool enable)
        {
            base.EnableRagdoll(enable);
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
            EnemyEventHandler.EventStack.Clear();
            EnableRagdoll(true);
            isDead = true;
            PlayerManager.Instance.levelSystem.AddExperience(experienceOnDeath);
        }

        /*private void OnGUI()
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
        */

    }
}
