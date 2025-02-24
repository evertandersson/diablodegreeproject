using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class ItemDrop 
    {
        public ItemSO item;
        [Range(0f, 1f)] public float dropRate;
    }

    public class Enemy : Character, IPooledObject
    {
        [SerializeField] private int experienceOnDeath = 20;

        // Base stats: 
        private int baseMaxHealth;
        private int baseDamage;
        private int baseDefense;
        private int baseExperienceOnDeath;

        private Transform player;

        public bool standStill = false;
        public bool isAggro = false;

        public float distanceToAttack = 1.5f;

        // Animation names:
        [HideInInspector] public int damageAnim = Animator.StringToHash("damage");
        [HideInInspector] public int[] attackAnims = { Animator.StringToHash("Attack1"), Animator.StringToHash("Attack2") };
        [HideInInspector] public int rangedAttackAnim = Animator.StringToHash("Battlecry");

        // Trigger names:
        [HideInInspector] public int attackTrigger = Animator.StringToHash("Attack");
        [HideInInspector] public int isHitTrigger = Animator.StringToHash("IsHit");
        [HideInInspector] public int rangedAttackTrigger = Animator.StringToHash("RangedAttack");

        public EnemyEventHandler EnemyEventHandler { get; private set; }
        public List<EnemyEvent> Events { get; private set; } 
        public Transform Player => player;

        [SerializeField] protected EnemyHealthBar healthBar;

        [Header("Drop rate")]
        [SerializeField] private List<ItemDrop> itemDropRates;

        public GolemBoss golem;

        public void OnObjectSpawn()
        {
            isDead = false;
            health = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.DisplayHealthBar(true);

            Agent.enabled = false;

            EnemyEventHandler.EventStack.Clear();
            EnableRagdoll(false);
            standStill = false;

            SetNewEvent<EnemyIdle>();
        }

        public void SetLevel(int level)
        {
            this.level = level;
            healthBar.SetLevelText(level);

            if (level > 1)
            {
                float statsMultiplier = 1.5f;
                maxHealth = Mathf.RoundToInt(baseMaxHealth * Mathf.Pow(statsMultiplier, level - 1));
                health = maxHealth;
                damage = Mathf.RoundToInt(baseDamage * Mathf.Pow(statsMultiplier, level - 1));
                defense = Mathf.RoundToInt(baseDefense * Mathf.Pow(statsMultiplier, level - 1));
                experienceOnDeath = Mathf.RoundToInt(baseExperienceOnDeath * Mathf.Pow(statsMultiplier, level - 1));
            }
        }

        protected virtual void Awake()
        {
            // Cache components on Awake
            Initialize();

            SetBaseStats();

            CharacterAnimator = GetComponent<Animator>();
            EnemyEventHandler = EnemyEventHandler.CreateEventHandler();
            EnableRagdoll(false);

            golem = this as GolemBoss;

            if (EnemyEventHandler == null)
            {
                Debug.LogError("EventHandler not found on Enemy.");
            }

            Events = new List<EnemyEvent>(GetComponents<EnemyEvent>());
        }

        private void SetBaseStats()
        {
            baseMaxHealth = maxHealth;
            baseDamage = damage;
            baseDefense = defense;
            baseExperienceOnDeath = experienceOnDeath;
        }

        protected virtual void OnEnable()
        {
            TriggerCutscene01.StartCutscene01 += StartCutscene;
            TriggerCutscene01.StopCutscene01 += StopCutscene;
            TriggerCutscene02.StartCutscene02 += StartCutscene;
            TriggerCutscene02.StopCutscene02 += StopCutscene;
        }

        protected virtual void OnDisable()
        {
            TriggerCutscene01.StartCutscene01 -= StartCutscene;
            TriggerCutscene01.StopCutscene01 -= StopCutscene;
            TriggerCutscene02.StartCutscene02 -= StartCutscene;
            TriggerCutscene02.StopCutscene02 -= StopCutscene;
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
            
            SetLevel(level);

            healthBar.SetMaxHealth(maxHealth);

            player = PlayerManager.Instance.gameObject.transform;

            SetNewEvent<EnemyIdle>(); // Set the first event as idle

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
                    SetNewEvent<EnemyTakeDamage>(); // Add damage event to the stack
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
            // Locate the sword and shield GameObjects
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

        // Called when adding a new event to the event
        public void SetNewEvent<T>() where T : EnemyEvent
        {
            EnemyEvent newEvent = Events.FirstOrDefault(e => e is T);
            if (newEvent != null)
            {
                EnemyEventHandler.PushEvent(newEvent);
            }
        }

        public override void Attack(int attackIndex = 0)
        {
            SetNewEvent<EnemyMeleeAttack>();
        }

        public virtual void FollowTarget()
        {
            SetNewEvent<EnemyFollowTarget>();
        }

        protected override void Die()
        {
            EnemyEventHandler.EventStack.Clear();
            EnableRagdoll(true);
            isDead = true;
            PlayerManager.Instance.levelSystem.AddExperience(experienceOnDeath);
            HandleItemDrops();
        }

        private void HandleItemDrops()
        {
            foreach(ItemDrop itemDrop in itemDropRates)
            {
                float rng = Random.Range(0f, 1f);
                if (rng < itemDrop.dropRate)
                {
                    Instantiate(itemDrop.item.prefab, transform.position, Quaternion.identity);
                }
            }
        }

//        private void OnGUI()
//        {
//
//#if UNITY_EDITOR
//            const float LINE_HEIGHT = 32.0f;
//            GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.7f);
//            Rect r = new Rect(0, 0, 250.0f, LINE_HEIGHT * EnemyEventHandler.EventStack.Count);
//            GUI.DrawTexture(r, Texture2D.whiteTexture);
//
//            Rect line = new Rect(10, 0, r.width - 20, LINE_HEIGHT);
//            for (int i = 0; i < EnemyEventHandler.EventStack.Count; i++)
//            {
//                GUI.color = EnemyEventHandler.EventStack[i] == EnemyEventHandler.CurrentEvent ? Color.green : Color.white;
//                GUI.Label(line, "#" + i + ": " + EnemyEventHandler.EventStack[i].ToString(), i == 0 ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label);
//                line.y += line.height;
//            }
//#endif
//        }

//        private void OnDrawGizmos()
//        {
//            // Draw the vision range
//            Gizmos.color = Color.yellow;
//            Gizmos.DrawWireSphere(transform.position, visionRange);
//                    // Draw the vision cone
//            Vector3 forward = transform.forward * visionRange;
//            Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle, 0) * forward;
//            Vector3 rightBoundary = Quaternion.Euler(0, visionAngle, 0) * forward;
//                    Gizmos.color = Color.red;
//            Gizmos.DrawRay(transform.position, leftBoundary);
//            Gizmos.DrawRay(transform.position, rightBoundary);
//        }
        

    }
}
