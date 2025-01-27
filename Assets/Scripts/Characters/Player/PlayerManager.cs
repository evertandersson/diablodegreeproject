using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
        private PlayerMovement playerMovement;
        private ProjectileSpawner projectileSpawner;

        [SerializeField]
        public MouseInput mouseInput;

        [SerializeField]
        private State currentPlayerState;

        public SlotManager slotManager;
        public Inventory inventory;
        private StatsDisplay statsDisplay;
        public SkillTreeManager skillTree;

        private ActionItemSO currentAction;

        [SerializeField]
        private PlayerHealthOrb healthBar;

        [SerializeField]
        private PlayerManaOrb manaBar;

        // Mana:
        private float currentMana;
        [Header("Mana")]
        [SerializeField] private float maxMana = 50;
        [SerializeField] private float manaRegeneration = 2;

        public bool isInteracting;
        private bool isAttacking;
        private bool canAttack = true;
        private bool isRolling;

        private Interactable currentObject = null;

        public float attackTimer;

        [SerializeField] private RawImage attackIndicator;

        // Particles effects
        [SerializeField] private ParticleSystem healParticle;
        [SerializeField] private ParticleSystem manaParticle;
        [SerializeField] private ParticleSystem levelUpParticle;

        #region Properties

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
                        break;

                    case State.Attack:
                        break;

                    case State.Rolling:
                        playerMovement.RollStart();
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
            private set
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

            statsDisplay = GetComponentInChildren<StatsDisplay>(true);

            attackIndicator.enabled = false;

            Initialize();
        }

        protected override void Start()
        {
            playerAnimator = GetComponent<PlayerAnimator>();
            playerInput = GetComponent<PlayerInput>();
            playerMovement = GetComponent<PlayerMovement>();
            projectileSpawner = GetComponentInChildren<ProjectileSpawner>();

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

        public override void SetStats()
        {
            base.SetStats();
            statsDisplay.UpdateStatsText();
        }

        public void UpgradeLevel(object sender, System.EventArgs e)
        {
            level = levelSystem.GetCurrentLevel();
            statsDisplay.UpdateStatsText();
        }

        public void ApplySkillPoint(SkillSO skill)
        {
            maxHealth += skill.healthIncrease;
            health = maxHealth;
            damage += skill.damageIncrease;
            defense += skill.defenceIncrease;
            healthBar.SetMaxValue(maxHealth);

            statsDisplay.UpdateStatsText();
        }

        public void SetEquipment(EquipmentSO equipment, int apply = 1)
        {
            maxHealth += equipment.healthIncrease * apply;
            damage += equipment.damageIncrease * apply;
            defense += equipment.defenseIncrease * apply;
            healthBar.SetMaxValue(maxHealth);

            statsDisplay.UpdateStatsText();
        }

        public void Heal(int amount)
        {
            health += amount;
            healthBar.SetValue(health);
            healParticle.Play();
            healParticle.playbackSpeed = 2f;
        }
        public void RefillMana(bool isFlask = false, float amount = 0)
        {
            if (isFlask)
            {
                currentMana += amount;
                manaParticle.Play();
                manaParticle.playbackSpeed = 2f;
            }
            else
            {
                currentMana += manaRegeneration * Time.deltaTime;
            }

            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        }

        public void Update()
        {
            HandleAttackDelay();
            slotManager.HandleCooldowns();
            playerAnimator.HandleAnimations(IsAttacking);

            // Handle movement
            if (playerInput.IsMoving()) playerInput.Move();

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
                    if (attackTimer <= attackTypeAction.nextAttackDelay)
                    {
                        attackTimer += Time.deltaTime;
                    }
                    else
                    {
                        ClearAttack();
                        currentPlayerState = State.Idle;
                        playerMovement.ProcessBufferedInput(true);
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
    }

}
