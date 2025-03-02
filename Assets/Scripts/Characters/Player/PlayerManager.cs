using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PlayerManager : Character
    {
        public enum State
        {
            Idle,
            Attack,
            Rolling,
            Inventory,
            Dead
        }

        private static PlayerManager _instance;

        //Events
        public event Action<float> OnYValueChanged;

        private float lastYPosition;

        public LevelSystem levelSystem { get; set; }
        public LevelProgressBar progressBar;

        //Components and fields
        private PlayerAnimator playerAnimator;
        public PlayerInput playerInput;
        public PlayerMovement playerMovement;
        private ProjectileSpawner projectileSpawner;

        [SerializeField]
        public MouseInput mouseInput;

        [SerializeField]
        private State currentPlayerState;

        public SlotManager slotManager;
        public Inventory inventory;
        private StatsDisplay statsDisplay;
        public SkillTreeManager skillTree;
        public PauseMenu pauseMenu;

        private ActionItemSO currentAction;

        [SerializeField]
        private PlayerHealthOrb healthBar;

        [SerializeField]
        private PlayerManaOrb manaBar;

        // Mana:
        private float currentMana;
        [Header("Player Stats")]

        [Header("Mana")]
        [SerializeField] private float maxMana = 50;
        [SerializeField] private float manaRegeneration = 2;
        [SerializeField] private float healthRegeneration = 0;

        [Header("Rolling")]
        public float rollSpeed = 1;

        [HideInInspector] public bool isInteracting;
        public bool isAttacking;
        private bool canAttack = true;
        private bool isRolling;

        private Interactable currentObject = null;

        private float attackSpeed = 1f;
        public float attackTimer;

        [SerializeField] private RawImage attackIndicator;

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem healParticle;
        [SerializeField] private ParticleSystem manaParticle;
        [SerializeField] private ParticleSystem levelUpParticle;

        private Vector3 textOffset = new Vector3(0, 2.5f, 0);

        #region Properties

        public static PlayerManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public State CurrentPlayerState
        {
            get => currentPlayerState;
            set
            {
                if (currentPlayerState == value) return; // Avoid redundant updates

                currentPlayerState = value;

                // Execute behavior based on the new state
                switch (value)
                {
                    case State.Idle:
                        isRolling = false;
                        CanAttack = true;
                        playerMovement.ResetBufferedInput();
                        break;

                    case State.Attack:
                        break;

                    case State.Rolling:
                        isRolling = true;
                        CanAttack = false;
                        break;

                    case State.Inventory:
                        CanAttack = false;
                        break;

                    // Handle other states as needed
                    default:
                        CanAttack = false;
                        break;
                }
            }
        }

        public Interactable CurrentObject { get => currentObject; set => currentObject = value; }

        public ActionItemSO CurrentAction
        {
            get => currentAction;
            set
            {
                if (currentAction == value) return; // No need to reassign if the value hasn't changed

                ActionItemSO previousAction = currentAction;
                currentAction = value;

                // Reset the attack timer if the action changes
                if (previousAction != currentAction)
                {
                    attackTimer = 0;
                }
            }
        }

        public bool IsAttacking
        {
            get => isAttacking;
            set
            {
                isAttacking = value;
                if (value == true)
                {
                    currentPlayerState = State.Attack;
                }
            }
        }

        public bool IsRolling => isRolling;

        public bool CanAttack
        {
            get => canAttack;
            set
            {
                canAttack = value;
            }
        }

        public float Mana => currentMana;
        public float MaxMana => maxMana;
        public float ManaRegen => manaRegeneration;
        public float HealthRegen => healthRegeneration;

        public float AttackSpeed => attackSpeed;

        #endregion


        protected void Awake()
        {
            // Singleton setup
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            //DontDestroyOnLoad(gameObject);

            Debug.Log(Popup.activePopups);

            statsDisplay = GetComponentInChildren<StatsDisplay>(true);

            attackIndicator.enabled = false;

            Initialize();

            //Time.timeScale = 0.2f;


        }

        protected override void Start()
        {
            playerAnimator = GetComponent<PlayerAnimator>();
            playerInput = GetComponent<PlayerInput>();
            playerMovement = GetComponent<PlayerMovement>();
            projectileSpawner = GetComponentInChildren<ProjectileSpawner>();

            healParticle.playbackSpeed = 2f;
            manaParticle.playbackSpeed = 2f;
            levelUpParticle.playbackSpeed = 2f;

            UpdateActionSlots();

            CharacterAnimator = GetComponentInChildren<Animator>();

            //Health setup
            base.Start();
            healthBar.SetMaxValue(maxHealth);

            //Mana setup
            currentMana = maxMana;
            manaBar.SetMaxValue((int)maxMana);

            EnableRagdoll(false);
        }

        public void SetStats()
        {
            level = levelSystem.GetCurrentLevel();
            statsDisplay.UpdateStatsText();
        }

        public void UpgradeLevel(object sender, System.EventArgs e)
        {
            level = levelSystem.GetCurrentLevel();
            statsDisplay.UpdateStatsText();
            levelUpParticle.Play();
        }

        public void ApplySkillPoint(SkillSO skill)
        {
            // Apply skill stats
            foreach (Stat bonusStat in skill.Stats)
            {
                ApplyStats(bonusStat);
            }

            SetOrbsValues();

            statsDisplay.UpdateStatsText();
        }

        public void SetEquipment(EquipmentSO equipment, int apply = 1)
        {
            // Apply main stat
            ApplyStats(equipment.mainStat, apply);
            // Apply bonus stats
            foreach (Stat bonusStat in equipment.Stats)
            {
                ApplyStats(bonusStat, apply);
            }

            SetOrbsValues();

            statsDisplay.UpdateStatsText();
        }

        private void SetOrbsValues()
        {
            health = MaxHealth;
            currentMana = MaxMana;
            healthBar.SetMaxValue(maxHealth);
            manaBar.SetMaxValue((int)maxMana);
        }

        private void ApplyStats(Stat stat, int apply = 1)
        {
            switch (stat.type)
            {
                case BonusStatType.Damage:
                    damage += (int)stat.statImprovement * apply;
                    break;
                case BonusStatType.Defense:
                    defense += (int)stat.statImprovement * apply;
                    break;
                case BonusStatType.Health:
                    maxHealth += (int)stat.statImprovement * apply;
                    break;
                case BonusStatType.HealthRegen:
                    healthRegeneration += stat.statImprovement * apply;
                    break;
                case BonusStatType.Mana:
                    maxMana += stat.statImprovement * apply;
                    break;
                case BonusStatType.ManaRegen:
                    manaRegeneration += stat.statImprovement * apply;
                    break;
                case BonusStatType.Movement:
                    // TODO: Add movement speed
                    break;
                case BonusStatType.AttackSpeed:
                    attackSpeed += stat.statImprovement * apply;
                    break;
            }
        }

        public void RefillHealth(bool isFlask = false, float amount = 0)
        {
            if (isFlask)
            {
                health += amount;
                healParticle.Play();
            }
            else
            {
                health += healthRegeneration * Time.deltaTime;
            }

            health = Mathf.Clamp(health, 0, maxHealth);
        }

        public void RefillMana(bool isFlask = false, float amount = 0)
        {
            if (isFlask)
            {
                currentMana += amount;
                manaParticle.Play();
            }
            else
            {
                currentMana += manaRegeneration * Time.deltaTime;
            }

            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        }

        public void OnUpdate()
        {
            HandleAttackDelay();
            slotManager.HandleCooldowns();
            playerAnimator.HandleAnimations(IsAttacking);

            attackIndicator.enabled = IsAttacking;
            
            // Handle attacking
            if (IsAttacking)
            {
                if (Agent.enabled) Agent.isStopped = true;
                HandleRotation(mouseInput.mouseInputPosition);
            }

            if (isRolling)
            {
                playerMovement.RollUpdate();
            }

            if (EventHandler.Main.CurrentEvent is DialougeManager)
            {
                if (DialougeManager.Instance.currentNPC != null)
                {
                    HandleRotation(DialougeManager.Instance.currentNPC.transform.position);
                }
            }

            isInteracting = isRolling || isAttacking;


            if (currentObject != null)
            {
                var interactableTransform = (currentObject as MonoBehaviour)?.transform;

                if (Vector3.Distance(transform.position, interactableTransform.position) < 2)
                {
                    currentObject.Trigger();
                    currentObject = null;
                    Agent.isStopped = true;
                }
            }

            // Check if the Y position has changed
            float currentYPosition = transform.position.y;
            if (Mathf.Abs(currentYPosition - lastYPosition) > Mathf.Epsilon)
            {
                lastYPosition = currentYPosition;
                OnYValueChanged?.Invoke(currentYPosition); // Trigger the event
            }
        }

        public void OnFixedUpdate()
        {
            // Handle movement
            if (playerInput.IsMoving()) playerInput.Move();
        }

        public void SetCurrentObject(Interactable obj)
        {
            currentObject = obj;
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            if (bloodSplashEffect)
                bloodSplashEffect.Play();
            SoundManager.PlaySound(SoundType.HURT);
            healthBar.SetValue(health);
        }


        public override void Attack(int attackIndex)
        {
            if (CanAttack)
            {
                // Check if the index is valid
                if (attackIndex >= 0 && attackIndex < slotManager.actionSlots.Length)
                {
                    // Get the item from the corresponding action slot
                    ActionItemSO actionItem = slotManager.actionSlots[attackIndex].item as ActionItemSO;

                    if (actionItem != null && actionItem.timerCooldown >= actionItem.cooldown)
                    {
                        CurrentAction = actionItem;

                        // Handle attack item
                        if (actionItem is AttackTypeSO attackTypeAction)
                        {
                            if (currentMana > attackTypeAction.manaCost)
                            {
                                Agent.SetDestination(transform.position);
                                attackTypeAction.PerformAction(CharacterAnimator);

                                // Handle mana
                                currentMana -= attackTypeAction.manaCost;
                                manaBar.SetValue((int)currentMana);

                                IsAttacking = true;
                                projectileSpawner.projectile = attackTypeAction.projectile;
                            }
                            else
                            {
                                CurrentAction = null;

                                PopupText text = ObjectPooling.Instance.SpawnFromPool("PopupText", transform.position + textOffset, Quaternion.identity).GetComponent<PopupText>();
                                text.message = "Not enough mana";
                                text.StartCoroutine("Trigger");
                            }
                        }
                        // Handle potion item
                        if (actionItem is PotionSO potionSO)
                        {
                            potionSO.PerformAction(CharacterAnimator);
                            slotManager.actionSlots[attackIndex].itemAmount -= 1;
                            slotManager.actionSlots[attackIndex].UpdateItemAmountText();
                        }
                    }
                }
                else
                {
                    Debug.Log("Not a valid index");
                }
            }
        }


        public void UpdateActionSlots()
        {
            slotManager.SetUpSlots(); // Refresh UI slots
        }


        private void HandleAttackDelay()
        {
            if (IsAttacking && currentAction != null)
            {
                if (currentAction is AttackTypeSO attackTypeAction)
                {
                    attackTimer += Time.deltaTime;
                    
                    if (attackTimer >= StatsCalculator.CalculateAttackSpeed(attackTypeAction.nextAttackDelay))
                    {
                        if (playerMovement.hasBufferedInput)
                        {
                            ClearAttack();
                            playerMovement.ProcessBufferedInput(true);
                            return;
                        }

                        if (!IsAnimationPlayingStrict(attackTypeAction.attackHashString) && !playerMovement.hasBufferedAttack)
                        {
                            ClearAttack();
                            playerMovement.ResetBufferedInput();
                        }
                    }
                }
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
            EnableRagdoll(true);
            currentPlayerState = State.Dead;
            Popup.Create<DeathScreen>();
        }

        public override void Pause()
        {
            Agent.isStopped = true;
            CharacterAnimator.speed = 0;
        }

        public override void UnPause()
        {
            Agent.isStopped = false;
            CharacterAnimator.speed = 1;
        }
    }

}
