using System;
using UnityEngine;

namespace Game
{
    public class PlayerManager : Character
    {
        public enum State { Idle, Attack, Rolling, Inventory, GoToDoor }

        private static PlayerManager _instance;

        // Events
        public event Action<float> OnYValueChanged;

        // Serialized fields
        [SerializeField] private LevelProgressBar progressBar;
        [SerializeField] private MouseInput mouseInput;
        [SerializeField] private HealthBar healthBar;

        // Components and fields
        private PlayerAnimator playerAnimator;
        private PlayerMovement playerMovement;
        private PlayerInput playerInput;
        private ProjectileSpawner projectileSpawner;

        private float lastYPosition;
        private float attackTimer;
        private bool isAttacking;
        private bool isRolling;
        private bool canAttack = true;

        private Interactable currentObject;
        private ActionItemSO currentAction;

        public LevelSystem LevelSystem { get; private set; }
        public SlotManager SlotManager { get; private set; }
        public Inventory Inventory { get; private set; }

        public static PlayerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("PlayerManager instance is null.");
                }
                return _instance;
            }
        }

        public State CurrentPlayerState { get; private set; }

        public bool IsAttacking
        {
            get => isAttacking;
            private set
            {
                isAttacking = value;
                if (value)
                {
                    CurrentPlayerState = State.Attack;
                }
            }
        }

        public bool CanAttack
        {
            get => canAttack;
            set => canAttack = value;
        }

        public bool IsRolling => isRolling;

        protected override void Awake()
        {
            // Singleton setup
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeComponents();
            InitializeLevelSystem();
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            InitializeHealthBar();
            LevelSystem.OnLevelChanged += UpgradeStats;
        }

        private void InitializeComponents()
        {
            playerAnimator = GetComponent<PlayerAnimator>();
            playerInput = GetComponent<PlayerInput>();
            playerMovement = GetComponent<PlayerMovement>();
            projectileSpawner = GetComponentInChildren<ProjectileSpawner>();
            SlotManager = GetComponent<SlotManager>();
        }

        private void InitializeLevelSystem()
        {
            LevelSystem = new LevelSystem();
            progressBar.SetLevelSystem(LevelSystem);
            SetStats();
        }

        private void InitializeHealthBar()
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        public void Heal(int amount)
        {
            health += amount;
            healthBar.SetHealth(health);
        }

        protected override void SetStats()
        {
            base.SetStats();
            LevelSystem.SetLevel(level);
        }

        private void UpgradeStats(object sender, EventArgs e)
        {
            level = LevelSystem.GetCurrentLevel();
            if (level > 1)
            {
                ApplyLevelUpStats();
            }
        }

        private void ApplyLevelUpStats()
        {
            float multiplier = 1.5f;
            maxHealth = Mathf.RoundToInt(maxHealth * multiplier);
            health = maxHealth;
            damage = Mathf.RoundToInt(damage * multiplier);
            defense = Mathf.RoundToInt(defense * multiplier);
            healthBar.SetMaxHealth(maxHealth);
        }

        private void Update()
        {
            HandleAttackDelay();
            SlotManager.HandleCooldowns();
            playerAnimator.HandleAnimations(IsAttacking);

            if (IsAttacking)
            {
                Agent.isStopped = true;
                HandleRotation(mouseInput.mouseInputPosition);
            }

            if (isRolling)
            {
                playerMovement.HandleEndRolling();
            }

            HandleInteraction();
            CheckYPositionChange();
        }

        private void HandleInteraction()
        {
            if (currentObject != null && Vector3.Distance(transform.position, currentObject.GetCenterPoint()) < 2)
            {
                currentObject.Trigger();
                currentObject = null;
                Agent.isStopped = true;
            }
        }

        private void CheckYPositionChange()
        {
            float currentYPosition = transform.position.y;
            if (Mathf.Abs(currentYPosition - lastYPosition) > Mathf.Epsilon)
            {
                lastYPosition = currentYPosition;
                OnYValueChanged?.Invoke(currentYPosition);
            }
        }

        public void SetCurrentObject(Interactable obj)
        {
            currentObject = obj;
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            healthBar.SetHealth(health);
        }

        public override void Attack(int attackIndex)
        {
            if (!CanAttack || attackIndex < 0 || attackIndex >= SlotManager.actionSlots.Length)
            {
                Debug.Log("Invalid attack index.");
                return;
            }

            ActionItemSO actionItem = SlotManager.actionSlots[attackIndex].item as ActionItemSO;
            if (actionItem != null && actionItem.timerCooldown >= actionItem.cooldown)
            {
                PerformAction(actionItem, attackIndex);
            }
        }

        private void PerformAction(ActionItemSO actionItem, int attackIndex)
        {
            currentAction = actionItem;

            if (actionItem is AttackTypeSO attackType)
            {
                attackType.PerformAction(CharacterAnimator);
                IsAttacking = true;
                projectileSpawner.projectile = attackType.projectile;
            }
            else if (actionItem is PotionSO potion)
            {
                potion.PerformAction(CharacterAnimator);
                SlotManager.actionSlots[attackIndex].itemAmount -= 1;
                SlotManager.actionSlots[attackIndex].UpdateItemAmountText();
            }
        }

        private void HandleAttackDelay()
        {
            if (!IsAttacking || currentAction == null) return;

            if (currentAction is AttackTypeSO attackType && attackTimer > attackType.attackDelay)
            {
                ClearAttack();
                CurrentPlayerState = State.Idle;
                playerMovement.ProcessBufferedInput();
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

        public void ClearAttack()
        {
            attackTimer = 0;
            currentAction = null;
            IsAttacking = false;
        }

        protected override void Die()
        {
            Debug.Log("Player is dead");
        }
    }
}
